/// <summary>
/// Describes a skill
/// </summary>
public class Skill
{

    public enum skillEffect
    {
        nothing,
        stun,
        knockback,
        bleed
    }

    public enum skillTarget
    {
        SingleTarget, MultiTarget, InFront
    }

    public AnimationController.AnimatorStates name;
    public skillEffect effect;
    public int effectDuration;
    public int damage;
    public int staminaCost;
    public float cooldown;
    public skillTarget targetType;
    public float range;
    public bool needsToBeGrounded;
    public float delay;

    public bool notOnCooldown = true;


    public Skill(AnimationController.AnimatorStates skillName, float skillDelay, int skillDamage, Skill.skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, skillTarget skillTargetType,
        float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true)
    {
        name = skillName;
        delay = skillDelay;
        damage = skillDamage;
        effect = skillSpecialEffect;
        effectDuration = skillSpecialEffectTime;
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