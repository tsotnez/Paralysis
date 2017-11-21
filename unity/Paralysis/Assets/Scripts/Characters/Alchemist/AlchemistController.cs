using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AlchemistController : ChampionClassController
{
    #region default

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<AlchemistAnimationController>();

        //basicAttack1_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, meeleRange);

        //skill1_var = new MeleeSkill(AnimationController.AnimatorStates.Skill1, delay_Skill1, damage_Skill1, Skill.SkillEffect.stun, 3, stamina_Skill1, Skill.SkillTarget.MultiTarget, cooldown_Skill1, meeleRange);
        //skill2_var = new MeleeSkill(AnimationController.AnimatorStates.Skill2, delay_Skill2, damage_Skill2, Skill.SkillEffect.stun, 3, stamina_Skill2, Skill.SkillTarget.MultiTarget, cooldown_Skill2, meeleRange);
        //skill3_var = new MeleeSkill(AnimationController.AnimatorStates.Skill3, delay_Skill3, damage_Skill3, Skill.SkillEffect.knockback, 0, stamina_Skill3, Skill.SkillTarget.SingleTarget, cooldown_Skill3, meeleRange);
        //skill4_var = new RangedSkill(AnimationController.AnimatorStates.Skill4, false, new Vector2(7, 0), Skill4_Spear, delay_Skill4, damage_Skill4, Skill.SkillEffect.nothing, 0, stamina_Skill4, Skill.SkillTarget.SingleTarget, cooldown_Skill4, 5f);
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
            throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    #endregion

}