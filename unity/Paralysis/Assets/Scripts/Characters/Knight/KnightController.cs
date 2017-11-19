using System;
using System.Collections;
using UnityEngine;

public class KnightController : ChampionClassController
{
    [SerializeField]
    private GameObject Skill4_Spear;
    [SerializeField]
    private float skill2_leap_speed = 10f;

    private bool skill2_leap = false;

    #region default Methods

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<KnightAnimationController>();

        basicAttack1_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.skillEffect.nothing, 0, stamina_BasicAttack1, true, cooldown_BasicAttack1, meeleRange);
        basicAttack2_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack2, delay_BasicAttack3, damage_BasicAttack3, Skill.skillEffect.nothing, 0, stamina_BasicAttack3, true, cooldown_BasicAttack2, meeleRange);

        skill1_var = new MeleeSkill(AnimationController.AnimatorStates.Skill1, delay_Skill1, damage_Skill1, Skill.skillEffect.stun, 3, stamina_Skill1, false, cooldown_Skill1, meeleRange);
        skill2_var = new MeleeSkill(AnimationController.AnimatorStates.Skill2, delay_Skill2, damage_Skill2, Skill.skillEffect.stun, 3, stamina_Skill2, false, cooldown_Skill2, meeleRange);
        skill3_var = new MeleeSkill(AnimationController.AnimatorStates.Skill3, delay_Skill3, damage_Skill3, Skill.skillEffect.knockback, 0, stamina_Skill3, false, cooldown_Skill3, meeleRange);
        skill4_var = new RangedSkill(AnimationController.AnimatorStates.Skill4, false, new Vector2(7, 0), Skill4_Spear, delay_Skill4, damage_Skill4, Skill.skillEffect.nothing, 0, stamina_Skill4, true, cooldown_Skill4, 5f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Add force to knight when leaping (Skill 2)
        if (skill2_leap)
        {
            m_Rigidbody2D.velocity = new Vector2(skill2_leap_speed, m_Rigidbody2D.velocity.y);
        }
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
                        case 1:
                        case 2:
                            // do meele attack
                            doMeeleSkill(ref animCon.trigBasicAttack1, (MeleeSkill)basicAttack1_var);
                            // Reset timer of combo
                            resetComboTime();
                            break;
                        case 3:
                            // do meele attack
                            doMeeleSkill(ref animCon.trigBasicAttack2, (MeleeSkill)basicAttack2_var);
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
        doMeeleSkill(ref animCon.trigSkill1, (MeleeSkill)skill1_var);
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
        // Validate if skill can be performed
        if (canPerformAction(true) && canPerformAttack() && skill2_var.notOnCooldown && stats.hasSufficientStamina(stamina_Skill2))
        {
            // set animation
            animCon.trigSkill2 = true;

            // start coroutine for hitting when reaching ground
            StartCoroutine(skill2_Coroutine());
        }
    }

    private IEnumerator skill2_Coroutine()
    {
        stats.immovable = true;
        skill2_leap = true;

        // Calculate correct direction
        if (!m_FacingRight)
            skill2_leap_speed = Math.Abs(skill2_leap_speed) * (-1);
        else
            skill2_leap_speed = Math.Abs(skill2_leap_speed);

        // Add a vertical force to the player.
        m_Rigidbody2D.velocity = Vector2.zero;
        m_Rigidbody2D.AddForce(new Vector2(0, 400));

        // Wait until force is applied
        yield return new WaitUntil(() => !animCon.m_Grounded);
        // Wait while not on ground
        yield return new WaitUntil(() => animCon.m_Grounded);
        Camera.main.GetComponent<CameraBehaviour>().startShake(); //Shake the camera
        skill2_leap = false;
        stats.immovable = false;

        doMeeleSkill(ref ((KnightAnimationController)animCon).trigSkill2End, (MeleeSkill)skill2_var, true);
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
        doMeeleSkill(ref animCon.trigSkill3, (MeleeSkill)skill3_var);
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
        doRangeSkill(ref animCon.trigSkill4, (RangedSkill)skill4_var);
    }

    #endregion

}