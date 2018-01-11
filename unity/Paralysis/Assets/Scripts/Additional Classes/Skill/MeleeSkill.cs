
public class MeleeSkill : Skill
{
    public MeleeSkill(ChampionAndTrinketDatabase.Keys skillName, float skillDelay, int skillDamage, SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType, float skillCooldown, float skillRange, ChampionAndTrinketDatabase.Champions skillChampion, bool skillNeedsToBeGrounded = true) 
        : base(skillName ,skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillSpecialEffectValue, skillStaminaCost, skillTargetType, skillCooldown, skillRange, skillChampion, skillNeedsToBeGrounded)
    {

    }
}
