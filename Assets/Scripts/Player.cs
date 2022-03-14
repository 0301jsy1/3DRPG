using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
//Animator가 없는 게임오브젝트에 Player 클래스를 컴포넌트로 추가하면 Animator도 자동으로 추가됨
public class Player : Entity
{
    protected override void Recovery()
    {
        base.Recovery();

        int buff_bonus_hp = 0;
        HP += buff_bonus_hp;

        int buff_bonus_mp = 0;
        MP += buff_bonus_mp;
    }
    public override int HPMax
    {
        get
        {
            int base_hp = 100 + Level * 10;
            int equip_bonus = 0;
            int attr_bonus = Health * 20;

            return base_hp + equip_bonus + HpBuffBonus + attr_bonus;
        }
    }
    public override int HPRecovery
    {
        get
        {
            int base_HpRec = Level;
            int equip_bonus = 0;
            int buff_bonus = 0;
            int attr_bonus = Health;

            return base_HpRec + equip_bonus + buff_bonus + attr_bonus;
        }
    }
    public override int MPMax
    {
        get
        {
            int base_mp = 100 + Level * 10;
            int equip_bonus = 0;
            int attr_bonus = Mana * 10;

            return base_mp + equip_bonus + MpBuffBonus + attr_bonus;
        }
    }
    public override int MPRecovery
    {
        get
        {
            int base_MpRec = Level;
            int equip_bonus = 0;
            int buff_bonus = 0;
            int attr_bonus = Mana;

            return base_MpRec + equip_bonus + buff_bonus + attr_bonus;
        }
    }
    public override int Physics_Damage
    {
        get
        {
            int base_dmg = 10 + Level;
            int equip_bonus = 0;
            int attr_bonus = Strength * 2;

            return base_dmg + equip_bonus + DamageBuffBonus + attr_bonus;
        }
    }
    public override int Magic_Damage
    {
        get
        {
            int base_mdmg = 10 + Level * 2;
            int equip_bonus = 0;
            int attr_bonus = Intelligence * 4;

            return base_mdmg + equip_bonus + DamageBuffBonus + attr_bonus;
        }
    }
    public override int Physics_Defense
    {
        get
        {
            int base_def = 5 + Level;
            int equip_bonus = 0;
            int attr_bonus = Strength + Health;

            return base_def + equip_bonus + DefenseBuffBonus + attr_bonus;
        }
    }
    public override int Magic_Defense
    {
        get
        {
            int base_mdef = 5 + Level;
            int equip_bonus = 0;
            int attr_bonus = Intelligence + Mana;

            return base_mdef + equip_bonus + DefenseBuffBonus + attr_bonus;
        }
    }

    [Header("Attribute Points")]
    [SerializeField]
    int _att_point;
    public int Att_Point
    {
        get { return _att_point; }
        set { _att_point = Mathf.Clamp(value, 0, value); }
    }

    [Header("Experience")]
    [SerializeField]
    long _exp;
    public long Exp
    {
        get { return _exp; }
        set
        {
            _exp = value;
            if (_exp >= ExpMax)
            {
                _exp -= ExpMax;

                if (Level * 0.3 < 1.0) Att_Point++;
                else Att_Point = Att_Point + (int)(Level * 0.3);

                _lv++;

                HP = HPMax;
                MP = MPMax;
            }
        }
    }

    public float Exp_Percent() { return (Exp != 0 && ExpMax != 0) ? (float)Exp / (float)ExpMax : 0.0f; }
    public long ExpMax { get { return Level * Level * 100; } }

    private Animator anim;
    private float idle_time;

    private Vector3 goal_pos;

    private GameObject target_portal;
    private Rigidbody _rigid;

    [SerializeField]
    Target targetMark;

    bool is_attack_target = false;
    //플레이어의 공격 범위인 AttackRange 때문에 플레이어가 마우스 왼쪽 버튼을 클릭해 공격하지 않아도 
    //범위 안에 있는 적을 또 공격하기 때문에 공격하지 않게 제어

