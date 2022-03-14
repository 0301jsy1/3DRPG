using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Skill", menuName = "ChoARPG_Skill", order = 999)]
public class SkillTemplate : ScriptableObject {

    public SKILL_TYPE skillType;
    public SKILL_ATTACK_TYPE skillAttackType;
    public ENTITY_CLASS learnClassType;

    public Sprite skillImage;
    public bool learnDefault;

    [TextArea(1, 30)]
    public string tooltip;
    public SkillLevel[] levels = new SkillLevel[] { new SkillLevel() };

    public GameObject effectCastSkill; // 스킬 시전 이펙트
    public GameObject effectSkillRange; //스킬 범위 지정 이펙트(MagicCircle )
    public GameObject effectHitSkill; // 스킬 타격 이펙트

    [System.Serializable]
    public struct SkillLevel
    {
        //Base Skill Data
        public int manaCost; //필요 마나
        public float castTime; //캐스팅 시간
        public float castRange; //공격 사정거리
        public float cooldown; //스킬 쿨 타임
        public float aoeRadius; //광역 스킬의 범위

        //Attack Typed
        public int damage; //공격력

        //Buff Types
        public float buffTime; //버프 지속시간
        public int buffDamage; //공격력 증가량
        public int buffDefense; //방어력 증가량
        public int buffHpMax; //체력 증가량
        public int buffMpMax; //마나 증가량
        public int buffHeal; //체력 회복량

        //require
        public int requiredLevel; //습득 가능 플레이어 레벨
        public int requiredSkillPoint; // 습득에 필요한 스킬 포인트

        public Projectile projectile; //원커리 스킬의 발사체
    }

    static Dictionary<string, SkillTemplate> skilldict = null;
    public static Dictionary<string, SkillTemplate> SkillDict
    {
        get
        {
            if(skilldict == null)
            {
                skilldict = Resources.LoadAll<SkillTemplate>("").ToDictionary(item => item.name, item => item);
            }
            return skilldict;
        }
    }
}
