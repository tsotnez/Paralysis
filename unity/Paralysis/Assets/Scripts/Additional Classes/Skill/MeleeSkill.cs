
public class MeleeSkill : Skill
{
    public MeleeSkill(AnimationController.AnimatorStates skillName, float skillDelay, int skillDamage, SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType, float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true) 
        : base(skillName ,skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillSpecialEffectValue, skillStaminaCost, skillTargetType, skillCooldown, skillRange, skillNeedsToBeGrounded)
    {

    }
}
