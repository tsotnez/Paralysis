using UnityEngine;
using System.Collections.Generic;
using System;

public class RangedSkill : Skill
{
    public bool onHitEffect;
    public GameObject prefab;
    public Vector2 speed;
    public float castTime;

    public short rangedSkillId = 0;
    public static Dictionary<short, RangedSkill> rangedSkillDict = new Dictionary<short, RangedSkill>();

    public RangedSkill(ChampionAndTrinketDatabase.Keys skillName, bool skillOnHitEffect, Vector2 projectileSpeed, GameObject projectilePrefab, float skillDelay, int skillDamage, SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType, float skillCooldown, float skillRange, ChampionAndTrinketDatabase.Champions skillChampion, bool skillNeedsToBeGrounded = true, float castTime = 0f) 
        : base(skillName, skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillSpecialEffectValue, skillStaminaCost, skillTargetType, skillCooldown, skillRange, skillChampion, skillNeedsToBeGrounded)
    {
        onHitEffect = skillOnHitEffect;
        prefab = projectilePrefab;
        speed = projectileSpeed;
        this.castTime = castTime;

        //set an id of the ranged skill to be used for networking
        int rangedSkillHash = projectilePrefab.name.GetHashCode();
        rangedSkillHash = rangedSkillHash ^ skillDamage.GetHashCode();
        rangedSkillHash = rangedSkillHash ^ projectileSpeed.x.GetHashCode();
        rangedSkillHash = rangedSkillHash ^ skillSpecialEffectTime.GetHashCode();

        rangedSkillId = CRCCalculator.CRCFromInt(rangedSkillHash);

        if(!rangedSkillDict.ContainsKey(rangedSkillId))
        {
            rangedSkillDict.Add(rangedSkillId, this);
        }
        else
        {
            //This is fine as long as there are 2 of the same skills on 2 different characters.
            Debug.Log("Duplicate ranged skill hash: " + skillName);
        }
    }

    //Clears ranged skills and ids
    public static void clearRangedSkillDict()
    {
        rangedSkillDict.Clear();
    }
}