    void OnTriggerEnter(Collider col)
    //void OnCollisionEnter(Collision col)
    {

        /*if (target_portal != null )
               {
                   if (col.gameObject.name.Contains("Portal"))
                   {
                       if (col.gameObject == target_portal)
                       {
                           Portal portal = col.gameObject.GetComponent<Portal>() as Portal;
                           portal.Move_To(this);

                           target_portal = null;

                           anim.Play("Idle_Base");
                           player_state = PLAYER_STATE.IDLE;
                       }
                   }
               }*/

        /*
         if (target_portal != null && col.gameObject.name.Contains("Portal") && col.gameObject == target_portal)
          {    
                   Portal portal = col.gameObject.GetComponent<Portal>() as Portal;
                   portal.Move_To(this);

                   target_portal = null;

                   anim.Play("Idle_Base");
                   player_state = PLAYER_STATE.IDLE;
           }
         */

        //target이 없거나 현재 부딪힌 오브젝트의 이름에 "Portal"이 포함되어 있지 않을 땐 return
        if (target_portal == null) return;
        //내가 클릭한 것이 포털일 때 넘어가는 조건(함수를 종료하는 조건)
        if (!col.gameObject.name.Contains("Portal")) return;
        //내가 부딛친 오브젝트가 포털일 때 넘어가는 조건(함수를 종료하는 조건)
        if (col.gameObject != target_portal) return; 
        // 클릭한 오브젝트와 부딛친 오브젝트가 같지 않으면 넘어가는 조건(함수를 종료하는 조건)
        Portal portal = col.gameObject.GetComponent<Portal>() as Portal;
        portal.Move_To(this);
        //Portal 클래스의 Move_To()메소드를 이용해 캐릭터를 이동시키고 target을 해제

        target_portal = null;
        anim.Play("Idle_Base");
        _state = ENTITY_STATE.IDLE;
        //이동 상태로 포탈에 도착하였기 때문에 이동 후엔 대기 상태로 변경해 주어야 함
    }

    void OnCollisionExit(Collision col)
    {
        _rigid.isKinematic = true;
        //오브젝트와의 충돌 후 물리력을 제거하기 위한 isKinematic = true
    }

    void Awake()
    {
        base.Init();

        anim = GetComponent<Animator>();
        _state = ENTITY_STATE.IDLE;
        idle_time = .0f;

        move_speed = 10.0f;
        goal_pos = Vector3.zero;

        target_portal = null;
        _rigid = GetComponent<Rigidbody>();

        for (int i = 0; i < skills.Count; ++i)
        {
            Debug.Log("SkillName:" + skills[i].name + ", SkillDamage:" + skills[i].damage);
        }
    }

    void Update()
    {
        Update_Inputs();
        Update_Actions();
        Update_Skills();
    }

