/// <summary>
/// Describes a skill
/// </summary>
[System.Serializable]
public class Skill
{
    public enum SkillEffect
    {
        nothing,
        stun,
        knockback,
        bleed,
        slow
    }

    public enum SkillTarget
    {
        SingleTarget, MultiTarget, InFront
    }

    public enum SkillType
    {
        BasicAttack1, BasicAttack2, BasicAttack3, JumpAttack, Skill1, Skill2, Skill3, Skill4
    }

    public SkillType type;
    public string skillName = "NotSet";
    public SkillEffect effect = SkillEffect.nothing;
    public int effectDuration = 0;
    public float effectValue = 0;
    public int damage = 0;
    public int staminaCost = 0;
    public float cooldown = 0;
    public SkillTarget targetType = SkillTarget.SingleTarget;
    public float range = GameConstants.MEELEATTACKRANGE;
    public bool needsToBeGrounded = true;
    public float delay = 0;
    public bool notOnCooldown = true;
    [UnityEngine.Multiline(4)]
    public string skillDescription = "";


    public Skill(SkillType skillType, float skillDelay, int skillDamage, Skill.SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType,
        float skillCooldown, float skillRange, string name, bool skillNeedsToBeGrounded = true)
    {
        type = skillType;
        delay = skillDelay;
        damage = skillDamage;
        effect = skillSpecialEffect;
        effectDuration = skillSpecialEffectTime;
        effectValue = skillSpecialEffectValue;
        cooldown = skillCooldown;
        staminaCost = skillStaminaCost;
        needsToBeGrounded = skillNeedsToBeGrounded;
        range = skillRange;
        targetType = skillTargetType;
        skillName = name;
    }

    public Skill(SkillType skillType, string name, float skillCooldown)
    {
        skillName = name;
        cooldown = skillCooldown;
        type = skillType;
    }
}