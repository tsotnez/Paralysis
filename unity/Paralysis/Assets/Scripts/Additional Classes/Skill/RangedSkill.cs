using UnityEngine;

public class RangedSkill : Skill
{
    public bool onHitEffect;
    public GameObject prefab;
    public Vector2 speed;
    public float castTime;

    public RangedSkill(AnimationController.AnimatorStates skillName, bool skillOnHitEffect, Vector2 projectileSpeed, GameObject projectilePrefab, float skillDelay, int skillDamage, SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType, float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true, float castTime = 0f) 
        : base(skillName, skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillSpecialEffectValue, skillStaminaCost, skillTargetType, skillCooldown, skillRange, skillNeedsToBeGrounded)
    {
        onHitEffect = skillOnHitEffect;
        prefab = projectilePrefab;
        speed = projectileSpeed;
        this.castTime = castTime;
    }
}
