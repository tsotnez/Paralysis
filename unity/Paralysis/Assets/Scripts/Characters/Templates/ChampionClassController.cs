﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChampionClassController : MonoBehaviour
{
    #region Parameters for Inspector

    [SerializeField]
    protected LayerMask m_WhatIsGround;                                     // A mask determining what is ground to the character
    [SerializeField]
    protected LayerMask m_whatToHit;                                        // What to hit when checking for hits while attacking

    [SerializeField]
    protected float m_MaxSpeed = 10f;                                       // The fastest the player can travel in the x axis.
    [SerializeField]
    protected float m_MoveSpeedWhileAttacking = 5f;                         // Max speed while attacking
    [SerializeField]
    protected float m_MoveSpeedWhileBlocking = 0f;                          // Max speed while blocking

    [SerializeField]
    protected float m_JumpForce = 400f;                                     // Amount of force added when the player jumps.   
    [SerializeField]
    protected float m_jumpAttackRadius = 10f;                               // Radius of jump Attack damage
    [SerializeField]
    protected float m_jumpAttackForce = 10f;                                // Amount of force added when the player jump attack

    [SerializeField]
    protected float m_dashSpeed = 7f;                                       // Force applied when dashing
    [SerializeField]
    protected int m_dashStaminaCost = 10;

    [SerializeField]
    protected const float meeleRange = 1.5f;                                // Default range for meele attacks

    protected int attackCount = 0;                                          // The ComboState 0 means the character has not attacked yet
    protected bool inCombo = false;                                         // When true, the next comboStage can be reached
    [SerializeField]
    protected float m_ComboExpire = 1;                                      // How long the next combostage is reachable (seconds)
    protected bool jumpAttacking = false;                                   // True while the character is jump attacking

    [SerializeField]
    protected float delay_BasicAttack1 = 0;
    [SerializeField]
    protected float delay_BasicAttack2 = 0;
    [SerializeField]
    protected float delay_BasicAttack3 = 0;
    [SerializeField]
    protected float delay_Skill1 = 0;
    [SerializeField]
    protected float delay_Skill2 = 0;
    [SerializeField]
    protected float delay_Skill3 = 0;
    [SerializeField]
    protected float delay_Skill4 = 0;

    [SerializeField]
    protected int stamina_BasicAttack1 = 5;
    [SerializeField]
    protected int stamina_BasicAttack2 = 5;
    [SerializeField]
    protected int stamina_BasicAttack3 = 5;
    [SerializeField]
    protected int stamina_JumpAttack = 5;
    [SerializeField]
    protected int stamina_Skill1 = 0;
    [SerializeField]
    protected int stamina_Skill2 = 0;
    [SerializeField]
    protected int stamina_Skill3 = 0;
    [SerializeField]
    protected int stamina_Skill4 = 0;

    [SerializeField]
    protected int damage_BasicAttack1 = 0;
    [SerializeField]
    protected int damage_BasicAttack2 = 0;
    [SerializeField]
    protected int damage_BasicAttack3 = 0;
    [SerializeField]
    protected int damage_JumpAttack = 5;
    [SerializeField]
    protected int damage_Skill1 = 0;
    [SerializeField]
    protected int damage_Skill2 = 0;
    [SerializeField]
    protected int damage_Skill3 = 0;
    [SerializeField]
    protected int damage_Skill4 = 0;

    [SerializeField]
    protected int cooldown_BasicAttack1 = 0;
    [SerializeField]
    protected int cooldown_BasicAttack2 = 0;
    [SerializeField]
    protected int cooldown_BasicAttack3 = 0;
    [SerializeField]
    protected int cooldown_JumpAttack = 0;
    [SerializeField]
    protected int cooldown_Skill1 = 0;
    [SerializeField]
    protected int cooldown_Skill2 = 0;
    [SerializeField]
    protected int cooldown_Skill3 = 0;
    [SerializeField]
    protected int cooldown_Skill4 = 0;

    #endregion

    protected Transform m_GroundCheck;                                      // A position marking where to check if the player is grounded.
    protected const float k_GroundedRadius = .2f;                           // Radius of the overlap circle to determine if grounded

    public bool m_FacingRight = true;                                       // For determining which way the player is currently facing.
    public bool dashing = false;                                            // true while dashing
    public bool blocking = false;                                           // Is the character blocking?

    protected Rigidbody2D m_Rigidbody2D;                                    // Reference to the players rigidbody
    protected CharacterStats stats;                                         // Reference to stats
    protected Transform graphics;                                           // Reference to the graphics child
    protected ChampionAnimationController animCon;                          // Reference to the Animation Contoller

    //Coroutines
    protected Coroutine comboRoutine;

    #region default methods

    protected virtual void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        graphics = transform.Find("graphics");
        stats = GetComponent<CharacterStats>();

        animCon = graphics.GetComponent<ChampionAnimationController>();
    }

    protected virtual void Update()
    {
        if (stats.trigHit)
        {
            stats.trigHit = false;
            animCon.trigHit = true;
        }
        animCon.statStunned = stats.stunned;
        animCon.statKnockedBack = stats.knockedBack;
    }

    protected virtual void FixedUpdate()
    {
        animCon.m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                animCon.m_Grounded = true;
            }
        }

        // Determines the vertical speed
        animCon.m_vSpeed = m_Rigidbody2D.velocity.y;

        //Move the character if dashing
        if (dashing && animCon.m_Grounded)
        {
            m_Rigidbody2D.velocity = new Vector2(m_dashSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion


    #region gets of cooldown

    public int CoolDownSkill(){



        return 0;
    }



    #endregion







    #region Normal character things (Move, Jump, JumpAttack, BasicAttack, Skills, ...)

    public virtual void Move(float move)
    {
        //only control the player if grounded or airControl is turned on and not jump attacking and not dashing
        if (!jumpAttacking && !stats.immovable)
        {
            //Slow down the player if he's attacking or blocking
            float maxSpeed;
            if (!canPerformAttack()) maxSpeed = m_MoveSpeedWhileAttacking;
            else if (blocking) maxSpeed = m_MoveSpeedWhileBlocking;
            else maxSpeed = m_MaxSpeed;

            // Prevent player from turning around while blocking
            //if (maxSpeed == 0) return;

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            animCon.m_Speed = Mathf.Abs(move);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                if (!canPerformAttack()) return; // prevent player from turning around while attacking
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                if (!canPerformAttack()) return;
                // ... flip the player.
                Flip();
            }

            // Move the character      
            m_Rigidbody2D.velocity = new Vector2(move * maxSpeed * stats.slowFactor, m_Rigidbody2D.velocity.y);
        }
    }

    public virtual void jump(bool jump)
    {
        // If the player should jump...
        if (canPerformAction(true) && jump && canPerformAttack())
        {
            // Add a vertical force to the player.
            animCon.m_Grounded = false;
            animCon.trigJump = true;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    protected virtual IEnumerator jumpAttack()
    {
        jumpAttacking = true; //Set status variable

        int direction;
        if (m_FacingRight) direction = 1;
        else direction = -1;

        m_Rigidbody2D.velocity = new Vector2( 4  *direction, -m_jumpAttackForce); //Add downwards force
        yield return new WaitUntil(() => animCon.m_Grounded); //Jump attacking as long as not grounded

        // Deal damage to all enemys
        StartCoroutine(doMeeleSkill_Hit(0, damage_JumpAttack, skillEffect.nothing, 0, false, m_jumpAttackRadius));

        Camera.main.GetComponent<CameraBehaviour>().startShake(); //Shake the camera
        jumpAttacking = false;
        animCon.trigJumpAttackEnd = true;
    }

    /// <summary>
    /// Manages the attacking and Combos
    /// </summary>
    public abstract void basicAttack(bool shouldAttack);

    public abstract void skill1();
    public abstract void skill2();
    public abstract void skill3();
    public abstract void skill4();

    //Dashes in the given direction
    public virtual IEnumerator dash(int direction)
    {
        if (canPerformAction(true) && canPerformAttack())
        {
            if (direction != 0 && stats.loseStamina(m_dashStaminaCost))
            {
                // flip if necessary
                if (direction < 0 && !m_FacingRight || direction > 0 && m_FacingRight) Flip();

                // Calculate new dashForce to go in right direction
                m_dashSpeed = Mathf.Abs(m_dashSpeed) * direction;
                m_Rigidbody2D.velocity = Vector2.zero;

                dashing = true;
                animCon.trigDash = true;
                stats.immovable = true;

                yield return new WaitForSeconds(0.1f);
                stats.invincible = true; //Player is invincible for a period of time while dashing

                yield return new WaitUntil(() => animCon.currentAnimation != AnimationController.AnimatorStates.Dash);
                dashing = false;
                stats.invincible = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); //Stop moving

                yield return new WaitForSeconds(0.04f); //Short time where character cant move after dashing
                stats.immovable = false;
            }
        }
    }

    public void manageDefensive(bool pDefensive)
    {
        if (pDefensive && animCon.m_Grounded)
        {
            //Start being defensive only if not defensive already and grounded  
            if (!blocking)
            {
                // Stop moving
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); 
            }
            blocking = true;
        }
        else
        {
            blocking = false;
        }
        animCon.statBlock = blocking;
    }

    public virtual void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = graphics.localScale;
        theScale.x *= -1;
        graphics.localScale = theScale;
    }

    #endregion

    #region Combo coroutine

    /// <summary>
    /// set/reset the combo after 1 second
    /// </summary>
    /// <returns></returns>
    protected IEnumerator setCombo()
    {
        inCombo = true;
        yield return new WaitForSeconds(m_ComboExpire);
        abortCombo();
    }

    /// <summary>
    /// (Re-)Sets the time of the combo
    /// </summary>
    protected void resetComboTime()
    {
        if (comboRoutine != null) StopCoroutine(comboRoutine);
        comboRoutine = StartCoroutine(setCombo());
    }

    /// <summary>
    /// Abort the combo and sets combo values to default
    /// </summary>
    protected void abortCombo()
    {
        if (comboRoutine != null) StopCoroutine(comboRoutine);
        inCombo = false;
        attackCount = 0;
    }

    #endregion

    #region Meele Skill

    public enum skillEffect
    {
        nothing,
        stun,
        knockback,
        bleed
    }

    /// <summary>
    /// do a complete skill
    /// </summary>
    /// <param name="animationVar">Trigger to set</param>
    /// <param name="skillDelay">Delay of skill if requiered, else 0</param>
    /// <param name="skillDamage">Damage of the skill</param>
    /// <param name="skillSpecialEffect">Special effect of the skill like nothing, stun, knockback or bleed</param>
    /// <param name="skillSpecialEffectTime">Duration of the special effect (only required for stun and bleed)</param>
    /// <param name="skillStaminaCost">Stamina costs of the skill</param>
    /// <param name="singleTarget">only the first target or all targets? (default: singleTarget - true)</param>
    /// <param name="skillRange">Range of the skill (default: meeleRange - 1.5f)</param>.
    /// </summary>
    protected void doMeeleSkill(ref bool animationVar, float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, bool singleTarget = true, float skillRange = meeleRange, bool needsToBeGrounded = true)
    {
        //Validate that character is not attacking and standing on ground
        if (canPerformAction(needsToBeGrounded) && canPerformAttack() && stats.loseStamina(skillStaminaCost))
        {
            // set animation trigger
            animationVar = true;
            // do hit by coroutine
            StartCoroutine(doMeeleSkill_Hit(skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, singleTarget, skillRange));
        }
    }

    protected IEnumerator doMeeleSkill_Hit(float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, bool singleTarget, float skillRange)
    {
        // wait till delay ends
        yield return new WaitForSeconds(skillDelay);

        CharacterStats target;
        RaycastHit2D[] hits = null;
        if (singleTarget)
        {
            // if single target skill - get only one hit
            RaycastHit2D singleTargetHit = tryToHit(skillRange);
            if (singleTargetHit)
            {
                hits = new RaycastHit2D[1];
                hits[0] = singleTargetHit;
            }
        }
        else
        {
            // if multi target skill - get all hits
            hits = Physics2D.CircleCastAll(m_GroundCheck.position, skillRange, Vector2.up, 0.01f, m_whatToHit);
        }

        // only if something is in range
        if (hits != null)
        {
            // loop through hits
            foreach (RaycastHit2D hit in hits)
            {
                // get the target of the hit
                target = hit.transform.gameObject.GetComponent<CharacterStats>();
                // add skill effect if required
                switch (skillSpecialEffect)
                {
                    case skillEffect.nothing:
                        // do nothing
                        break;
                    case skillEffect.stun:
                        target.startStunned(skillSpecialEffectTime);
                        break;
                    case skillEffect.knockback:
                        target.startKnockBack(transform.position);
                        break;
                    case skillEffect.bleed:
                        target.startBleeding(skillSpecialEffectTime);
                        break;
                    default:
                        // should not happen
                        throw new System.NotImplementedException();
                }
                // deal damage to target
                target.takeDamage(skillDamage, false);
            }
        }
    }

    /// <summary>
    /// Checks if the character hit anything
    /// </summary>
    protected RaycastHit2D tryToHit(float range)
    {
        Vector2 direction;// Direction to check in
        if (m_FacingRight) direction = Vector2.right;

        else direction = Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, m_whatToHit); //Send raycast
        return hit;
    }

    #endregion

    #region Ranged Skill

    protected void doRangeSkill(ref bool animationVar, float skillDelay, GameObject projectilePrefab, float range, int damage, skillEffect effect, int effectDuration, int skillStaminaCost, Vector2 speed, bool onHitEffect = false, bool needsToBeGrounded = true)
    {
        //Validate that character is not attacking and standing on ground
        if (canPerformAction(needsToBeGrounded) && canPerformAttack() && stats.loseStamina(skillStaminaCost))
        {
            // set animation trigger
            animationVar = true;
            // do hit by coroutine
            StartCoroutine(doRangeSkill_Hit(skillDelay, projectilePrefab, range, damage, effectDuration, speed, onHitEffect));
        }
    }

    private IEnumerator doRangeSkill_Hit(float skillDelay, GameObject projectilePrefab, float range, int damage, int effectDuration, Vector2 speed, bool onHitEffect)
    {
        // wait till delay ends
        yield return new WaitForSeconds(skillDelay);

        // calculate direction
        int direction;
        if (m_FacingRight) direction = 1;
        else direction = -1;

        // generate GameObject
        GameObject goProjectile = Instantiate(projectilePrefab, transform.position + new Vector3(1f * direction, 0.3f), new Quaternion(projectilePrefab.transform.rotation.x, 
            projectilePrefab.transform.rotation.y, projectilePrefab.transform.rotation.z * direction, projectilePrefab.transform.rotation.w));
        ProjectileBehaviour projectile = goProjectile.GetComponent<ProjectileBehaviour>();

        // assign variables to procetile Script
        projectile.direction = direction;
        projectile.creator = this.gameObject;
        projectile.whatToHit = m_whatToHit;
        projectile.range = range;
        projectile.speed = speed;
        projectile.explodeOnHit = onHitEffect;
        projectile.damage = damage;
        projectile.effectDuration = effectDuration;
        projectile.ready = true;
    }

    #endregion

    #region Character Can Perform

    /// <summary>
    /// Checks if Character is not knockedBack or Stunned
    /// </summary>
    /// <param name="GroundCheck">Check also, if character is on ground</param>
    /// <returns></returns>
    public bool canPerformAction(bool GroundCheck)
    {
        if (GroundCheck && !animCon.m_Grounded)
            return false;
        else if (stats.knockedBack)
            return false;
        else if (stats.stunned)
            return false;
        else if (dashing)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Checks if character is not attacking
    /// </summary>
    /// <returns></returns>
    public bool canPerformAttack()
    {
        switch (animCon.currentAnimation)
        {
            case AnimationController.AnimatorStates.BasicAttack1:
            case AnimationController.AnimatorStates.BasicAttack2:
            case AnimationController.AnimatorStates.BasicAttack3:
            case AnimationController.AnimatorStates.JumpAttack:
            case AnimationController.AnimatorStates.JumpAttackEnd:
            case AnimationController.AnimatorStates.Skill1:
            case AnimationController.AnimatorStates.Skill2:
            case AnimationController.AnimatorStates.Skill3:
            case AnimationController.AnimatorStates.Skill4:
                return false;
        }
        return true;
    }

    #endregion
}