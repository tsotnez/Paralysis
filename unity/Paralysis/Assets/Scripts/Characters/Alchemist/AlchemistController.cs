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

    public override void BasicAttack(bool shouldAttack)
    {
        throw new NotImplementedException();
    }

    public override void Skill1()
    {
        throw new NotImplementedException();
    }

    public override void Skill2()
    {
        throw new NotImplementedException();
    }

    public override void Skill3()
    {
        throw new NotImplementedException();
    }

    public override void Skill4()
    {
        throw new NotImplementedException();
    }

    #endregion

}