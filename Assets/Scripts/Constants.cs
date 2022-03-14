//상태(행동) 타입 정의
public enum ENTITY_STATE
    { IDLE=0, MOVE, ATTACK, HIT, CALCSKILLRANGE, CASTING, DIE,}

//직업 타입 정의
public enum ENTITY_CLASS
    { NOOB=0, MAGE, WARRIOR, ARCHER,}

//스킬 타입 정의
public enum SKILL_TYPE
{
    ATTACK = 0, BUFF, HEAL,
}

//공격 타입 정의
public enum SKILL_ATTACK_TYPE
{
    NOT_FIRE = 0, SINGLE, MULTISPLASH, MULTIRANGE, MULTICHAIN,
}