    void Update_Inputs()
    {
        Ray ray;
        RaycastHit hit;

        if ( _state != ENTITY_STATE.CALCSKILLRANGE && magicCircle != null)
            Destroy(magicCircle);

        if (_state == ENTITY_STATE.CALCSKILLRANGE )
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                magicCircle.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
            }
            if (Input.GetMouseButtonDown(0))
            {
                _state = ENTITY_STATE.CASTING;
                posRangeSkill = magicCircle.transform.position;
                Destroy(magicCircle);
                CastingSkill(cur_skill);
            }
        }

        #region Left-Mouse Button Down
        if (Input.GetMouseButtonDown(0))
        {
            // 
            if (EventSystem.current.IsPointerOverGameObject()) return;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                var entity = hit.transform.GetComponent<Entity>() as Entity;
                if (entity && entity != this)
                {
                    if (entity != null &&
                        entity.GetComponent<Enemy>().GetCurrentState() == Enemy01.States.Die.Instance)
                    {
                        _target = null;
                        return;
                    }
                    //_target이 있는 위치까지 이동하여  AttackRange Collider에 의해 거리가 가까워지면 자동으로 공격
                    if (_target == entity) //동일 _target을 2번 이상 눌렀을 때
                    {
                        is_attack_target = true;
                        if (Vector3.Distance(_target.Get_Pos(), Get_Pos()) >= 0.85f) //_target과의 거리를 계산하여 거리가 멀 때
                        {
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                move_speed = .5f;
                                anim.Play("Run_SilentWalk");
                            }
                            else
                            {
                                move_speed = 10.0f;
                                anim.Play("Run_Base");
                            }
                            _state = ENTITY_STATE.MOVE;
                            goal_pos = _target.Get_Pos();
                        }
                        else //_target과의 거리를 계산하여 거리가 가까울 때
                        {
                            Base_Attack(); //기본 공격을 하는 메소드
                        }
                        transform.localRotation = Quaternion.LookRotation(_target.Get_Pos() - Get_Pos());
                        return;
                    }
                    _target = entity;
                    _target._target = this;     // 내가 선택한 적이 나를 타겟으로 설정하게 함.
                    targetMark.Target_On(_target.Get_Pos());
                    targetMark.transform.SetParent(_target.transform);
                }
                else
                {
                    _target = null;
                    targetMark.Target_Off();
                    targetMark.transform.SetParent(null);
                }
            }
        }
        #endregion

        //마우스 오른쪽 클릭시 Mouse Picking을 이용해 이동
        #region Right-Mouse Button Down
        if (Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.LeftControl)) return;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name.Equals("PC_01")) return;
                //캐릭터를 클릭했을 땐 반응하지 않음
                if (hit.transform.name.Contains("Enemy")) return;
                //적을 클릭했을 땐 반응하지 않음
                if (hit.transform.name.Contains("Portal"))
                {
                    target_portal = hit.transform.gameObject;
                }
                //포털을 클릭했을 땐 target으로 설정
                else
                    target_portal = null;                

                //왼쪽 Shift를 누르고 있는 상태면 속도 0.5에 걷기 모션 실행 아니면 속도 10에 뛰기 모션 실행
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    move_speed = .5f;
                    anim.Play("Run_SilentWalk");
                }
                else
                {
                    move_speed = 10.0f;
                    anim.Play("Run_Base");
                }

                _state = ENTITY_STATE.MOVE;
                
                goal_pos = hit.point;
                goal_pos.y = .0f;
                //마우스 오른쪽 클릭을 하게 되면 카메라로 부터 마우스 좌표로 광선을 쏴 
                //부딪힌 오브젝트의 세부 좌표를 hit.point를 통해 받아옴

                transform.localRotation = Quaternion.LookRotation(goal_pos - Get_Pos());
                //이동방향 바라보기
            }
        }
        #endregion
    }

    void Update_Actions()
    {
        switch (_state)
        {
            #region ENTITY_STATE.IDLE
            case ENTITY_STATE.IDLE:
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Base")) 
                    //기본 상태(Idle_Base)일 때만 일정 시간마다 두리번 거리는 상태를 1회씩 재생
                {
                    if (idle_time < 5.0f) idle_time += Time.deltaTime;
                    else
                    {
                        idle_time = .0f;
                        anim.SetTrigger("idle_lookingAround");
                    }
                }

                if (Input.GetKeyDown(KeyCode.P))
                    //현재는 체력 시스템이 없기 때문에 P키를 이용해 기본 상태에서 피곤한 상태로 변경
                {
                    anim.SetBool("idle_is_tired", !anim.GetBool("idle_is_tired"));
                }
                break;
            #endregion

            #region ENTITY_STATE.MOVE
            case ENTITY_STATE.MOVE:
                //왼쪽 Shift를 누르고 있는 상태면 속도 0.5에 걷기 모션 실행 아니면 속도 10에 뛰기 모션 실행
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    move_speed = .5f;
                    anim.Play("Run_SilentWalk");
                }
                else
                {
                    move_speed = 10.0f;
                    anim.Play("Run_Base");
                }

                //현재 좌표와 목표 좌표 사이의 거리가 멀면 이동하고 가까우면 상태를 대기(Idle)로 변경
                Vector3 move_pos = Vector3.zero;
                if (Vector3.Distance(goal_pos, Get_Pos()) > .1f)
                    move_pos = Vector3.Normalize(goal_pos - Get_Pos());
                else
                {
                    anim.Play("Idle_Base"); //목표지점에 도달하면 대기 상태로 변경

                    Set_Pos(goal_pos);
                    _state = ENTITY_STATE.IDLE;
                }
                Add_Pos(move_pos * move_speed * Time.deltaTime);

                if (_rigid.isKinematic == true)
                    _rigid.isKinematic = false;
                //OnCollisionExit()에서 해제한 물리를 다시 적용
                break;
            #endregion

            /*#region ENTITY_STATE.HIT
            case ENTITY_STATE.HIT:
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Base"))
                    _state = ENTITY_STATE.IDLE;
                break;
            #endregion*/
        }
    }

    public void Base_Attack() //기본 공격 실행 메소드
    {
        if (is_attack_target == false) return;

        anim.Play("Attack_Base");
        _state = ENTITY_STATE.ATTACK;

        is_attack_target = false;
    }

    public void Update_Physics_Attack() //캐릭터의 기본 공격이 적에게 맞는 시점에 호출되는 함수
    {
        //Bug Report//
        // Player Class의 Base_Attack() 메소드 실행으로 적을 공격할 때
        // Update_Physics_Attack() 메소드가 실행되기 전에 마우스 왼쪽 클릭으로 Target을
        // 없애버리면 Update_Physics_Attack() 메소드의
        // Enemy enemy = _target.GetComponent<Enemy>() as Enemy; 코드에서
        // _target에 대해 NullException 오류 발생
        Enemy enemy = _target.GetComponent<Enemy>() as Enemy;

        if (enemy._target != this)
            enemy._target = this;

        enemy.Hit_Type = 1;
        enemy.ChangeState(Enemy01.States.Hit.Instance);
    }

    public void End_Physics_Attack() //캐릭터의 기본 공격이 끝나는 시점에 호출되는 함수
    {
        anim.Play("Idle_Base");
        _state = ENTITY_STATE.IDLE;
    }

    public void Hit(Enemy enemy)
    {
        if (_state != ENTITY_STATE.MOVE)
        {
            anim.Play(null);
            anim.Play("Hit_Front");
        }

        int dmg = enemy.Physics_Damage - Physics_Defense;
        dmg = Mathf.Clamp(dmg, 1, 99999);

        HP -= dmg;
        Damage_Popup("" + dmg);

        if (HP <= 0)
        {
            Debug.Log("캐릭터 사망");
        }
    }

    void Damage_Popup(string _dmg)
    {
        GameObject clone = Instantiate(damage_popup_prefab) as GameObject;

        Bounds bounds = GetComponent<Collider>().bounds;
        clone.transform.position = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

        clone.GetComponent<TextMesh>().text = _dmg;
    }

    void Update_Skills()
    {
        if (Input.anyKeyDown)
        {
            if (_state == ENTITY_STATE.CALCSKILLRANGE) return;

            int key = 0;
            if (int.TryParse(Input.inputString, out key))
            {
                if (Level <= 8)
                {
                    if (key >= 1 && key <= Level)
                        BeforeCastSkill(skills[key - 1]);
                }
                else
                {
                    if (key >= 1 && key <= 8)
                    {
                        //if (key == 2 || key == 4)
                        //    transform.localRotation = Quaternion.LookRotation(_target.Get_Pos() - Get_Pos());
                        BeforeCastSkill(skills[key - 1]);
                    }
                }
            }
        }
    }

    public GameObject magicCircle;

    public override bool CanAttackType(System.Type t)
    {
        // 공격할 수 있는 대상은 플레이어 또는 적
        return t == typeof(Player) || t == typeof(Enemy);
    }

    public override void DealDamageAt(Entity _entity, int _damage, float aoeRadius = .0f)
    {
        _entity.GetComponent<Enemy>().Hit_Type = 2;
        _entity.GetComponent<Enemy>().ChangeState(Enemy01.States.Hit.Instance);
        // 목표(_entity)의 체력을 _damage만큼 감소
    }

    public override void CalcSkillRange(GameObject clone)
    {
        anim.Play("Idle_Base");
        _state = ENTITY_STATE.CALCSKILLRANGE;
        magicCircle = clone;
    }
}
