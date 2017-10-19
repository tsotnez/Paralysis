using System;
using System.Collections;
using UnityEngine;

public class KnightController : ChampionClassController
{
    [Header("Attack Delays")]
    [SerializeField]
    private float delay_BasicAttack = 0;
    [SerializeField]
    private float delay_BasicAttackCombo = 0;
    [SerializeField]
    private float delay_Skill1 = 0;
    [SerializeField]
    private float delay_Skill2 = 0;
    [SerializeField]
    private float delay_Skill3 = 0;
    [SerializeField]
    private float delay_Skill4 = 0;

    [Header("Attack Stamina Costs")]
    [SerializeField]
    private int stamina_BasicAttack = 5;
    [SerializeField]
    private int stamina_BasicAttackCombo = 7;
    [SerializeField]
    private int stamina_JumpAttack = 5;
    [SerializeField]
    private int stamina_Skill1_GroundSmash = 20;
    [SerializeField]
    private int stamina_Skill2_Leap = 15;
    [SerializeField]
    private int stamina_Skill3_ShieldBash = 30;
    [SerializeField]
    private int stamina_Skill4_Spear = 20;

    [Header("Attack Damage")]
    [SerializeField]
    private int damage_BasicAttack = 5;
    [SerializeField]
    private int damage_BasicAttackCombo = 10;
    [SerializeField]
    private int damage_Skill1_GroundSmash = 5;
    [SerializeField]
    private int damage_Skill2_Leap = 5;
    [SerializeField]
    private int damage_Skill3_ShieldBash = 10;
    [SerializeField]
    private int damage_Skill4_Spear = 15;

    // Trigger for character specific animations
    private bool dashingForward = false;

    #region default Methods

    // Use this for initialization
    void Start()
    {

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
        if (shouldAttack && !attacking)
        {
            if (m_Grounded)
            {
                // Check if enough stamina for attack
                if (stats.hasSufficientStamina(stamina_BasicAttack) && (attackCount == 0 || attackCount == 1) || //Basic Attack
                    stats.hasSufficientStamina(stamina_BasicAttackCombo) && (attackCount == 2)) // Strong Attack
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
                            doMeeleSkill(0, ref trigBasicAttack1, delay_BasicAttack, damage_BasicAttack, skillEffect.nothing, 0, stamina_BasicAttack);
                            // Reset timer of combo
                            resetComboTime();
                            break;
                        case 3:
                            // do meele attack
                            doMeeleSkill(2, ref trigBasicAttack2, delay_BasicAttackCombo, damage_BasicAttackCombo, skillEffect.nothing, 0, stamina_BasicAttackCombo);
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
                if (stats.hasSufficientStamina(stamina_JumpAttack))
                {
                    // Lose Stamina
                    stats.loseStamina(stamina_JumpAttack);
                    // Jump Attack
                    StartCoroutine(jumpAttack());
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[3] - 0.08f));
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
        doMeeleSkill(4, ref trigSkill1, delay_Skill1, damage_Skill1_GroundSmash, skillEffect.stun, 3, stamina_Skill1_GroundSmash);
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
        doMeeleSkill(5, ref trigSkill2, delay_Skill2, damage_Skill2_Leap, skillEffect.stun, 3, stamina_Skill2_Leap);
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
        doMeeleSkill(6, ref trigSkill3, delay_Skill3, damage_Skill3_ShieldBash, skillEffect.knockback, 0, stamina_Skill3_ShieldBash, false);
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
        //range skill
        //animation: skill4_Spear
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
        if (m_Grounded && !attacking && !dashing && stats.hasSufficientStamina(m_dashStaminaCost))
        {
            if (direction != 0 && !dashing)
            {
                //lose stamina
                stats.loseStamina(m_dashStaminaCost);

                //Calculate new dashForce to go in right direction
                m_dashSpeed = Mathf.Abs(m_dashSpeed) * direction;
                m_Rigidbody2D.velocity = Vector2.zero;

                // set var for dash or dashForward
                if (direction < 0 && !m_FacingRight || direction > 0 && m_FacingRight) dashing = true;
                else dashingForward = true;

                dontMove = true;

                yield return new WaitForSeconds(0.1f);
                stats.invincible = true; //Player is invincible for a period of time while dashing

                yield return new WaitForSeconds(0.3f);
                dashing = false;
                stats.invincible = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); //Stop moving

                yield return new WaitForSeconds(0.04f); //Short time where character cant move after dashing
                dontMove = false;
            }
        }
    }

    #endregion

    #region Character specific animation

    protected override bool additionalAnimationCondition(AnimationController animCon)
    {
        if (blocking && m_Speed > 0.001)
            animCon.StartAnimation(AnimationController.AnimatorStates.BlockMove);            
        else if (dashingForward)
            animCon.StartAnimation(AnimationController.AnimatorStates.DashFor);
        else
            return false;

        return true;
    }

    protected override bool additionalNotInterruptCondition(AnimationController.AnimatorStates activeAnimation)
    {
        if (activeAnimation == AnimationController.AnimatorStates.DashFor)
                return true;

        return false;
    }

    #endregion
}