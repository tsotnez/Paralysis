using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ChampionClassController : Photon.MonoBehaviour
{
    // Constraints
    protected const float GroundedRadius = .02f;                // Radius of the overlap circle to determine if grounded
    protected const float FallThroughDuration = .5f;            // Duration of falling through a platform
    protected const float AllowAnotherJumpAfterGround = .1f;    // How long to wait to be able to jump again after grounded

    #region Parameters for Inspector

    // Character identification
    public ChampionDatabase.Champions className;                // Character Name
    public string characterFullName;                            // Character Name with title
    [Multiline(6)]
    public string characterLore;                                // Character description

    // Layers
    public LayerMask m_WhatIsGround;                            // A mask determining what is ground to the character
    public LayerMask m_whatToHit;                               // What to hit when checking for hits while attacking
    public LayerMask m_fallThroughMask;                         // A layermask to determine if a player can fall through something

    // Movement
    [SerializeField]
    protected float m_MaxSpeed = 6f;                            // The fastest the player can travel in the x axis.
    [SerializeField]
    protected float m_MoveSpeedWhileAttacking = 0.5f;           // Max speed while attacking
    [SerializeField]
    protected float m_MoveSpeedWhileBlocking = 0f;              // Max speed while blocking
    [SerializeField]
    protected bool m_CanTurnAroundWhileBlocking = true;         // Can turn around while blocking

    // Jump & JumpAttack
    [SerializeField]
    protected float m_initialJumpVelocity = 100f;               // Initial Y velocity when we start jumping
    [SerializeField]
    protected float m_maxJumpTime = .3f;                        // Max time in air going up
    [SerializeField]
    public int stamina_Jump = 15;                               // Stamina costs for a Jump
    [SerializeField]
    protected float m_JumpDivisor = 1f;                         // Jump divsor
    [SerializeField]
    protected float m_DoubleJumpDivisor = 1.05f;                // Double jump divsor
    [SerializeField]
    protected float m_jumpAttackForce = 10f;                    // Amount of force added when the player jump attack

    // Dash
    [SerializeField]
    protected float m_dashSpeed = 12f;                          // Force applied when dashing
    [SerializeField]
    public int m_dashStaminaCost = 10;                          // Stamina Costs of Skill Dash
    [SerializeField]
    public bool m_CanDashForward = false;                       // Indicates whether the character can dash forward or have to turn around before dashing

    // Combo
    [SerializeField]
    protected float ComboExpire = 1;                            // How long the next combostage is reachable (seconds)
    protected int attackCount = 0;                              // The ComboState 0 means the character has not attacked yet
    protected bool inCombo = false;                             // When true, the next comboStage can be reached

    // Skills
    [SerializeField]
    protected MeleeSkill[] MeleeSkills;                         // Contains all melee skills
    [SerializeField]
    protected RangedSkill[] RangeSkills;                        // Contains all range skills

    // Trinkets
    public Trinket Trinket1;
    public Trinket Trinket2;

    #endregion

    // Objects
    [HideInInspector]
    public HotbarController hotbar;
    protected Transform m_GroundCheck;                          // A position marking where to check if the player is grounded.
    protected Transform ProjectilePosition;                     // A position marking where a projectile shall be spawned.
    protected SpriteRenderer shadowRenderer;                    // Reference to the shadow of the character (ground).
    protected Rigidbody2D m_Rigidbody2D;                        // Reference to the players rigidbody.
    protected CharacterStats stats;                             // Reference to stats.
    protected Transform graphics;                               // Reference to the graphics child.
    protected ChampionAnimationController animCon;              // Reference to the Animation Contoller.

    // Stats
    public bool FacingRight = true;                             // For determining which way the player is currently facing.
    public bool dashing = false;                                // true while dashing
    public bool blocking = false;                               // Is the character blocking?
    public bool doubleJumped = false;                           // Has the character double jumped already?
    protected bool casting = false;                             // Is the character casting (RangeAttack)?
    protected bool jumpAttacking = false;                       // True while the character is jump attacking
    protected bool fallingThrough = false;                      // True while we are falling through a platform
    private bool applyDashingForce = false;                     // true while force for dashing shall be applieds
    private bool m_canDoubleJump = true;                        // true if the player can double jump
    private bool m_canJump = true;                              // true when the player can start a new jump
    private float m_allowAnotherJump = 0f;                      // time to wait for allowing the player to jump
    private float m_timeInAir = 0f;
    private bool m_jumpPressed = false;

    // Coroutines
    protected Coroutine comboRoutine;

    #region default methods

    protected virtual void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        ProjectilePosition = transform.Find("ProjectilePosition");
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        graphics = transform.Find("graphics");
        stats = GetComponent<CharacterStats>();
        animCon = graphics.GetComponent<ChampionAnimationController>();
        shadowRenderer = m_GroundCheck.GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() { }

    protected virtual void Start()
    {
        // Refresh manager instance list of players
        if (!PhotonNetwork.offlineMode)
        {
            //If the player is not mine set the rigidbody to not kinematic
            //and Disable updates on this object
            if (!photonView.isMine)
            {
                enabled = false;
                m_Rigidbody2D.isKinematic = true;
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        animCon.propGrounded = false;

        if (m_Rigidbody2D.velocity.y == 0)
        {
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    animCon.propGrounded = true;
                }
            }
        }

        if (animCon.propGrounded) m_timeInAir = 0;
        else m_timeInAir += Time.deltaTime;

        // Reset doubleJumped when touching ground
        if (animCon.propGrounded) doubleJumped = false;

        // Disable shadow if mid air
        if (shadowRenderer != null)
            shadowRenderer.enabled = animCon.propGrounded;

        // Determines the vertical speed
        animCon.propVSpeed = m_Rigidbody2D.velocity.y;

        // Move the character if dashing
        if (applyDashingForce && animCon.propGrounded)
        {
            m_Rigidbody2D.velocity = new Vector2(m_dashSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion

    #region Normal character things (Move, Jump, JumpAttack, BasicAttack, Skills, ...)

    public virtual void Move(float move)
    {
        if (stats.stunned && !stats.knockedBack)
        {
            // Prevent from sliding after knockback while stunned 
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        else if (stats.stunned || stats.knockedBack)
        {
            // Ignore input while stunned or knockedback
        }
        // only control the player if grounded or airControl is turned on and not jump attacking and not dashing
        else if (!jumpAttacking && !stats.immovable)
        {
            // Slow down the player if he's attacking or blocking
            float maxSpeed;
            if (!CanPerformAttack()) maxSpeed = m_MoveSpeedWhileAttacking;
            else if (blocking) maxSpeed = m_MoveSpeedWhileBlocking;
            else maxSpeed = m_MaxSpeed;

            // Add movement effects to the speed
            maxSpeed *= stats.PercentageMovement;

            // Prevent player from turning around while blocking
            if (!m_CanTurnAroundWhileBlocking && maxSpeed == 0) return;

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            animCon.propSpeed = Mathf.Abs(move * maxSpeed);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !FacingRight)
            {
                if (!CanPerformAttack()) return; // prevent player from turning around while attacking
                // flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && FacingRight)
            {
                if (!CanPerformAttack()) return;
                // flip the player.
                Flip();
            }

            // Move the character      
            m_Rigidbody2D.velocity = new Vector2(move * maxSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    public virtual void Jump(bool jump)
    {
        m_jumpPressed = CanPerformAction(false) && jump;
        if (Time.time < m_allowAnotherJump) return;

        // If the player should jump...
        if (m_jumpPressed && CanPerformAttack())
        {
            if (m_canJump && (animCon.propGrounded || m_timeInAir < .2f) && stats.LoseStamina(stamina_Jump))
            {
                m_canJump = false;
                animCon.propGrounded = false;
                m_canDoubleJump = false;
                animCon.trigJump = true;
                StartCoroutine(JumpWaitForGrounded());
                StartCoroutine(JumpRoutine(m_JumpDivisor));
            }
        }

        //Handle double jump
        if (m_jumpPressed && !animCon.propGrounded && CanPerformAction(false) && m_canDoubleJump && !doubleJumped)
        {
            animCon.trigDoubleJump = true;
            doubleJumped = true;
            StartCoroutine(JumpRoutine(m_DoubleJumpDivisor));
        }

        //if we let go of jump in the air and we havent double jumped
        if (!jump && !animCon.propGrounded && !doubleJumped && CanPerformAction(false))
        {
            m_canDoubleJump = true;
        }
    }

    private IEnumerator JumpRoutine(float divisor)
    {
        m_Rigidbody2D.velocity = Vector2.zero;
        float timer = 0;
        float maxJumpTime = m_maxJumpTime / divisor;
        float intialJumpV = m_initialJumpVelocity / divisor;

        while (m_jumpPressed && timer < maxJumpTime)
        {
            float proportionCompleted = timer / maxJumpTime;
            Vector2 currentVel = m_Rigidbody2D.velocity;
            currentVel.y = intialJumpV;
            Vector2 thisFrameJumpV = Vector2.Lerp(currentVel, Vector2.zero, proportionCompleted);
            m_Rigidbody2D.AddForce(thisFrameJumpV);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator JumpWaitForGrounded()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => animCon.propGrounded);
        Vector2 currentVel = m_Rigidbody2D.velocity;
        currentVel.y = 0;
        m_Rigidbody2D.velocity = currentVel;
        m_canJump = true;
        m_allowAnotherJump = Time.time + AllowAnotherJumpAfterGround;
    }

    public virtual bool CheckFallThrough()
    {
        //If we are currently not falling through check
        if (!fallingThrough)
        {
            RaycastHit2D hit = Physics2D.Raycast(m_GroundCheck.position, transform.up * -1,
                                  1f, m_fallThroughMask);

            //If the raycast hit and we are grounded make the player fall through
            if ((hit && animCon.propGrounded))
            {
                StartCoroutine(FallThrough(hit.collider.gameObject));
                return true;
            }
        }
        return false;
    }

    protected virtual IEnumerator FallThrough(GameObject fallThroughObj)
    {
        //Get all the colliders and set them to ignore the falling through obj
        fallingThrough = true;
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(collider, fallThroughObj.GetComponent<Collider2D>(), fallingThrough);
        }
        yield return new WaitForSeconds(FallThroughDuration);

        //Then set them back
        fallingThrough = false;
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(collider, fallThroughObj.GetComponent<Collider2D>(), fallingThrough);
        }
    }

    public virtual IEnumerator JumpAttack()
    {
        MeleeSkill skillJumpAttack = GetMeleeSkillByType(Skill.SkillType.JumpAttack);
        // Check if enough stamina is left
        if (CanPerformAttack() && stats.LoseStamina(skillJumpAttack.staminaCost))
        {
            // Set status variable
            jumpAttacking = true;
            animCon.trigJumpAttack = true;
            yield return new WaitForSeconds(skillJumpAttack.delay);

            // Calculate direction
            int direction;
            if (FacingRight) direction = 1;
            else direction = -1;

            // Add downwards force
            m_Rigidbody2D.velocity = new Vector2(4 * direction, -m_jumpAttackForce);
            yield return new WaitUntil(() => animCon.propGrounded);                       // Jump attacking as long as not grounded
            m_Rigidbody2D.velocity = Vector2.zero;                                      // Set velocity to zero to prevent character from moving after landing on ground
            Camera.main.GetComponent<CameraBehaviour>().startShake();                   // Shake the camera

            // Deal damage to all enemies
            animCon.trigJumpAttackEnd = true;
            StartCoroutine(DoMeleeSkill_Hit(skillJumpAttack));

            // Wait till animation is finished and end jump attack
            yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimationTypes.JumpAttack);
            jumpAttacking = false;
            doubleJumped = false;

            // Stop Combo
            AbortCombo();
        }
    }

    /// <summary>
    /// Dashes in the given direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual IEnumerator Dash(int direction)
    {
        if (CanPerformAction(true) && CanPerformAttack())
        {
            if (direction != 0 && stats.LoseStamina(m_dashStaminaCost))
            {
                AnimationController.AnimationTypes RequiredAnimState;
                if (m_CanDashForward)
                {
                    // set var for dash or dashForward
                    if (direction < 0 && !FacingRight || direction > 0 && FacingRight)
                    {
                        // set correct animation for validation
                        RequiredAnimState = AnimationController.AnimationTypes.DashFor;
                        animCon.trigDashForward = true;
                    }
                    else
                    {
                        // set correct animation for validation
                        RequiredAnimState = AnimationController.AnimationTypes.Dash;
                        animCon.trigDash = true;
                    }
                }
                else
                {
                    // set correct animation for validation
                    RequiredAnimState = AnimationController.AnimationTypes.Dash;

                    // flip if necessary
                    if (direction < 0 && !FacingRight || direction > 0 && FacingRight)
                        Flip();
                    animCon.trigDash = true;
                }

                // Calculate new dashForce to go in right direction
                m_dashSpeed = Mathf.Abs(m_dashSpeed) * direction;
                m_Rigidbody2D.velocity = Vector2.zero;

                dashing = true;
                applyDashingForce = true;
                stats.immovable = true;

                // Player is invincible for a period of time while dashing
                stats.invincible = true;
                yield return new WaitUntil(() => animCon.CurrentAnimation == RequiredAnimState);                                // Wait till animation has started

                // If an EndAnimation is present do some extra stuff
                yield return new WaitUntil(() => animCon.CurrentAnimationState == AnimationController.AnimationState.Waiting);  // Wait till animation is in state waiting
                yield return new WaitUntil(() => animCon.propGrounded);                                                         // Wait till character is on ground

                // Start EndAnimation
                applyDashingForce = false;

                if (RequiredAnimState == AnimationController.AnimationTypes.DashFor) animCon.trigDashForwardEnd = true;
                else animCon.trigDashEnd = true;

                yield return new WaitUntil(() => animCon.CurrentAnimation != RequiredAnimState);                                // Wait till next animation is present
                stats.invincible = false;

                // Set end of dash
                applyDashingForce = false;
                dashing = false;

                // Short time where character can't move after dashing
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); // Stop moving
                yield return new WaitForSeconds(0.04f);
                stats.immovable = false;
            }
        }
    }

    public void ManageDefensive(bool pDefensive)
    {
        if (pDefensive && animCon.propGrounded && CanPerformAttack())
        {
            // Start being defensive only if not defensive already and grounded  
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
        FacingRight = !FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = graphics.localScale;
        theScale.x *= -1;
        graphics.localScale = theScale;

        // Flip ProjectilePosition
        theScale = ProjectilePosition.transform.localPosition;
        theScale.x *= -1;
        ProjectilePosition.transform.localPosition = theScale;
    }

    /// <summary>
    /// Manages the attacking and Combos
    /// </summary>
    public abstract void BasicAttack();
    public abstract void Skill1();
    public abstract void Skill2();
    public abstract void Skill3();
    public abstract void Skill4();

    #endregion

    #region Combo

    /// <summary>
    /// Do a default ComboAttack
    /// </summary>
    /// <param name="trigBA1">Animation Trigger Boolean for Basic Attack 1</param>
    /// <param name="trigBA2">Animation Trigger Boolean for Basic Attack 2</param>
    /// <param name="trigBA3">Animation Trigger Boolean for Basic Attack 3</param>
    protected void DoComboBasicAttack(ref bool trigBA1, ref bool trigBA2, ref bool trigBA3)
    {
        if (CanPerformAction(false) && CanPerformAttack() && animCon.propGrounded)
        {
            // Check if enough stamina for attack
            if ((stats.HasSufficientStamina(GetMeleeSkillByType(Skill.SkillType.BasicAttack1).staminaCost) && attackCount == 0) || // Basic Attack 1
                (stats.HasSufficientStamina(GetMeleeSkillByType(Skill.SkillType.BasicAttack2).staminaCost) && attackCount == 1) || // Basic Attack 2
                (stats.HasSufficientStamina(GetMeleeSkillByType(Skill.SkillType.BasicAttack3).staminaCost) && attackCount == 2))   // Combo Attack
            {
                // Already in combo?
                if (!inCombo)
                {
                    // First attack - initialize combo coroutine
                    ResetComboTime();
                    attackCount = 0;
                }

                // AttackCount: increase per attack
                attackCount++;

                // Playing the correct animation depending on the attackCount and setting attacking status
                switch (attackCount)
                {
                    case 1:
                        // do meele attack
                        DoMeleeSkill(ref trigBA1, Skill.SkillType.BasicAttack1);
                        // Reset timer of combo
                        ResetComboTime();
                        break;
                    case 2:
                        // do meele attack
                        DoMeleeSkill(ref trigBA2, Skill.SkillType.BasicAttack2);
                        break;
                    case 3:
                        // do meele attack
                        DoMeleeSkill(ref trigBA3, Skill.SkillType.BasicAttack3);
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
    }

    /// <summary>
    /// set/reset the combo after 1 second
    /// </summary>
    /// <returns></returns>
    protected IEnumerator SetCombo()
    {
        inCombo = true;
        yield return new WaitForSeconds(ComboExpire);
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

    #region Melee Skill

    /// <summary>
    /// do a complete melee skill
    /// </summary>
    /// <param name="animationVar">Animation Trigger</param>
    /// <param name="skillToPerform">MeleeSkill that shall be performed</param>
    /// <param name="NoValidation"></param>
    protected void DoMeleeSkill(ref bool animationVar, Skill.SkillType SkillType, bool NoValidation = false)
    {
        MeleeSkill skillToPerform = GetMeleeSkillByType(SkillType);
        if (skillToPerform == null)
        {
            throw new NullReferenceException("Skill that shall be performed can not be null");
        }

        //Validate that character is not attacking and standing on ground
        if (NoValidation || CanPerformAction(skillToPerform.needsToBeGrounded) && CanPerformAttack())
        {
            if (skillToPerform.notOnCooldown && stats.LoseStamina(skillToPerform.staminaCost))
            {
                // set animation trigger
                animationVar = true;
                hotbar.StartCoroutine(hotbar.flashBlack(skillToPerform.type));
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
            if (FacingRight) postion.x += (skillToPerform.range / 2);
            else postion.x -= (skillToPerform.range / 2);
            hits = Physics2D.CircleCastAll(postion, skillToPerform.range / 2, Vector2.up, 0.01f, m_whatToHit);
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
                    case Skill.SkillEffect.slow:
                        target.StartSlow(skillToPerform.effectDuration, skillToPerform.effectValue);
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
        if (FacingRight) direction = Vector2.right;
        else direction = Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, m_whatToHit); //Send raycast
        return hit;
    }

    #endregion

    #region Ranged Skill

    protected void DoRangeSkill(ref bool animationVar, Skill.SkillType SkillType)
    {
        RangedSkill skillToPerform = GetRangeSkillByType(SkillType);
        if (skillToPerform == null)
        {
            throw new NullReferenceException("Skill that shall be performed can not be null");
        }

        //Validate that character is not attacking and standing on ground
        if (CanPerformAction(skillToPerform.needsToBeGrounded) && CanPerformAttack()
            && skillToPerform.notOnCooldown && stats.LoseStamina(skillToPerform.staminaCost))
        {
            // set animation trigger
            animationVar = true;
            hotbar.StartCoroutine(hotbar.flashBlack(skillToPerform.type));
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
        if (FacingRight) direction = 1;
        else direction = -1;

        if (PhotonNetwork.offlineMode)
        {
            StartCoroutine(RangedSkill(skillToPerform, direction));
        }
        else
        {
            photonView.RPC("RPC_SpawnRangedSkill", PhotonTargets.All, skillToPerform.rangedSkillId, (short)direction);
        }
    }

    [PunRPC]
    public void RPC_SpawnRangedSkill(short rangedSkillId, short direction)
    {
        RangedSkill skillToPerform = global::RangedSkill.rangedSkillDict[rangedSkillId];
        StartCoroutine(RangedSkill(skillToPerform, direction));
    }

    public IEnumerator RangedSkill(RangedSkill skillToPerform, int direction)
    {
        GameObject goProjectile;
        goProjectile = skillToPerform.prefab;

        // assign variables to projectile Script
        ProjectileBehaviour projectile = goProjectile.GetComponent<ProjectileBehaviour>();
        projectile.direction = direction;
        projectile.creator = this.gameObject;
        projectile.whatToHit = m_whatToHit;

        if (direction == -1)
            goProjectile.GetComponent<SpriteRenderer>().flipX = true;
        else
            goProjectile.GetComponent<SpriteRenderer>().flipX = false;

        goProjectile = Instantiate(goProjectile, ProjectilePosition.position,
            new Quaternion(goProjectile.transform.rotation.x, goProjectile.transform.rotation.y,
                goProjectile.transform.rotation.z * direction, goProjectile.transform.rotation.w));
        goProjectile.layer = GameConstants.PROJECTILE_LAYER;

        // Apply skill to projectile
        projectile = goProjectile.GetComponent<ProjectileBehaviour>();
        projectile.SkillValues = skillToPerform;

        // Wait till cast is finished to interrupt new skills/actions
        if (skillToPerform.castTime > 0)
        {
            // Stop Moving
            m_Rigidbody2D.velocity = Vector2.zero;
            casting = true;
            stats.immovable = true;
            yield return new WaitUntil(() => !projectile.CastFinished || projectile.Interrupted); // Wait till cast has started
            yield return new WaitUntil(() => projectile.CastFinished || projectile.Interrupted);  // Wait till cast is finished
            stats.immovable = false;
            casting = false;
        }

        if (PhotonNetwork.offlineMode || photonView.isMine)
        {
            StartCoroutine(SetSkillOnCooldown(skillToPerform));
        }
    }


    #endregion

    #region setOnCooldown

    protected IEnumerator SetSkillOnCooldown(Skill skillToPerform)
    {
        //Set on cooldown
        if (skillToPerform.cooldown > 0)
        {
            hotbar.setOnCooldown(skillToPerform.type, skillToPerform.cooldown);

            skillToPerform.notOnCooldown = false;
            yield return new WaitForSeconds(skillToPerform.cooldown);
            skillToPerform.notOnCooldown = true;
        }
    }

    #endregion

    #region Skill stuff

    public Skill GetSkillByType(Skill.SkillType Type)
    {
        Skill returnValue = null;

        // Check for skill in MeleeSkills
        returnValue = GetMeleeSkillByType(Type);
        if (returnValue != null)
            return returnValue;

        // Check for skill in RangeSkills
        returnValue = GetRangeSkillByType(Type);

        // Return null or what is found
        return returnValue;
    }

    public MeleeSkill GetMeleeSkillByType(Skill.SkillType Type)
    {
        // Check for skill in MeleeSkills
        foreach (MeleeSkill skill in MeleeSkills)
        {
            if (skill.type == Type)
            {
                return skill;
            }
        }

        // Return null if nothing is found
        return null;
    }

    public RangedSkill GetRangeSkillByType(Skill.SkillType Type)
    {
        // Check for skill in RangeSkills
        foreach (RangedSkill skill in RangeSkills)
        {
            if (skill.type == Type)
            {
                return skill;
            }
        }

        // Return null if nothing is found
        return null;
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
        if (GroundCheck && !animCon.propGrounded)
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
            case AnimationController.AnimationTypes.BasicAttack1:
            case AnimationController.AnimationTypes.BasicAttack2:
            case AnimationController.AnimationTypes.BasicAttack3:
            case AnimationController.AnimationTypes.JumpAttack:
            case AnimationController.AnimationTypes.Skill1:
            case AnimationController.AnimationTypes.Skill2:
            case AnimationController.AnimationTypes.Skill3:
            case AnimationController.AnimationTypes.Skill4:
            case AnimationController.AnimationTypes.KnockedBack:
            case AnimationController.AnimationTypes.Stunned:
                return false;
        }
        if (casting)
        {
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
                case AnimationController.AnimationTypes.Jump:
                case AnimationController.AnimationTypes.Dash:
                case AnimationController.AnimationTypes.Hit:
                case AnimationController.AnimationTypes.DoubleJump:
                case AnimationController.AnimationTypes.DashFor:
                    return false;
            }
        }
        return true;
    }

    #endregion

    #region Reset

    /// <summary>
    /// Resets everything, so the game can be restarted
    /// </summary>
    public void ResetValues()
    {
        foreach (MeleeSkill skill in MeleeSkills)
        {
            skill.notOnCooldown = true;
        }
        // Check for skill in RangeSkills
        foreach (RangedSkill skill in RangeSkills)
        {
            skill.notOnCooldown = true;
        }
        Trinket1.resetValues();
        Trinket2.resetValues();

        hotbar.resetValues();
    }

    #endregion
}