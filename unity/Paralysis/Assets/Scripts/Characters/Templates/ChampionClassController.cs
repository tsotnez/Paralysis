using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ChampionClassController : Photon.MonoBehaviour
{
    // Constraints
    protected const float GroundedRadius = .02f;                            // Radius of the overlap circle to determine if grounded
    protected const float MeeleRange = 1.5f;                                // Default range for meele attacks
    protected const float FallThroughDuration = .5f;                        // Duration of falling through a platform

    #region Parameters for Inspector

    // Layers
    public LayerMask m_WhatIsGround;                                        // A mask determining what is ground to the character
    public LayerMask m_whatToHit;                                           // What to hit when checking for hits while attacking
    public LayerMask m_fallThroughMask;                                     // A layermask to determine if a player can fall through something

    // Movement
    [SerializeField]
    protected float m_MaxSpeed = 6f;                                        // The fastest the player can travel in the x axis.
    [SerializeField]
    protected float m_MoveSpeedWhileAttacking = 0.5f;                       // Max speed while attacking
    [SerializeField]
    protected float m_MoveSpeedWhileBlocking = 0f;                          // Max speed while blocking
    [SerializeField]
    protected bool CanTurnAroundWhileBlocking = true;                       // Can turn around while blocking

    // Jump & JumpAttack
    [SerializeField]
    protected float m_JumpForce = 700f;                                     // Amount of force added when the player jumps.  
    [SerializeField]
    protected float m_DoubleJumpForce = 600f;                               // Force added when doublejumping 
    [SerializeField]
    protected float m_jumpAttackRadius = 1.5f;                              // Radius of jump Attack damage
    [SerializeField]
    protected float m_jumpAttackForce = 10f;                                // Amount of force added when the player jump attack

    // Dash
    [SerializeField]
    protected float m_dashSpeed = 12f;                                      // Force applied when dashing
    [SerializeField]
    protected int m_dashStaminaCost = 10;                                   // Stamina Costs of Skill Dash
    [SerializeField]
    protected bool CanDashForward = false;                                  // Indicates whether the character can dash forward or have to turn around before dashing

    // Combo
    [SerializeField]
    protected float ComboExpire = 1;                                        // How long the next combostage is reachable (seconds)
    protected int attackCount = 0;                                          // The ComboState 0 means the character has not attacked yet
    protected bool inCombo = false;                                         // When true, the next comboStage can be reached

    // Skill Delay
    [SerializeField]
    protected float delay_BasicAttack1 = 0;
    [SerializeField]
    protected float delay_BasicAttack2 = 0;
    [SerializeField]
    protected float delay_BasicAttack3 = 0;
    [SerializeField]
    protected float delay_JumpAttack = 0;
    [SerializeField]
    protected float delay_Skill1 = 0;
    [SerializeField]
    protected float delay_Skill2 = 0;
    [SerializeField]
    protected float delay_Skill3 = 0;
    [SerializeField]
    protected float delay_Skill4 = 0;

    // Skills Stamina Costs
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

    // Skill Damage
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

    // Skill Cooldown
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

    // Objects
    public ChampionAndTrinketDatabase.Champions className;
    protected Transform m_GroundCheck;                                      // A position marking where to check if the player is grounded.
    protected Transform ProjectilePosition;                                 // A position marking where a projectile shall be spawned.
    protected SpriteRenderer shadowRenderer;
    protected Rigidbody2D m_Rigidbody2D;                                    // Reference to the players rigidbody
    protected CharacterStats stats;                                         // Reference to stats
    protected Transform graphics;                                           // Reference to the graphics child
    protected ChampionAnimationController animCon;                          // Reference to the Animation Contoller
    [HideInInspector]
    public HotbarController hotbar;

    // Skills & Trinkets
    protected Skill basicAttack1_var;
    protected Skill basicAttack2_var;
    protected Skill basicAttack3_var;
    protected Skill jumpAttack_var;
    protected Skill skill1_var;
    protected Skill skill2_var;
    protected Skill skill3_var;
    protected Skill skill4_var;
    public Trinket Trinket1;
    public Trinket Trinket2;

    // Stats
    public bool FacingRight = true;                                         // For determining which way the player is currently facing.
    public bool dashing = false;                                            // true while dashing
    public bool blocking = false;                                           // Is the character blocking?
    public bool doubleJumped = false;                                       // Has the character double jumped already?
    protected bool casting = false;                                         // Is the character casting (RangeAttack)?
    protected bool jumpAttacking = false;                                   // True while the character is jump attacking
    protected bool fallingThrough = false;                                  // True while we are falling through a platform
    private bool applyDashingForce = false;                                 // true while force for dashing shall be applieds

    //Coroutines
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
            photonView.RPC("OnNewPlayerInstantiated", PhotonTargets.All);
            //If the player is not mine set the rigidbody to not kinematic
            //and Disable updates on this object
            if(!photonView.isMine)
            {
                enabled = false;
                m_Rigidbody2D.isKinematic = true;
            }
        }

        // Set default JumpAttack
        jumpAttack_var = new MeleeSkill(0, 0, damage_JumpAttack, Skill.SkillEffect.nothing, 0, 0, 10, Skill.SkillTarget.MultiTarget, 0, m_jumpAttackRadius, ChampionAndTrinketDatabase.Champions.Alchemist);
    }

    protected virtual void FixedUpdate()
    {
        animCon.m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                animCon.m_Grounded = true;
            }
        }

        // Reset doubleJumped when touching ground
        if (animCon.m_Grounded) doubleJumped = false;

        // Disable shadow if mid air
        if (shadowRenderer != null)
            shadowRenderer.enabled = animCon.m_Grounded;

        // Determines the vertical speed
        animCon.m_vSpeed = m_Rigidbody2D.velocity.y;

        // Move the character if dashing
        if (applyDashingForce && animCon.m_Grounded)
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
            if (!CanTurnAroundWhileBlocking && maxSpeed == 0) return;

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            animCon.m_Speed = Mathf.Abs(move * maxSpeed);

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

    public virtual bool CheckFallThrough()
    {        
        //If we are currently not falling through check
        if (!fallingThrough)
        {
            RaycastHit2D hit = Physics2D.Raycast (m_GroundCheck.position, transform.up * -1,
                                  1f, m_fallThroughMask);

            //If the raycast hit and we are grounded make the player fall through
            if ((hit && animCon.m_Grounded)) 
            {
                StartCoroutine (fallThrough(hit.collider.gameObject));
                return true;
            }
        }
        return false;
    }

    protected virtual IEnumerator fallThrough(GameObject fallThroughObj)
    {
        //Get all the colliders and set them to ignore the falling through obj
        fallingThrough = true;
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach(Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(collider, fallThroughObj.GetComponent<Collider2D>(), fallingThrough);
        }
        yield return new WaitForSeconds(FallThroughDuration);

        //Then set them back
        fallingThrough = false;
        foreach(Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(collider, fallThroughObj.GetComponent<Collider2D>(), fallingThrough);
        }
    }

    public virtual IEnumerator JumpAttack()
    {
        // Check if enough stamina is left
        if (CanPerformAttack() && stats.LoseStamina(stamina_JumpAttack))
        {
            // Set status variable
            jumpAttacking = true;
            animCon.trigJumpAttack = true;
            yield return new WaitForSeconds(jumpAttack_var.delay);

            // Calculate direction
            int direction;
            if (FacingRight) direction = 1;
            else direction = -1;

            // Add downwards force
            m_Rigidbody2D.velocity = new Vector2(4 * direction, -m_jumpAttackForce);
            yield return new WaitUntil(() => animCon.m_Grounded);                       // Jump attacking as long as not grounded
            m_Rigidbody2D.velocity = Vector2.zero;                                      // Set velocity to zero to prevent character from moving after landing on ground
            Camera.main.GetComponent<CameraBehaviour>().startShake();                   // Shake the camera

            // Deal damage to all enemies
            animCon.trigJumpAttackEnd = true;
            StartCoroutine(DoMeleeSkill_Hit((MeleeSkill)jumpAttack_var));

            // Wait till animation is finished and end jump attack
            yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimatorStates.JumpAttack);
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
                AnimationController.AnimatorStates RequiredAnimState;
                if (CanDashForward)
                {
                    // set var for dash or dashForward
                    if (direction < 0 && !FacingRight || direction > 0 && FacingRight)
                    {
                        // set correct animation for validation
                        RequiredAnimState = AnimationController.AnimatorStates.DashFor;
                        animCon.trigDashForward = true;
                    }
                    else
                    {
                        // set correct animation for validation
                        RequiredAnimState = AnimationController.AnimatorStates.Dash;
                        animCon.trigDash = true;
                    }
                }
                else
                {
                    // set correct animation for validation
                    RequiredAnimState = AnimationController.AnimatorStates.Dash;

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
                yield return new WaitUntil(() => animCon.CurrentAnimation == RequiredAnimState);                                    // Wait till animation has started

                // If an EndAnimation is present do some extra stuff
                if (animCon.AnimationDictionaryHasAnimation(RequiredAnimState, AnimationController.TypeOfAnimation.EndAnimation))
                {
                    yield return new WaitUntil(() => animCon.CurrentAnimationState == AnimationController.AnimationState.Waiting);  // Wait till animation is in state waiting
                    yield return new WaitUntil(() => animCon.m_Grounded);                                                           // Wait till character is on ground

                    // Start EndAnimation
                    applyDashingForce = false;

                    if (RequiredAnimState == AnimationController.AnimatorStates.DashFor) animCon.trigDashForwardEnd = true;
                    else animCon.trigDashEnd = true;
                }
                else
                    Debug.LogAssertion("DashEnd-Animation not found. Only Dash-Animation has been played!");

                yield return new WaitUntil(() => animCon.CurrentAnimation != RequiredAnimState);                                    // Wait till next animation is present
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
        if (pDefensive && animCon.m_Grounded && CanPerformAttack())
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
    /// <param name="shouldAttack">bool</param>
    /// <param name="trigBA1">Animation Trigger Boolean for Basic Attack 1</param>
    /// <param name="trigBA2">Animation Trigger Boolean for Basic Attack 2</param>
    /// <param name="trigBA3">Animation Trigger Boolean for Basic Attack 3</param>
    protected void DoComboBasicAttack(ref bool trigBA1, ref bool trigBA2, ref bool trigBA3)
    {
        if (CanPerformAction(false) && CanPerformAttack() && animCon.m_Grounded)
        {
            // Check if enough stamina for attack
            if ((stats.HasSufficientStamina(basicAttack1_var.staminaCost) && attackCount == 0) || // Basic Attack 1
                (stats.HasSufficientStamina(basicAttack2_var.staminaCost) && attackCount == 1) || // Basic Attack 2
                (stats.HasSufficientStamina(basicAttack3_var.staminaCost) && attackCount == 2))   // Combo Attack
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
    protected void DoMeleeSkill(ref bool animationVar, MeleeSkill skillToPerform, bool NoValidation = false)
    {
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

    protected void DoRangeSkill(ref bool animationVar, RangedSkill skillToPerform)
    {
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

        if(PhotonNetwork.offlineMode)
        {
            StartCoroutine(rangedSkill(skillToPerform, direction, ProjectilePosition.position));
        }
        else
        {
            photonView.RPC("RPC_spawnRangedSkill", PhotonTargets.All, skillToPerform.rangedSkillId, (short) direction, ProjectilePosition.position);
        }
    }

    [PunRPC]
    public void RPC_spawnRangedSkill(int rangedSkillId, short direction, Vector3 position)
    {
        RangedSkill skillToPerform = RangedSkill.rangedSkillDict[rangedSkillId];
        StartCoroutine(rangedSkill(skillToPerform, direction, position));
    }

    public IEnumerator rangedSkill(RangedSkill skillToPerform, int direction, Vector3 position)
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

        goProjectile = Instantiate(goProjectile, position,
            new Quaternion(goProjectile.transform.rotation.x, goProjectile.transform.rotation.y,
                goProjectile.transform.rotation.z * direction, goProjectile.transform.rotation.w));    

        /*
        goProjectile = Instantiate(goProjectile, ProjectilePosition.position,
            new Quaternion(goProjectile.transform.rotation.x, goProjectile.transform.rotation.y,
                goProjectile.transform.rotation.z * direction, goProjectile.transform.rotation.w));        
        */

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

        if(PhotonNetwork.offlineMode || photonView.isMine)
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

    #region Reset

    /// <summary>
    /// Resets everything, so the game can be restarted
    /// </summary>
    public void ResetValues()
    {
        basicAttack1_var.notOnCooldown = true;
        basicAttack2_var.notOnCooldown = true;
        basicAttack3_var.notOnCooldown = true;
        skill1_var.notOnCooldown = true;
        skill2_var.notOnCooldown = true;
        skill3_var.notOnCooldown = true;
        skill4_var.notOnCooldown = true;

        Trinket1.resetValues();
        Trinket2.resetValues();

        hotbar.resetValues();
    }

    #endregion
}