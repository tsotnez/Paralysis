using System;

public class InfantryContoller : ChampionClassController
{
    #region default

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<InfantryAnimationController>();

        basicAttack1_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.skillEffect.nothing, 0, stamina_BasicAttack1, Skill.skillTarget.SingleTarget, cooldown_BasicAttack1, meeleRange);
        basicAttack2_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack2, delay_BasicAttack2, damage_BasicAttack2, Skill.skillEffect.nothing, 0, stamina_BasicAttack2, Skill.skillTarget.SingleTarget, cooldown_BasicAttack2, meeleRange);

        skill1_var = new Skill      (AnimationController.AnimatorStates.Skill1, delay_Skill1, damage_Skill1, Skill.skillEffect.stun, 3, stamina_Skill1, Skill.skillTarget.SingleTarget, cooldown_Skill1, meeleRange);
        skill2_var = new MeleeSkill (AnimationController.AnimatorStates.Skill2, delay_Skill2, damage_Skill2, Skill.skillEffect.stun, 3, stamina_Skill2, Skill.skillTarget.InFront, cooldown_Skill2, meeleRange);
        skill3_var = new MeleeSkill (AnimationController.AnimatorStates.Skill3, delay_Skill3, damage_Skill3, Skill.skillEffect.nothing, 0, stamina_Skill3, Skill.skillTarget.MultiTarget, cooldown_Skill3, meeleRange);
        skill4_var = new MeleeSkill (AnimationController.AnimatorStates.Skill4, delay_Skill4, damage_Skill4, Skill.skillEffect.knockback, 0, stamina_Skill4, Skill.skillTarget.InFront, cooldown_Skill4, meeleRange);
    }

    #endregion

    #region BasicAttack

    /// <summary>
    /// Attack combo. 1 normal hit, 2 strong hits
    /// 
    /// normal hit: 5 dmg
    /// strong hit: 7 dmg
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void basicAttack(bool shouldAttack)
    {
        if (shouldAttack && canPerformAction(false) && canPerformAttack())
        {
            if (animCon.m_Grounded)
            {
                // Check if enough stamina for attack
                if (stats.hasSufficientStamina(stamina_BasicAttack1) && (attackCount == 0) || //Basic Attack
                    stats.hasSufficientStamina(stamina_BasicAttack2) && (attackCount == 1 || attackCount == 2)) // Strong Attack
                {
                    // Already in combo?
                    if (!inCombo)
                    {
                        // First attack - initialize combo coroutine
                        resetComboTime();
                        attackCount = 0;
                    }

                    // AttackCount increase per attack
                    attackCount++;

                    // Playing the correct animation depending on the attackCount and setting attacking status
                    switch (attackCount)
                    {
                        case 1:
                            // do meele attack
                            doMeleeSkill(ref animCon.trigBasicAttack1, (MeleeSkill)basicAttack1_var);
                            // Reset timer of combo
                            resetComboTime();
                            break;
                        case 2:
                            // do meele attack
                            doMeleeSkill(ref animCon.trigBasicAttack2, (MeleeSkill)basicAttack2_var);
                            break;
                        case 3:
                            // do meele attack
                            doMeleeSkill(ref animCon.trigBasicAttack2, (MeleeSkill)basicAttack2_var);
                            // Reset Combo after combo-hit
                            abortCombo();
                            break;
                        default:
                            // Should not be triggered
                            abortCombo();
                            break;

                    }
                }
            }
            // Jump attack only when falling
            else
            {
                // Check if enough stamina is left
                if (stats.loseStamina(stamina_JumpAttack))
                {
                    // Jump Attack
                    StartCoroutine(jumpAttack());
                    // Abort combo
                    abortCombo();
                }
            }
        }
    }

    #endregion

    #region Skills

    /// <summary>
    /// Retract Hook (Knockback)
    /// Infantry launches a hook to a flexibly fixed distance and altitude the hook latches on to the first enemy it comes in contact with. 
    /// The Infantry then repels to that target and kicks, causing a knockback effect to the enemy
    /// 
    /// Damage: 5
    /// Effect: Knockback
    /// Cooldown: 15 sec
    /// Stamina: 10
    /// </summary>
    public override void skill1()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Ground Break (Stun)
    /// The Infantry strikes the ground with his sword producing a force that can stun any enemys directly in front of him.
    /// 
    /// Damage: 5
    /// Effect: Stun
    /// Cooldown: 20 sec
    /// Stamina: 15
    /// </summary>
    public override void skill2()
    {
        doMeleeSkill(ref animCon.trigSkill2, (MeleeSkill)skill2_var);
    }

    /// <summary>
    /// Bladestorm (Damage)
    /// Infantry swings his sword in all directions. Any enemy directly in front or behind the Infantry will take extensive damage
    ///  
    /// Damage: 15
    /// Effect: nothing
    /// Cooldown: 25 sec
    /// Stamina: 20
    /// </summary>
    public override void skill3()
    {
        doMeleeSkill(ref animCon.trigSkill3, (MeleeSkill)skill3_var);
    }

    /// <summary>
    /// Hit/Throw (Knockback)
    /// Infantry swings his sword upwards, along with any enemy directly in front of him. This causes a knockback to the enemy.
    /// 
    /// Damage: 10
    /// Effect: Knockback
    /// Cooldown: 20 sec
    /// Stamina: 15
    /// </summary>
    public override void skill4()
    {
        doMeleeSkill(ref animCon.trigSkill4, (MeleeSkill)skill4_var);
    }

    #endregion
}