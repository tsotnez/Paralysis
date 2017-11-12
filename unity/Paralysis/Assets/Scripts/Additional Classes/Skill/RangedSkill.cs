﻿using UnityEngine;

public class RangedSkill : Skill
{
    public bool onHitEffect;
    public GameObject prefab;
    public Vector2 speed;

    public RangedSkill(AnimationController.AnimatorStates skillName, bool skillOnHitEffect, Vector2 projectileSpeed, GameObject projectilePrefab, float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, bool skillSingleTarget, float skillCooldown, float skillRange, bool skillNeedsToBeGrounded = true) 
        : base(skillName, skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillStaminaCost, skillSingleTarget, skillCooldown, skillRange, skillNeedsToBeGrounded)
    {
        onHitEffect = skillOnHitEffect;
        prefab = projectilePrefab;
        speed = projectileSpeed;
    }
}
