using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class RangedSkill : Skill
{
    public bool onHitEffect = false;
    public GameObject prefab;
    public Vector2 speed = new Vector2(9,0);
    public float castTime = 0;

    public short rangedSkillId = 0;
    public static Dictionary<short, RangedSkill> rangedSkillDict = new Dictionary<short, RangedSkill>();

    public RangedSkill(Skill.SkillType skillName, bool skillOnHitEffect, Vector2 projectileSpeed, GameObject projectilePrefab, float skillDelay, int skillDamage, SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType, float skillCooldown, float skillRange, string name, bool skillNeedsToBeGrounded = true, float castTime = 0f) 
        : base(skillName, skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, skillSpecialEffectValue, skillStaminaCost, skillTargetType, skillCooldown, skillRange, name, skillNeedsToBeGrounded)
    {
        onHitEffect = skillOnHitEffect;
        prefab = projectilePrefab;
        speed = projectileSpeed;
        this.castTime = castTime;
    }

    public void addSkillToDB()
    {
        //set an id of the ranged skill to be used for networking
        int rangedSkillHash = skillName.GetHashCode();
        if (prefab != null) rangedSkillHash ^= prefab.name.GetHashCode();
        if (speed.y != 0) rangedSkillHash ^= speed.y.GetHashCode();
        if (speed.x != 0) rangedSkillHash ^= speed.x.GetHashCode();
        if (range != 0) rangedSkillHash ^= range.GetHashCode();

        rangedSkillId = CRCCalculator.CRCFromInt(rangedSkillHash);
        if(!rangedSkillDict.ContainsKey(rangedSkillId))
        {
            Debug.Log("Adding ranged skill: " + skillName + " hash:" + rangedSkillId);
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
