using System;
using System.Collections;
using UnityEngine;

public class KnightController : ChampionClassController
{
    [Header("Knight Special")]
    [SerializeField]
    private GameObject Skill4_Spear;

    #region default Methods

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<KnightAnimationController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    #endregion

    #region BasicAttack

    /// <summary>
    /// Attack combo. 2 normal hits, 1 strong hit
    /// 
    /// normal hit:  5 dmg
    /// strong hit: 10 dmg
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void basicAttack(bool shouldAttack)
    {
        if (shouldAttack && canPerformAction(false) && canPerformAttack())
        {
            if (animCon.m_Grounded)
            {
                // Check if enough stamina for attack
                if (stats.hasSufficientStamina(stamina_BasicAttack1) && (attackCount == 0 || attackCount == 1) || //Basic Attack
                    stats.hasSufficientStamina(stamina_BasicAttack3) && (attackCount == 2)) // Strong Attack
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
                        case 1: case 2:
                            // do meele attack
                            doMeeleSkill(ref animCon.trigBasicAttack1, delay_BasicAttack1, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1);
                            // Reset timer of combo
                            resetComboTime();
                            break;
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

    #region Skills

    /// <summary>
    /// Ground Smash (Stun)
    /// The knight smashes his shield into the ground.
    /// Stunning any enemy in a short radius
    /// 
    /// Damage: 5
    /// Effect: Stun
    /// Cooldown: 15 sec
    /// Stamina: 20
    /// </summary>
    public override void skill1()
    {
        doMeeleSkill(ref animCon.trigSkill1, delay_Skill1, damage_Skill1, skillEffect.stun, 3, stamina_Skill1);
    }


    /// <summary>
    /// Leap (Stun)
    /// The knight leaps at a fixed altitude of the map.
    /// Once he lands he stuns any enemies in a short radius.
    /// 
    /// Damage: 5
    /// Effect: Stun
    /// Cooldown: 20 sec
    /// Stamina: 15
    /// </summary>
    public override void skill2()
    {
        doMeeleSkill(ref animCon.trigSkill2, delay_Skill2, damage_Skill2, skillEffect.stun, 3, stamina_Skill2);
    }

    /// <summary>
    /// Shield Bash (Knockback)
    /// The knight takes a few swift steps to deliver a strike with his shield
    /// Any enemy struck by it will be put in a knockback animation.
    /// 
    /// Damage: 10
    /// Effect: Knockback
    /// Cooldown: 20 sec
    /// Stamina: 30
    /// </summary>
    public override void skill3()
    {
        doMeeleSkill(ref animCon.trigSkill3, delay_Skill3, damage_Skill3, skillEffect.knockback, 0, stamina_Skill3, false);
    }

    /// <summary>
    /// Spear (Damage)
    /// The knight throws a spear at a fixed distance and a fixed altitude.
    /// Any enemy struck by it will receive damage.
    /// 
    /// Damage: 15
    /// Cooldown: 25 sec
    /// Stamina: 20
    /// </summary>
    public override void skill4()
    {
        doRangeSkill(ref animCon.trigSkill4, delay_Skill4, Skill4_Spear, 5f, damage_Skill4, skillEffect.nothing, 0, stamina_Skill4);
    }

    #endregion

    #region Dash

    /// <summary>
    /// Dashes in the given direction with Dash and Dash Forward
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public override IEnumerator dash(int direction)
    {
        if (canPerformAction(true) && canPerformAttack() && stats.loseStamina(m_dashStaminaCost))
        {
            if (direction != 0 && !dashing)
            {
                //Calculate new dashForce to go in right direction
                m_dashSpeed = Mathf.Abs(m_dashSpeed) * direction;
                m_Rigidbody2D.velocity = Vector2.zero;

                // set var for dash or dashForward
                if (direction < 0 && !m_FacingRight || direction > 0 && m_FacingRight) animCon.trigDash = true;
                else ((KnightAnimationController)animCon).trigDashForward = true;

                dashing = true;
                stats.immovable = true;

                yield return new WaitForSeconds(0.1f);
                stats.invincible = true; //Player is invincible for a period of time while dashing

                yield return new WaitForSeconds(0.3f);
                dashing = false;
                stats.invincible = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); //Stop moving

                yield return new WaitForSeconds(0.04f); //Short time where character cant move after dashing
                stats.immovable = false;
            }
        }
    }

    #endregion
}