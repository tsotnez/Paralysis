public class MeleeSkill : Skill
{
    public MeleeSkill(float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, bool skillSingleTarget, float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true)
        : base(skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillStaminaCost, skillSingleTarget, skillCooldown, skillRange, skillNeedsToBeGrounded)
    {

    }
}