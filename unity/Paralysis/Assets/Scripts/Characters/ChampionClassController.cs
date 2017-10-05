using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChampionClassController : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField]
    protected float m_MaxSpeed = 10f;                                     // The fastest the player can travel in the x axis.
    [SerializeField]
    protected float m_MoveSpeedWhileAttacking = 5f;                       // Max speed while attacking
    [SerializeField]
    protected float m_JumpForce = 400f;                                   // Amount of force added when the player jumps.         
    [SerializeField]
    protected float m_dashSpeed = 7f;                                   // Force applied when dashing
    [SerializeField]
    protected int m_dashStaminaCost = 10;
    [SerializeField]
    protected float m_jumpAttackRadius = 10f;                            // Radius of jump Attack damage
    [SerializeField]
    protected LayerMask m_WhatIsGround;                                   // A mask determining what is ground to the character
    [SerializeField]
    protected float m_ComboCounterMax = 1;                                // How long the next combostage is reachable (seconds)
    [SerializeField]
    protected float m_jumpAttackForce = 10f;
    public LayerMask whatToHit;                                           // What to hit when checking for hits while attacking


    protected Transform m_GroundCheck;                                    // A position marking where to check if the player is grounded.
    protected const float k_GroundedRadius = .2f;                         // Radius of the overlap circle to determine if grounded
    [SerializeField]
    protected bool m_Grounded;                                            // Whether or not the player is grounded.
    protected const float k_CeilingRadius = .02f;                         // Radius of the overlap circle to determine if the player can stand up
    protected Animator m_Anim;                                            // Reference to the player's animator component.
    protected Rigidbody2D m_Rigidbody2D;                                  // Reference to the players rigidbody
    protected CharacterStats stats;                                       // Reference to stats
    public bool m_FacingRight = true;                                     // For determining which way the player is currently facing.
    protected Transform graphics;                                         // Reference to the graphics child

    [Header("Range Variables")]
    protected const float meeleRange = 1.5f;

    [Header("Attacking Variables")]
    [SerializeField]
    protected int attackCount = 0;                                        // The ComboState 0 means the character has not attacked yet
    [SerializeField]
    protected bool inCombo = false;                                       // When true, the next comboStage can be reached
    [SerializeField]
    protected bool attacking = false;                                     // true, while the character is Attacking
    [SerializeField]
    protected bool jumpAttacking = false;                                 // True while the character is jump attacking
    [SerializeField]
    protected float[] attackLength;                                       // Stores the length of the characters attack animations in seconds. Order: [Basic Attack 1] [Basic Attack 2] [Basic Attack 3] [jump Attack] [Skill1] [Skill2] [Skill3] [Skill4]
    [SerializeField]
    private int damage_JumpAttack = 5;

    public bool dashing = false;                                          // true while dashing
    public bool dontMove = false;                                         // Character cannot move while true
    public bool defensive = false;                                        //Is the character defensive?

    protected Coroutine attackingRoutine;
    protected Coroutine comboRoutine;

    #region default methods

    protected virtual void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        graphics = transform.Find("graphics");
        stats = GetComponent<CharacterStats>();
    }

    protected virtual void Update()
    {
        m_Anim.SetBool("defensive", defensive);
    }

    protected virtual void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;               
            }
        }
        m_Anim.SetBool("Ground", m_Grounded);

        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

        //Move the character if dashing
        if (dashing && m_Grounded) m_Rigidbody2D.velocity = new Vector2(m_dashSpeed, m_Rigidbody2D.velocity.y);
    }

    #endregion

    #region Normal character things (Move, Jump, JumpAttack, BasicAttack, Skills, ...)

    public virtual void Move(float move)
    {
        //only control the player if grounded or airControl is turned on and not jump attacking and not dashing
        if (!jumpAttacking && !dashing && !dontMove)
        {
            //Slow down the player if he's attacking
            float maxSpeed;
            if (!attacking) maxSpeed = m_MaxSpeed;
            else maxSpeed = m_MoveSpeedWhileAttacking;

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                if (attacking) return; // prevent player from turning around while attacking
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                if (attacking) return;
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
        if (m_Grounded && jump && !dashing && !attacking)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    protected virtual IEnumerator jumpAttack()
    {
        jumpAttacking = true; //Set status variable
        m_Anim.SetBool("jumpAttack", true);

        int direction;
        if (m_FacingRight) direction = 1;
        else direction = -1;

        m_Rigidbody2D.velocity = new Vector2( 4  *direction, -m_jumpAttackForce); //Add downwards force
        yield return new WaitUntil(() => m_Grounded); //Jump attacking as long as not grounded
        
        //Get hit enemies
        RaycastHit2D[] hits = Physics2D.CircleCastAll(m_GroundCheck.position, m_jumpAttackRadius, Vector2.up, 0.01f, whatToHit);
        foreach(RaycastHit2D hit in hits)
        {
            //Deal damage to each
            hit.transform.gameObject.GetComponent<CharacterStats>().takeDamage(damage_JumpAttack, true);
        }

        Camera.main.GetComponent<CameraBehaviour>().startShake(); //Shake the camera
        jumpAttacking = false;
        m_Anim.SetBool("jumpAttack", false);
    }

    /// <summary>
    /// Manages the attacking and Combos
    /// </summary>
    public abstract void basicAttack(bool shouldAttack);

    //Dashes in the given direction
    public IEnumerator dash(int direction)
    {
        if (m_Grounded && !attacking && !dashing && stats.currentStamina >= m_dashStaminaCost)
        {
            if (direction != 0 && !dashing)
            {
                //lose stamina
                stats.loseStamina(m_dashStaminaCost);

                //flip if necessary
                if (direction < 0 && !m_FacingRight) Flip();
                else if (direction > 0 && m_FacingRight) Flip();

                //Calculate new dashForce to go in right direction
                m_dashSpeed = Mathf.Abs(m_dashSpeed) * direction;
                m_Rigidbody2D.velocity = Vector2.zero;

                //Start animation
                m_Anim.SetTrigger("dash");

                dashing = true;
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

    public abstract void skill1();
    public abstract void skill2();
    public abstract void skill3();
    public abstract void skill4();

    public void manageDefensive(bool pDefensive)
    {
        if (pDefensive)
        {
            //Start being defensive only if not defensive already   
            if (!defensive) m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); //Stop moving
            defensive = true;
        }
        else defensive = false;

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

    #region Attacking coroutine

    protected IEnumerator setAttacking(float seconds)
    {
        attacking = true;
        yield return new WaitForSeconds(seconds);
        attacking = false;
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
        yield return new WaitForSeconds(m_ComboCounterMax);
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

    protected enum skillEffect
    {
        nothing,
        stun,
        knockback,
        bleed
    }

    /// <summary>
    /// do a complete skill
    /// </summary>
    /// <param name="skillType">Value of attackLength[]. For order check property attackLength at ChampionClassController (Value 0-7)</param>
    /// <param name="calledAnimation">Name of the animation that should be played at the character</param>
    /// <param name="skillDelay">Delay of skill if requiered, else 0</param>
    /// <param name="skillDamage">Damage of the skill</param>
    /// <param name="skillSpecialEffect">Special effect of the skill like nothing, stun, knockback or bleed</param>
    /// <param name="skillSpecialEffectTime">Duration of the special effect (only required for stun and bleed)</param>
    /// <param name="skillStaminaCost">Stamina costs of the skill</param>
    /// <param name="singleTarget">only the first target or all targets? (default: singleTarget - true)</param>
    /// <param name="skillRange">Range of the skill (default: meeleRange - 1.5f)</param>.
    protected void doMeeleSkill(int skillType, string calledAnimation, float skillDelay, int skillDamage, skillEffect skillSpecialEffect, int skillSpecialEffectTime, int skillStaminaCost, bool singleTarget = true, float skillRange = meeleRange)
    {
        //Validate that character is not attacking and standing on ground
        if (!attacking && m_Grounded)
        {
            // check if enough stamina is left
            if (stats.currentStamina >= skillStaminaCost)
            {
                // set attacking coroutine
                if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                attackingRoutine = StartCoroutine(setAttacking(attackLength[skillType] - 0.08f));
                // set animation
                m_Anim.SetTrigger(calledAnimation);
                // do hit by coroutine
                StartCoroutine(doMeeleSkill_Hit(skillDelay, skillDamage, skillSpecialEffect, skillSpecialEffectTime, singleTarget, skillRange));
                // lose stamina for skill
                stats.loseStamina(skillStaminaCost);
            }
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
            hits = Physics2D.CircleCastAll(m_GroundCheck.position, m_jumpAttackRadius, Vector2.up, 0.01f, whatToHit);
        }

        // only if something is in range
        if (hits != null)
        {
            // loop throw hits
            foreach (RaycastHit2D hit in hits)
            {
                // get the target of the hit
                target = hit.transform.gameObject.GetComponent<CharacterStats>();
                // add skill effect if skill requires
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, whatToHit); //Send raycast
        return hit;
    }

    #endregion
}

