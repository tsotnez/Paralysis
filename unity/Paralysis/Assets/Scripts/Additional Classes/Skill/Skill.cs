/// <summary>
/// Describes a skill
/// </summary>
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

    public AnimationController.AnimatorStates name;
    public SkillEffect effect;
    public int effectDuration;
    public float effectValue;
    public int damage;
    public int staminaCost;
    public float cooldown;
    public SkillTarget targetType;
    public float range;
    public bool needsToBeGrounded;
    public float delay;

    public bool notOnCooldown = true;


    public Skill(AnimationController.AnimatorStates skillName, float skillDelay, int skillDamage, Skill.SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType,
        float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true)
    {
        name = skillName;
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
    }

    public Skill(AnimationController.AnimatorStates skillName, float skillCooldown)
    {
        cooldown = skillCooldown;
        name = skillName;
    }
}