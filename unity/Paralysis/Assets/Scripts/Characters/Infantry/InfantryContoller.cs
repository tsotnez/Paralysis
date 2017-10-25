using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Characters.Infantry
{
    class InfantryContoller : ChampionClassController
    {
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
                                doMeeleSkill(ref animCon.trigBasicAttack1, delay_BasicAttack1, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1);
                                // Reset timer of combo
                                resetComboTime();
                                break;
                            case 2:
                            case 3:
                                // do meele attack
                                doMeeleSkill(ref animCon.trigBasicAttack2, delay_BasicAttack3, damage_BasicAttack3, skillEffect.nothing, 0, stamina_BasicAttack3);
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
        /// Groudn Break (Stun)
        /// The Infantry strikes the ground with his sword producing a force that can stun any enemys directly in front of him.
        /// 
        /// Damage: 5
        /// Effect: Stun
        /// Cooldown: 20 sec
        /// Stamina: 15
        /// </summary>
        public override void skill2()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bladestorm (Damage)
        /// Infatry swings his sword in all directions. Any enemy directly in front or behind the Infantry will take extensive damage
        ///  
        /// Damage: 15
        /// Effect: nothing
        /// Cooldown: 25 sec
        /// Stamina: 20
        /// </summary>
        public override void skill3()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
