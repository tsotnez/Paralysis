
public class MeleeSkill : Skill
{
    public MeleeSkill(AnimationController.AnimatorStates skillName, float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, skillTarget skillTargetType, float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true) 
        : base(skillName ,skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillStaminaCost, skillTargetType, skillCooldown, skillRange, skillNeedsToBeGrounded)
    {

    }
}
