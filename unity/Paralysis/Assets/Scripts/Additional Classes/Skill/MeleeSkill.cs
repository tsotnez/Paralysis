
public class MeleeSkill : Skill
{
    public MeleeSkill(AnimationController.AnimatorStates skillName, float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, bool skillSingleTarget, float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true) 
        : base(skillName ,skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillStaminaCost, skillSingleTarget, skillCooldown, skillRange, skillNeedsToBeGrounded)
    {

    }
}
