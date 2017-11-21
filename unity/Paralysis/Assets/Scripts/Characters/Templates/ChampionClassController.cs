using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChampionClassController : MonoBehaviour
{
    #region Parameters for Inspector

    [SerializeField]
    protected LayerMask m_WhatIsGround;                                     // A mask determining what is ground to the character
    public LayerMask m_whatToHit;                                           // What to hit when checking for hits while attacking

    [SerializeField]
    protected float m_MaxSpeed = 6f;                                        // The fastest the player can travel in the x axis.
    [SerializeField]
    protected float m_MoveSpeedWhileAttacking = 0.5f;                       // Max speed while attacking
    [SerializeField]
    protected float m_MoveSpeedWhileBlocking = 0f;                          // Max speed while blocking

    [SerializeField]
    protected float m_JumpForce = 700f;                                     // Amount of force added when the player jumps.  
    [SerializeField]
    protected float m_DoubleJumpForce = 600f;                               // Force added when doublejumping 
    [SerializeField]
    protected float m_jumpAttackRadius = 1.5f;                               // Radius of jump Attack damage
    [SerializeField]
    protected float m_jumpAttackForce = 10f;                                // Amount of force added when the player jump attack

    [SerializeField]
    protected float m_dashSpeed = 12f;                                      // Force applied when dashing
    [SerializeField]
    protected int m_dashStaminaCost = 10;
    [SerializeField]
    protected bool m_CanDashForward = false;

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
    protected SpriteRenderer shadowRenderer;
    protected const float k_GroundedRadius = .02f;                          // Radius of the overlap circle to determine if grounded
    protected bool doubleJumped = false;                                    // Has the character double jumped already?

    protected Skill basicAttack1_var;
    protected Skill basicAttack2_var;
    protected Skill basicAttack3_var;
    protected Skill skill1_var;
    protected Skill skill2_var;
    protected Skill skill3_var;
    protected Skill skill4_var;

    public Trinket Trinket1;
    public Trinket Trinket2;

    public string className;
    public bool m_FacingRight = true;                                       // For determining which way the player is currently facing.
    public bool dashing = false;                                            // true while dashing
    public bool blocking = false;                                           // Is the character blocking?

    protected Rigidbody2D m_Rigidbody2D;                                    // Reference to the players rigidbody
    protected CharacterStats stats;                                         // Reference to stats
    protected Transform graphics;                                           // Reference to the graphics child
    protected ChampionAnimationController animCon;                          // Reference to the Animation Contoller
    [HideInInspector]
    public HotbarController hotbar;

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
        shadowRenderer = m_GroundCheck.GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() { }

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

        //Reset doubleJumped when touching ground
        if (animCon.m_Grounded) doubleJumped = false;

        //Disable shadow if mid air
        if (shadowRenderer != null)
            shadowRenderer.enabled = animCon.m_Grounded;

        // Determines the vertical speed
        animCon.m_vSpeed = m_Rigidbody2D.velocity.y;

        //Move the character if dashing
        if (dashing && animCon.m_Grounded)
        {
            m_Rigidbody2D.velocity = new Vector2(m_dashSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion

    #region Normal character things (Move, Jump, JumpAttack, BasicAttack, Skills, ...)

    public virtual void Move(float move)
    {
        // only control the player if grounded or airControl is turned on and not jump attacking and not dashing
        if (!jumpAttacking && !stats.immovable)
        {
            // Slow down the player if he's attacking or blocking
            float maxSpeed;
            if (!CanPerformAttack()) maxSpeed = m_MoveSpeedWhileAttacking;
            else if (blocking) maxSpeed = m_MoveSpeedWhileBlocking;
            else maxSpeed = m_MaxSpeed;

            // Add movement effects to the speed
            maxSpeed *= stats.PercentageMovement;

            // Prevent player from turning around while blocking
            //if (maxSpeed == 0) return;

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            animCon.m_Speed = Mathf.Abs(move);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                if (!CanPerformAttack()) return; // prevent player from turning around while attacking
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                if (!CanPerformAttack()) return;
                // flip the player.
                Flip();
            }

            // Move the character      
            m_Rigidbody2D.velocity = new Vector2(move * maxSpeed * stats.slowFactor, m_Rigidbody2D.velocity.y);
        }
    }

    public virtual void Jump(bool jump)
    {
        // If the player should jump...
        if (CanPerformAction(false) && jump && CanPerformAttack())
        {
            if (animCon.m_Grounded && stats.LoseStamina(15))
            {
                // Add a vertical force to the player.
                animCon.m_Grounded = false;
                animCon.trigJump = true;
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            else if (!doubleJumped && !animCon.m_Grounded)
            {
                // Double Jump
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));

                // set animation
                animCon.trigJump = true;

                // set variable to prevent third jump
                doubleJumped = true;
            }
        }
    }

    protected virtual IEnumerator JumpAttack()
    {
        // Set status variable
        jumpAttacking = true; 
        animCon.trigJumpAttack = true;

        // Calculate direction
        int direction;
        if (m_FacingRight) direction = 1;
        else direction = -1;

        // Add downwards force
        m_Rigidbody2D.velocity = new Vector2(4 * direction, -m_jumpAttackForce); 
        yield return new WaitUntil(() => animCon.m_Grounded); // Jump attacking as long as not grounded
        m_Rigidbody2D.velocity = Vector2.zero; // Set velocity to zero to prevent character from moving after landing on ground
        Camera.main.GetComponent<CameraBehaviour>().startShake(); // Shake the camera

        // Deal damage to all enemies
        animCon.trigJumpAttackEnd = true;
        StartCoroutine(DoMeleeSkill_Hit(new MeleeSkill(0, 0, damage_JumpAttack, Skill.SkillEffect.nothing, 0, 10, Skill.SkillTarget.MultiTarget, 0, m_jumpAttackRadius)));

        // Wait till animation is finished and end jump attack
        yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimatorStates.JumpAttack);
        jumpAttacking = false;
    }

    /// <summary>
    /// Manages the attacking and Combos
    /// </summary>
    public abstract void BasicAttack(bool shouldAttack);

    public abstract void Skill1();
    public abstract void Skill2();
    public abstract void Skill3();
    public abstract void Skill4();

    //Dashes in the given direction
    public virtual IEnumerator Dash(int direction)
    {
        if (CanPerformAction(true) && CanPerformAttack())
        {
            if (direction != 0 && stats.LoseStamina(m_dashStaminaCost))
            {
                if (m_CanDashForward)
                {
                    // set var for dash or dashForward
                    if (direction < 0 && !m_FacingRight || direction > 0 && m_FacingRight)
                        animCon.trigDash = true;
                    else
                        animCon.trigDashForward = true;
                }
                else
                {
                    // flip if necessary
                    if (direction < 0 && !m_FacingRight || direction > 0 && m_FacingRight)
                        Flip();
                    animCon.trigDash = true;
                }

                // Calculate new dashForce to go in right direction
                m_dashSpeed = Mathf.Abs(m_dashSpeed) * direction;
                m_Rigidbody2D.velocity = Vector2.zero;

                dashing = true;
                stats.immovable = true;

                stats.invincible = true; //Player is invincible for a period of time while dashing
                yield return new WaitUntil(() => animCon.CurrentAnimation == AnimationController.AnimatorStates.Dash);
                yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimatorStates.Dash);
                dashing = false;
                stats.invincible = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); //Stop moving

                yield return new WaitForSeconds(0.04f); //Short time where character cant move after dashing
                stats.immovable = false;
            }
        }
    }

    public void ManageDefensive(bool pDefensive)
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

    #region Combo

    /// <summary>
    /// Do a default ComboAttack
    /// </summary>
    /// <param name="shouldAttack">bool</param>
    /// <param name="trigBA1">Animation Trigger Boolean for Basic Attack 1</param>
    /// <param name="trigBA2">Animation Trigger Boolean for Basic Attack 2</param>
    /// <param name="trigBA3">Animation Trigger Boolean for Basic Attack 3</param>
    protected void DoComboBasicAttack(bool shouldAttack, ref bool trigBA1, ref bool trigBA2,ref bool trigBA3)
    {
        if (shouldAttack && CanPerformAction(false) && CanPerformAttack())
        {
            if (animCon.m_Grounded)
            {
                // Check if enough stamina for attack
                if (stats.HasSufficientStamina(basicAttack1_var.staminaCost) && (attackCount == 0) || // Basic Attack 1
                    stats.HasSufficientStamina(basicAttack2_var.staminaCost) && (attackCount == 1) || // Basic Attack 2
                    stats.HasSufficientStamina(basicAttack3_var.staminaCost) && (attackCount == 2))   // Combo Attack
                {
                    // Already in combo?
                    if (!inCombo)
                    {
                        // First attack - initialize combo coroutine
                        ResetComboTime();
                        attackCount = 0;
                    }

                    // AttackCount increase per attack
                    attackCount++;

                    // Playing the correct animation depending on the attackCount and setting attacking status
                    switch (attackCount)
                    {
                        case 1:
                            // do meele attack
                            DoMeleeSkill(ref trigBA1, (MeleeSkill)basicAttack1_var);
                            // Reset timer of combo
                            ResetComboTime();
                            break;
                        case 2:
                            // do meele attack
                            DoMeleeSkill(ref trigBA2, (MeleeSkill)basicAttack2_var);
                            break;
                        case 3:
                            // do meele attack
                            DoMeleeSkill(ref trigBA3, (MeleeSkill)basicAttack3_var);
                            // Reset Combo after combo-hit
                            AbortCombo();
                            break;
                        default:
                            // Should not be triggered
                            AbortCombo();
                            break;

                    }
                }
            }
            // Jump attack only when falling
            else
            {
                // Check if enough stamina is left
                if (stats.LoseStamina(stamina_JumpAttack))
                {
                    // Jump Attack
                    StartCoroutine(JumpAttack());
                    // Abort combo
                    AbortCombo();
                }
            }
        }
    }

    /// <summary>
    /// set/reset the combo after 1 second
    /// </summary>
    /// <returns></returns>
    protected IEnumerator SetCombo()
    {
        inCombo = true;
        yield return new WaitForSeconds(m_ComboExpire);
        AbortCombo();
    }

    /// <summary>
    /// (Re-)Sets the time of the combo
    /// </summary>
    protected void ResetComboTime()
    {
        if (comboRoutine != null) StopCoroutine(comboRoutine);
        comboRoutine = StartCoroutine(SetCombo());
    }

    /// <summary>
    /// Abort the combo and sets combo values to default
    /// </summary>
    protected void AbortCombo()
    {
        if (comboRoutine != null) StopCoroutine(comboRoutine);
        inCombo = false;
        attackCount = 0;
    }

    #endregion

    #region Meele Skill

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
    protected void DoMeleeSkill(ref bool animationVar, MeleeSkill skillToPerform, bool NoValidation = false)
    {
        //Validate that character is not attacking and standing on ground
        if (NoValidation || CanPerformAction(skillToPerform.needsToBeGrounded) && CanPerformAttack())
        {
            if (skillToPerform.notOnCooldown && stats.LoseStamina(skillToPerform.staminaCost))
            {
                // set animation trigger
                animationVar = true;
                // do hit by coroutine
                StartCoroutine(DoMeleeSkill_Hit(skillToPerform));
            }
        }
    }

    protected IEnumerator DoMeleeSkill_Hit(MeleeSkill skillToPerform)
    {
        // wait till delay ends
        yield return new WaitForSeconds(skillToPerform.delay);

        CharacterStats target;
        RaycastHit2D[] hits = null;
        if (skillToPerform.targetType == Skill.SkillTarget.SingleTarget)
        {
            // if single target skill - get only one hit
            RaycastHit2D singleTargetHit = TryToHit(skillToPerform.range);
            if (singleTargetHit)
            {
                hits = new RaycastHit2D[1];
                hits[0] = singleTargetHit;
            }
        }
        else if (skillToPerform.targetType == Skill.SkillTarget.MultiTarget)
        {
            // if multi target skill - get all hits
            hits = Physics2D.CircleCastAll(m_GroundCheck.position, skillToPerform.range, Vector2.up, 0.01f, m_whatToHit);
        }
        else
        {
            // Only enemys in front of the character
            Vector3 postion = m_GroundCheck.position;
            if (m_FacingRight) postion.x += (skillToPerform.range / 2);
            else postion.x -= (skillToPerform.range / 2);
            hits = Physics2D.CircleCastAll(postion, skillToPerform.range/2, Vector2.up, 0.01f, m_whatToHit);
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
                switch (skillToPerform.effect)
                {
                    case Skill.SkillEffect.nothing:
                        // do nothing
                        break;
                    case Skill.SkillEffect.stun:
                        target.StartStunned(skillToPerform.effectDuration);
                        break;
                    case Skill.SkillEffect.knockback:
                        target.StartKnockBack(transform.position);
                        break;
                    case Skill.SkillEffect.bleed:
                        target.StartBleeding(skillToPerform.effectDuration);
                        break;
                    default:
                        // should not happen
                        throw new NotImplementedException();
                }
                // deal damage to target
                stats.DealDamage(target, skillToPerform.damage, false);
            }
        }

        StartCoroutine(SetSkillOnCooldown(skillToPerform));
    }

    /// <summary>
    /// Checks if the character hit anything
    /// </summary>
    protected RaycastHit2D TryToHit(float range)
    {
        Vector2 direction;// Direction to check in
        if (m_FacingRight) direction = Vector2.right;
        else direction = Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, m_whatToHit); //Send raycast
        return hit;
    }

    #endregion

    #region Ranged Skill

    protected void DoRangeSkill(ref bool animationVar, RangedSkill skillToPerform)
    {
        //Validate that character is not attacking and standing on ground
        if (CanPerformAction(skillToPerform.needsToBeGrounded) && CanPerformAttack() && skillToPerform.notOnCooldown && stats.LoseStamina(skillToPerform.staminaCost))
        {
            // set animation trigger
            animationVar = true;
            // do hit by coroutine
            StartCoroutine(DoRangeSkill_Hit(skillToPerform));
        }
    }

    private IEnumerator DoRangeSkill_Hit(RangedSkill skillToPerform)
    {
        // wait till delay ends
        yield return new WaitForSeconds(skillToPerform.delay);

        // calculate direction
        int direction;
        if (m_FacingRight) direction = 1;
        else direction = -1;

        // generate GameObject
        GameObject goProjectile = skillToPerform.prefab;
        ProjectileBehaviour projectile = goProjectile.GetComponent<ProjectileBehaviour>();

        // assign variables to projectile Script
        projectile.direction = direction;
        projectile.creator = this.gameObject;
        projectile.whatToHit = m_whatToHit;
        projectile.range = skillToPerform.range;
        projectile.speed = skillToPerform.speed;
        projectile.effect = skillToPerform.effect;
        projectile.explodeOnHit = skillToPerform.onHitEffect;
        projectile.damage = skillToPerform.damage;
        projectile.effectDuration = skillToPerform.effectDuration;

        Instantiate(goProjectile, transform.position + new Vector3(1f * direction, 0.3f), new Quaternion(goProjectile.transform.rotation.x,
            goProjectile.transform.rotation.y, goProjectile.transform.rotation.z * direction, goProjectile.transform.rotation.w));

        StartCoroutine(SetSkillOnCooldown(skillToPerform));
    }

    #endregion

    #region setOnCooldown

    protected IEnumerator SetSkillOnCooldown(Skill skillToPerform)
    {
        //Set on cooldown
        if (skillToPerform.cooldown > 0)
        {
            hotbar.setOnCooldown(skillToPerform.name, skillToPerform.cooldown);

            skillToPerform.notOnCooldown = false;
            yield return new WaitForSeconds(skillToPerform.cooldown);
            skillToPerform.notOnCooldown = true;
        }
    }

    #endregion

    #region Character Can Perform

    /// <summary>
    /// Checks if Character is not knockedBack or Stunned
    /// </summary>
    /// <param name="GroundCheck">Check also, if character is on ground</param>
    /// <returns></returns>
    public bool CanPerformAction(bool GroundCheck)
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
    public bool CanPerformAttack()
    {
        switch (animCon.CurrentAnimation)
        {
            case AnimationController.AnimatorStates.BasicAttack1:
            case AnimationController.AnimatorStates.BasicAttack2:
            case AnimationController.AnimatorStates.BasicAttack3:
            case AnimationController.AnimatorStates.JumpAttack:
            case AnimationController.AnimatorStates.Skill1:
            case AnimationController.AnimatorStates.Skill2:
            case AnimationController.AnimatorStates.Skill3:
            case AnimationController.AnimatorStates.Skill4:
                return false;
        }
        return true;
    }

    public bool ShouldRegenerateStamina()
    {
        if (!CanPerformAttack())
            return false;
        else
        {
            switch (animCon.CurrentAnimation)
            {
                case AnimationController.AnimatorStates.Jump:
                case AnimationController.AnimatorStates.Dash:
                case AnimationController.AnimatorStates.Hit:
                case AnimationController.AnimatorStates.DoubleJump:
                case AnimationController.AnimatorStates.DashFor:
                    return false;
            }
        }
        return true;
    }

    #endregion
}