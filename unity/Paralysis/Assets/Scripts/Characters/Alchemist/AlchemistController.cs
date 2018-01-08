using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AlchemistController : ChampionClassController
{
    public GameObject GoBasicAttack;
    public GameObject GoSkill1_Frostbolt;
    public GameObject GoSkill3_Stun;

    public float CastTime_Skill1 = 1.5f;
    public float CastTime_Skill3 = 1.5f;

    #region default

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<AlchemistAnimationController>();

        //Instantiate skill variables
        basicAttack1_var = new RangedSkill(AnimationController.AnimatorStates.BasicAttack1, true, new Vector2(9, 0), GoBasicAttack, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, 7f);

        skill1_var = new RangedSkill(AnimationController.AnimatorStates.Skill1, true, new Vector2(9, 0), GoSkill1_Frostbolt, delay_Skill1, damage_Skill1, Skill.SkillEffect.slow, 3, 0.5f, stamina_Skill1, Skill.SkillTarget.SingleTarget, cooldown_Skill1, 7f, true, CastTime_Skill1);
        skill2_var = new Skill(AnimationController.AnimatorStates.Skill2, cooldown_Skill2);
        skill3_var = new RangedSkill(AnimationController.AnimatorStates.Skill3, true, new Vector2(9, 0), GoSkill3_Stun, delay_Skill3, damage_Skill3, Skill.SkillEffect.stun, 3, 0, stamina_Skill3, Skill.SkillTarget.SingleTarget, cooldown_Skill3, 7f, true, CastTime_Skill3);
        skill4_var = new MeleeSkill(AnimationController.AnimatorStates.Skill4, delay_Skill4, damage_Skill4, Skill.SkillEffect.knockback, 0, 0, stamina_Skill4, Skill.SkillTarget.MultiTarget, cooldown_Skill4, 3f);
    }

    #endregion

    #region BasicAttack and Skills

    /// <summary>
    /// Only Basic Attack
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void BasicAttack(bool shouldAttack)
    {
        if (shouldAttack)
        {
            if (animCon.m_Grounded) // Basic Attack
                DoRangeSkill(ref animCon.trigBasicAttack1, (RangedSkill)basicAttack1_var);
            else if (doubleJumped) // Jump Attack
                StartCoroutine(JumpAttack());
        }
    }

    /// <summary>
    /// Alchemist casts a frost bolt at a fixed distance and altitute.
    /// The first enemy hit by this effect will have a 50% decreased movement speed.
    /// 
    /// Damage: 5
    /// Effect: slow
    /// Cast time: 1.5s
    /// Stamina: 15
    /// </summary>
    public override void Skill1()
    {
        DoRangeSkill(ref animCon.trigSkill1, (RangedSkill)skill1_var);
    }

    /// <summary>
    /// Alchemist teleports in the direction that he is moving or dashing.
    /// If standing still, teleports in the direction he is facing.
    /// If jumping, teleports on top platform.
    /// If ducking, teleports on lower platform.
    /// 
    /// Cooldown: 15s
    /// Stamina: 15
    /// </summary>
    public override void Skill2()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Alchemist casts a spell at a fixed distance and altitude.
    /// The first enemy stuck by this will be stunned.
    /// 
    /// Damage: 5
    /// Effect: stun
    /// Cooldown: 15s
    /// Stamina: 15
    /// </summary>
    public override void Skill3()
    {
        DoRangeSkill(ref animCon.trigSkill3, (RangedSkill)skill3_var);
    }

    /// <summary>
    /// Alchemist uses his alchemy to explode and reconstruct himnself.
    /// As he explodes, any enemy within the radius will receive a knockback.
    /// 
    /// Damage: 10
    /// Effect: knockback
    /// Cooldown: 25s
    /// Stamina: 20
    /// </summary>
    public override void Skill4()
    {
        DoMeleeSkill(ref animCon.trigSkill4, (MeleeSkill)skill4_var);
    }

    #endregion

}