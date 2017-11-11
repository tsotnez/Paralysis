/// <summary>
/// Describes a skill
/// </summary>

public abstract class Skill
{ 
    public enum skillEffect
    {
        nothing,
        stun,
        knockback,
        bleed
    }

    public skillEffect effect;
    public int effectDuration;
    public int damage;
    public int staminaCost;
    public float cooldown;
    public bool singleTarget;
    public float range;
    public bool needsToBeGrounded;
    public float delay;

    public bool notOnCooldown = true;


    public Skill(float skillDelay, int skillDamage, Skill.skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, bool skillSingleTarget,
        float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true)
    {
        delay = skillDelay;
        damage = skillDamage;
        effect = skillSpecialEffect;
        effectDuration = skillSpecialEffectTime;
        cooldown = skillCooldown;
        staminaCost = skillStaminaCost;
        needsToBeGrounded = skillNeedsToBeGrounded;
        range = skillRange;
        singleTarget = skillSingleTarget;
    }
}