using UnityEngine;
using System.Collections.Generic;
using System;

public class RangedSkill : Skill
{
    public bool onHitEffect;
    public GameObject prefab;
    public Vector2 speed;
    public float castTime;

    public int rangedSkillId = 0;
    public static Dictionary<int, RangedSkill> rangedSkillDict = new Dictionary<int, RangedSkill>();

    public RangedSkill(ChampionAndTrinketDatabase.Keys skillName, bool skillOnHitEffect, Vector2 projectileSpeed, GameObject projectilePrefab, float skillDelay, int skillDamage, SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType, float skillCooldown, float skillRange, ChampionAndTrinketDatabase.Champions skillChampion, bool skillNeedsToBeGrounded = true, float castTime = 0f) 
        : base(skillName, skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillSpecialEffectValue, skillStaminaCost, skillTargetType, skillCooldown, skillRange, skillChampion, skillNeedsToBeGrounded)
    {
        onHitEffect = skillOnHitEffect;
        prefab = projectilePrefab;
        speed = projectileSpeed;
        this.castTime = castTime;

        //set an id of the ranged skill to be used for networking
        rangedSkillId = projectilePrefab.name.GetHashCode();
        rangedSkillId = (rangedSkillId * 397) ^ skillDamage.GetHashCode();
        rangedSkillId = (rangedSkillId * 397) ^ (int)castTime.GetHashCode();
        rangedSkillId = (rangedSkillId * 397) ^ (int)speed.x.GetHashCode();

        if(!rangedSkillDict.ContainsKey(rangedSkillId))
        {
            rangedSkillDict.Add(rangedSkillId, this);
        }
    }

    //Clears ranged skills and ids
    public static void clearRangedSkillDict()
    {
        rangedSkillDict.Clear();
    }
}
