using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBase  {

    public string name;
    public bool learned;
    public int level;
    public float castTimeEnd;
    public float cooldownEnd;
    public float buffTimeEnd;

    public SkillBase(SkillTemplate template)
    {
        name = template.name;

        learned = template.learnDefault;
        level = 1;

        castTimeEnd = cooldownEnd = buffTimeEnd = Time.time;
    }

    public bool TemplateExists()
    {
        return SkillTemplate.SkillDict.ContainsKey(name);
    }

    public SKILL_TYPE skillType { get { return SkillTemplate.SkillDict[name].skillType; } }
    public SKILL_ATTACK_TYPE skillAttackType { get { return SkillTemplate.SkillDict[name].skillAttackType; } }
    public ENTITY_CLASS classType { get { return SkillTemplate.SkillDict[name].learnClassType; } }

    public Sprite skillImage { get { return SkillTemplate.SkillDict[name].skillImage; } }
    public bool learnDefault { get { return SkillTemplate.SkillDict[name].learnDefault; } }

    public GameObject effectCastSkill { get { return SkillTemplate.SkillDict[name].effectCastSkill; } }
    public GameObject effectSkillRange {  get { return SkillTemplate.SkillDict[name].effectSkillRange; } }
    public GameObject effectHitSkill { get { return SkillTemplate.SkillDict[name].effectHitSkill; } }

    public int manaCost { get { return SkillTemplate.SkillDict[name].levels[level - 1].manaCost; } }
    public float castTime { get { return SkillTemplate.SkillDict[name].levels[level - 1].castTime; } }
    public float castRange { get { return SkillTemplate.SkillDict[name].levels[level - 1].castRange; } }    
    public float cooldown { get { return SkillTemplate.SkillDict[name].levels[level - 1].cooldown; } }
    public float aoeRadius { get { return SkillTemplate.SkillDict[name].levels[level - 1].aoeRadius; } }

    public int damage { get { return SkillTemplate.SkillDict[name].levels[level - 1].damage; } }

    public float buffTime { get { return SkillTemplate.SkillDict[name].levels[level - 1].buffTime; } }
    public int buffDamage { get { return SkillTemplate.SkillDict[name].levels[level - 1].buffDamage; } }
    public int buffDefense { get { return SkillTemplate.SkillDict[name].levels[level - 1].buffDefense; } }
    public int buffHpMax { get { return SkillTemplate.SkillDict[name].levels[level - 1].buffHpMax; } }
    public int buffMpMax { get { return SkillTemplate.SkillDict[name].levels[level - 1].buffMpMax; } }
    public int buffHeal { get { return SkillTemplate.SkillDict[name].levels[level - 1].buffHeal; } }

    public int requiredLevel { get { return SkillTemplate.SkillDict[name].levels[level - 1].requiredLevel; } }
    public int requiredSkillPoint { get { return SkillTemplate.SkillDict[name].levels[level - 1].requiredSkillPoint; } }    
    public Projectile projectile { get { return SkillTemplate.SkillDict[name].levels[level - 1].projectile; } }

    public int maxLevel { get { return SkillTemplate.SkillDict[name].levels.Length; } }
    public int upgradeRequiredLevel { get { return (level < maxLevel) ? SkillTemplate.SkillDict[name].levels[level - 1].requiredLevel : 0; } }
    public int upgradeRequiredSkillPoint { get { return (level < maxLevel) ? SkillTemplate.SkillDict[name].levels[level - 1].requiredSkillPoint : 10; } }

    public string Tooltip(bool showRequirements = false)
    {
        string tip = SkillTemplate.SkillDict[name].tooltip;

        tip = tip.Replace("{NAME}", name);
        tip = tip.Replace("{LEVEL}", level.ToString());

        tip = tip.Replace("{SKILLTYPE}", skillType.ToString());
        tip = tip.Replace("{SKILLATTACKTYPE}", skillAttackType.ToString());
        tip = tip.Replace("{CLASSTYPE}", classType.ToString());

        tip = tip.Replace("{MANACOST}", manaCost.ToString());
        tip = tip.Replace("{CASTTIME}", castTime.ToString());
        tip = tip.Replace("{CASTRANGE}", castRange.ToString());
        tip = tip.Replace("{COOLDOWN}", cooldown.ToString());
        tip = tip.Replace("{AOERADIUS}", aoeRadius.ToString());

        tip = tip.Replace("{DAMAGE}", damage.ToString());

        tip = tip.Replace("{BUFFTIME}", buffTime.ToString());
        tip = tip.Replace("{BUFFDAMAGE}", buffDamage.ToString());
        tip = tip.Replace("{BUFFDEFENSE}", buffDefense.ToString());
        tip = tip.Replace("{BUFFHPMAX}", buffHpMax.ToString());
        tip = tip.Replace("{BUFFMPMAX}", buffMpMax.ToString());
        tip = tip.Replace("{BUFFHEAL}", buffHeal.ToString());

        //요구 레벨과 요구 스킬 포인트 표시
        if(showRequirements)
        {
            tip += "\n<b><i>Required Level : " + requiredLevel + "</i></b>\n" +
                "<b><i>Required SkillPoint : " + requiredSkillPoint + "</i></b>\n"; 
        }
        //이미 배운 스킬이고, 레벨업이 가능한 스킬
        if(learned && level < maxLevel)
        {
            tip += "\n<i>Skill LevelUp</i>\n" + "<i>Required Lecel : " + upgradeRequiredLevel + "</i>\n" +
                "<i>Required SkillPoint : " + upgradeRequiredSkillPoint + "</i>\n";
        }
        return tip;
    }

    public float CastTimeRemaining() { return castTimeEnd <= Time.time ? .0f : castTimeEnd - Time.time; }
    public bool IsCasting() { return CastTimeRemaining() > .0f; }
    public float CooldownRemaining() { return cooldownEnd <= Time.time ? .0f : cooldownEnd - Time.time; }
    public float BuffTimeRemaining() { return buffTimeEnd <= Time.time ? .0f : buffTimeEnd - Time.time; }
    public bool IsSkillReady() { return CooldownRemaining() == .0f; }
}
