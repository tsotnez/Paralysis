﻿using System.Collections;
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
    protected bool m_AirControl = false;                                  // Whether or not a player can steer while jumping;
    [SerializeField]
    protected float m_dashForce = 400f;                                   // Force applied when dashing
    [SerializeField]
    protected LayerMask m_WhatIsGround;                                   // A mask determining what is ground to the character
    [SerializeField]
    protected float m_ComboCounterMax = 1;                                // How long the next combostage is reachable (seconds)
    [SerializeField]
    protected float m_jumpAttackForce = 10f;
    public LayerMask whatToHit;                                           // What to hit when checking for hits while attacking
    protected MyTimer timer;

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

    protected int attackCount = 0;                                        // The ComboState 0 means the character has not attacked yet
    protected bool inCombo = false;                                       // When true, the next comboStage can be reached
    protected bool attacking = false;                                     // true, while the character is Attacking
    protected bool jumpAttacking = false;                                 // True while the character is jump attacking
    public bool dashing = false;                                          // true while dashing
    public bool dontMove = false;                                         // Character cannot move while true

    [SerializeField]
    protected float[] attackLength;                                       // Stores the length of the characters attack animations in seconds. Order: [Basic Attack 1] [Basic Attack 2] [Basic Attack 3] [jump Attack] [Skill1] [Skill2] [Skill3] [Skill4]


    protected virtual void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_Anim = GetComponentInChildren<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        graphics = transform.Find("graphics");
        timer = new MyTimer(m_ComboCounterMax);
        stats = GetComponent<CharacterStats>();
    }

    protected virtual void Update()
    {
        timer.Update();
        m_Anim.SetBool("jumpAttack", jumpAttacking);

        //Move the character if dashing
        if (dashing && m_Grounded) transform.position += new Vector3(m_dashForce * Time.deltaTime, 0, 0);
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
    }


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

            // Move the character
            m_Rigidbody2D.velocity = new Vector2(move * maxSpeed * stats.slowFactor, m_Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
    }

    public virtual void jump(bool jump)
    {
        // If the player should jump...
        if (m_Grounded && jump && m_Anim.GetBool("Ground"))
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
        m_Rigidbody2D.velocity = new Vector2(0, -m_jumpAttackForce);
        yield return new WaitForSeconds(attackLength[3]);
        jumpAttacking = false;
    }

    /// <summary>
    /// Manages the attacking and Combos
    /// </summary>
    public abstract void basicAttack(bool shouldAttack);

    //Dashes in the given direction
    public IEnumerator dash(int direction)
    {
        if (direction != 0 && !dashing)
        {
            //flip if necessary
            if (direction < 0 && !m_FacingRight) Flip();
            else if (direction > 0 && m_FacingRight) Flip();

            //Calculate new dashForce
            m_dashForce = Mathf.Abs(m_dashForce) * direction;
            m_Rigidbody2D.velocity = Vector2.zero;

            //Start animation
            m_Anim.SetTrigger("dash");

            dashing = true;
            dontMove = true;

            yield return new WaitForSeconds(0.5f);
            dashing = false;

            yield return new WaitForSeconds(0.2f);
            dontMove = false;
        }
    }

    public abstract void skill1();
    public abstract void skill2();
    public abstract void skill3();
    public abstract void skill4();


    protected IEnumerator setAttacking(float seconds)
    {
        attacking = true;
        yield return new WaitForSeconds(seconds);
        attacking = false;
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
}

