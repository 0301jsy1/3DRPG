using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]	// Kinematic=true, only needed for OnTrigger
//Rigidbody가 없는 게임오브젝트에 Entity 클래스를 컴포넌트로 추가하면 Rigidbody도 자동으로 추가됨
//Entity 클래스에 의해 Rigidbody는 지울 수 없는 컴포넌트로 보호됨
public abstract class Entity : MonoBehaviour //abstract : 추상 클래스
{
    [Header("Target")] //Inspector View의 필드에 Target 헤더를 추가
    public Entity _target;

    [Header("ID/Class/Level/State")]
    [SerializeField]
    protected string _id;
    public string ID { get { return _id; } }

    [SerializeField]
    protected ENTITY_CLASS _class;
    public ENTITY_CLASS Class { get { return _class; } }

    [SerializeField]
    protected int _lv;
    public int Level { get { return _lv; } }
    public int LevelMax { private set; get; }

    [SerializeField]
    protected ENTITY_STATE _state;
    public ENTITY_STATE State { get { return _state; } }

    [Header("Attributes")]
    [SerializeField]
    protected int _strength;
    public int Strength
    {
        get { return _strength; }
        set { _strength = Mathf.Clamp(value, 0, value); }
        //현재 value 값을 min과 max 사이의 값으로 고정 시키기 위한 함수
    }
    [SerializeField]
    protected int _intelligence;
    public int Intelligence
    {
        get { return _intelligence; }
        set { _intelligence = Mathf.Clamp(value, 0, value); }
    }
    [SerializeField]
    protected int _health;
    public int Health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, value); }
    }
    [SerializeField]
    protected int _mana;
    public int Mana
    {
        get { return _mana; }
        set { _mana = Mathf.Clamp(value, 0, value); }
    }

    [Header("Damage Popup")]
    [SerializeField]
    protected GameObject damage_popup_prefab;

    [Header("HP/MP")]
    [SerializeField]
    protected bool invincible = false;  // GM, NPC, Player Get Item
    [SerializeField]
    protected int _hp = 100;
    public int HP
    {
        get { return _hp; }
        set { _hp = Mathf.Clamp(value, 0, HPMax); }
    }
    public abstract int HPRecovery { get; } // Per Second
    public abstract int HPMax { get; }

    [SerializeField]
    protected int _mp = 100;
    public int MP
    {
        get { return _mp; }
        set { _mp = Mathf.Clamp(value, 0, MPMax); }
    }
    public abstract int MPRecovery { get; } // Per Second
    public abstract int MPMax { get; }

    // Other properties
    public abstract int Physics_Damage { get; }
    public abstract int Magic_Damage { get; }
    public abstract int Physics_Defense { get; }
    public abstract int Magic_Defense { get; }

    public float move_speed { set; get; }

    /// <summary>
    /// Entity의 HP/MP 회복 함수
    /// </summary>
    protected virtual void Recovery()
    {
        if (!enabled) return;

        if (_hp < HPMax)
        {
            _hp += HPRecovery;
            if (_hp >= HPMax) _hp = HPMax;
        }
        if (_mp < MPMax)
        {
            _mp += MPRecovery;
            if (_mp >= MPMax) _mp = MPMax;
        }
    }
    public float Hp_Percent() { return (_hp != 0 && HPMax != 0) ? (float)_hp / (float)HPMax : 0.0f; }
    public float Mp_Percent() { return (_mp != 0 && MPMax != 0) ? (float)_mp / (float)MPMax : 0.0f; }

    public void Init()
    {
        LevelMax = 99;

        HP = HPMax;
        MP = MPMax;

        InvokeRepeating("Recovery", 1.0f, 1.0f);
        //InvokeRepeating(호출할 함수명, 최초 실행 지연 시간, 반복 실행 시 지연 시간)
        //특정 시간에 한번씩 반복 호출할 때 사용하는 유니티 함수

        cur_skill = null;
        skills = new List<SkillBase>();
        foreach (var t in SkillTemplate.SkillDict)
        {
            if (t.Value.learnClassType == ENTITY_CLASS.NOOB || t.Value.learnClassType == _class)
            {
                skills.Add(new SkillBase(t.Value));
            }
        }
        //skills[0].name
        //skills[0].skillImage
    }

    public SkillBase cur_skill; // 현재 Entity가 선택해 활성화 된 스킬
    public List<SkillBase> skills;      // Entity가 사용가능한 모든 스킬(배운스킬, 배우지 않은 스킬 모두 표시)

    /// <summary>
    /// 스킬을 사용하려고 할 때 최초 호출되는 함수로
    /// 현재 스킬 사용이 가능한지 검사
    /// </summary>
    /// <param name="_skill">플레이어가 사용하려고 선택한 스킬 정보</param>
    public void BeforeCastSkill(SkillBase _skill)
    {
        // 1. 스킬 사용이 가능한 상태인지 검사(캐릭터의 상태, CoolDown Time, MP 등)
        int castCheckSelf = CastCheckSelf(_skill);
        if (castCheckSelf == 1)
        {
            Debug.Log("플레이어가 죽어있거나 상태이상으로 인해 스킬 사용이 불가능 합니다.");
            return;
        }
        else if (castCheckSelf == 2)
        {
            Debug.Log("스킬의 쿨타임이 초기화되지 않았습니다.");
            return;
        }
        else if (castCheckSelf == 3)
        {
            Debug.Log("마나가 부족합니다.");
            return;
        }

        // 범위 스킬의 경우 범위 지정으로 이동
        if (_skill.skillAttackType == SKILL_ATTACK_TYPE.MULTIRANGE)
        {
            cur_skill = _skill;

            GameObject clone = Instantiate(_skill.effectSkillRange) as GameObject;
            clone.transform.localScale = new Vector3(_skill.aoeRadius, _skill.aoeRadius, 1.0f); //new Vector3(_skill.aoeRadius, .1f, _skill.aoeRadius);
            CalcSkillRange(clone);
            return;
        }

        // 2. 스킬을 사용하려는 대상에 대한 검사
        if (!CastCheckTarget(_skill)) return;

        // 3. 타겟과의 거리 계산
        if (!CastCheckDistance(_skill))
        {
            // Target과 나의 거리가 _skill.CastRange와 같거나 작을 때까지 이동

            // 이동 완료 전에 타겟이 없어지거나 다른 곳으로 이동, 다른 마법 사용을 하면 스킬 취소
            
            Debug.Log("타겟과의 거리가 멉니다.");
            return;
        }

        // 4. 스킬 캐스팅
        CastingSkill(_skill);
    }
    /// <summary>
    /// 1. 스킬 사용이 가능한 상태인지 검사(캐릭터의 상태, CoolDown Time, MP 등)
    /// </summary>
    /// <param name="_skill"></param>
    /// <returns></returns>
    public int CastCheckSelf(SkillBase _skill)
    {
        if (HP <= 0)                                // 캐릭터가 죽은 상태일 때(or 상태이상)
            return 1;
        else if (_skill.IsSkillReady() == false)    // 스킬의 쿨타임이 초기화 되지 않았을 때
            return 2;
        else if (MP < _skill.manaCost)          // 스킬을 사용할 마나가 없을 때
            return 3;

        return 0;
    }
    /// <summary>
    /// 2. 스킬을 사용하려는 대상에 대한 검사
    /// </summary>
    /// <param name="_skill"></param>
    /// <returns></returns>
    public bool CastCheckTarget(SkillBase _skill)
    {
        // 스킬의 속성이 공격이면
        if (_skill.skillType == SKILL_TYPE.ATTACK)
        {
            // 공격할 대상이 있고, 그 대상이 본인이 아니고,
            // 대상의 체력이 남아있고, 대상의 속성이 Player or Enemy일 때
            return _target != null && _target != this &&
                   _target.HP > 0 && CanAttackType(_target.GetType());
        }
        // 힐, 버프 스킬은 현재는 본인에게 사용이기 때문에 별다른 조건 검사 없이 종료
        else if (_skill.skillType == SKILL_TYPE.HEAL || _skill.skillType == SKILL_TYPE.BUFF)
            return true;

        return false;
    }
    /// <summary>
    /// 3. 타겟과의 거리를 계산(거리가 멀다면 이동 후 시전)
    /// </summary>
    /// <param name="_skill"></param>
    /// <returns></returns>
    public bool CastCheckDistance(SkillBase _skill)
    {
        if (_skill.skillType != SKILL_TYPE.ATTACK) return true; //공격스킬이 아니면 참을 반환

        return _target != null &&
            Vector3.Distance(Get_Pos(), _target.Get_Pos()) <= _skill.castRange; //공격 스킬에서 거리 계산
    }
    /// <summary>
    /// 4. 스킬의 캐스팅 시간동안 캐스팅 진행
    /// 이 상태에서 이동을 하거나 상태에 변화가 찾아오면 스킬 시전 취소
    /// </summary>
    /// <param name="_skill"></param>
    public void CastingSkill(SkillBase _skill)
    {
        cur_skill = _skill;

        // 스킬 캐스팅 이펙트가 있으면 캐스팅 할 때 이펙트 효과 방출
        if (_skill.effectCastSkill != null)
        {
            GameObject clone = Instantiate(_skill.effectCastSkill) as GameObject;
            clone.transform.position = Get_Pos() + Vector3.up * 0.5f;

            // 캐스팅 시간이 지나면 캐스팅 이펙트 삭제
            Destroy(clone, _skill.castTime);
        }

        // 캐스팅 시간이 지나면 스킬 사용
        Invoke("CastSkill", _skill.castTime);
    }
    /// <summary>
    /// 5. 모든 조건을 만족하여 스킬을 시전하는 상태
    /// </summary>
    public void CastSkill()
    {
        MP -= cur_skill.manaCost;
        cur_skill.cooldownEnd = Time.time + cur_skill.cooldown;

        // 스킬의 특성이 공격일 때
        if (cur_skill.skillType == SKILL_TYPE.ATTACK)
        {
            // 스킬의 공격 타입에 따라 재 분류

            // 근거리 공격 스킬
            if (cur_skill.skillAttackType == SKILL_ATTACK_TYPE.NOT_FIRE)
            {
                Debug.Log("NOT_FIRE ATTACK");
                
                // 타격에 대한 모션
                DealDamageAt(_target, Magic_Damage + cur_skill.damage, cur_skill.aoeRadius);
            }
            // 원거리 단일 or 스플래시 공격 스킬
            else if (cur_skill.skillAttackType == SKILL_ATTACK_TYPE.SINGLE ||
                      cur_skill.skillAttackType == SKILL_ATTACK_TYPE.MULTISPLASH)
            {
                Debug.Log("SINGLE or MULTISPLASH Attack");

                GameObject clone = Instantiate(cur_skill.projectile.gameObject) as GameObject;
                clone.transform.position = Get_Pos() + Vector3.up * 0.5f;

                Projectile projectile = clone.GetComponent<Projectile>();
                Debug.Log(Magic_Damage + cur_skill.damage);
                projectile.Init(this, _target, Magic_Damage + cur_skill.damage, cur_skill.aoeRadius);
            }
            // 원거리 범위 공격 스킬
            else if (cur_skill.skillAttackType == SKILL_ATTACK_TYPE.MULTIRANGE)
            {
                Debug.Log("MULTIRANGE Attack");

                int count = Random.Range(5, 11);
                for (int i = 0; i < count; ++i)
                {
                    Debug.Log(i);
                    GameObject clone = Instantiate(cur_skill.effectHitSkill) as GameObject;
                    clone.transform.position = new Vector3(
                        Random.Range(posRangeSkill.x - cur_skill.aoeRadius * 0.5f, posRangeSkill.x + cur_skill.aoeRadius * 0.5f),
                        .0f,
                        Random.Range(posRangeSkill.z - cur_skill.aoeRadius * 0.5f, posRangeSkill.z + cur_skill.aoeRadius * 0.5f));

                    Destroy(clone, 1.0f);
                }

                // 범위 안에 있는 적의 체력 감소
                GameObject[] enemys = GameObject.FindGameObjectsWithTag("ENEMY");
                for ( int i = 0; i < enemys.Length; ++ i )
                {
                    if ( Vector3.Distance(enemys[i].transform.position, posRangeSkill) <= cur_skill.aoeRadius * 0.5f )
                    {
                        // 범위에 들어온 적
                        // 타격에 대한 모션
                        DealDamageAt(enemys[i].GetComponent<Enemy>(), Magic_Damage + cur_skill.damage, cur_skill.aoeRadius);
                    }
                }
            }
            // 원거리 체인 공격 스킬
            else if (cur_skill.skillAttackType == SKILL_ATTACK_TYPE.MULTICHAIN)
            {
                Debug.Log("MULTICHAIN Attack");
            }
        }
        // 스킬의 특성이 회복 일 때
        else if (cur_skill.skillType == SKILL_TYPE.HEAL)
        {
            Debug.Log("HEAL SKILL");

            // 회복 이펙트 출력(1초)
            GameObject clone = Instantiate(cur_skill.effectHitSkill) as GameObject;
            clone.transform.position = Get_Pos();
            clone.transform.SetParent(transform);
            Destroy(clone, 1.0f);

            // 체력 회복
            HP += cur_skill.buffHeal;
        }
        // 스킬의 특성이 버프 일 때
        else if (cur_skill.skillType == SKILL_TYPE.BUFF)
        {
            Debug.Log("BUFF SKILL");

            // 버프 지속시간 동안 플레이어를 쫓아다니는 버프 Effect 생성
            GameObject clone = Instantiate(cur_skill.effectHitSkill) as GameObject;
            clone.transform.position = Get_Pos();
            clone.transform.SetParent(transform);
            Destroy(clone, cur_skill.buffTime);

            // 버프 효과를 주고 버프 종료 시간 계산
            HpBuffBonus += cur_skill.buffDamage;
            MpBuffBonus += cur_skill.buffDefense;
            DamageBuffBonus += cur_skill.buffHpMax;
            DefenseBuffBonus += cur_skill.buffMpMax;
            cur_skill.buffTimeEnd = Time.time + cur_skill.buffTime;
        }
    }

    [HideInInspector]
    public Vector3 posRangeSkill;

    [HideInInspector]
    public int HpBuffBonus = 0;
    [HideInInspector]
    public int MpBuffBonus = 0;
    [HideInInspector]
    public int DamageBuffBonus = 0;
    [HideInInspector]
    public int DefenseBuffBonus = 0;

    public abstract void CalcSkillRange(GameObject clone);
    public abstract void DealDamageAt(Entity _entity, int _damage, float aoeRadius = .0f);
    public abstract bool CanAttackType(System.Type t);

    public void Add_Pos(Vector3 p) { transform.position += p; }
    public void Set_Pos(Vector3 p) { transform.position = p; }
    public Vector3 Get_Pos() { return transform.position; }
}


