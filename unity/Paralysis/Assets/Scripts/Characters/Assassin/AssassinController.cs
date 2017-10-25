using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinController : ChampionClassController
{
    [Header("Assassin Specific")]
    [SerializeField]
    protected float m_DoubleJumpForce = 400f;                             // Force added when doublejumping 
    [SerializeField]
    private GameObject bulletPrefab;

    private bool doubleJumped = false;                                    // Has the character double jumped already?
    public bool invisible = false;
    Coroutine invisRoutine = null;

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<AssassinAnimationController>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (animCon.m_Grounded) doubleJumped = false;
    }

    protected override void Update()
    {
        base.Update();
        if (stats.stunned && invisible) stopInvisible();
        if (stats.knockedBack && invisible) stopInvisible();
    }

    #region Skills

    /// <summary>
    /// Stun attack
    /// </summary>
    public override void skill1()
    {
        if (invisible) stopInvisible();
        doMeeleSkill(ref animCon.trigSkill1, delay_Skill1, damage_Skill1, skillEffect.stun, 3, stamina_Skill1);
    }

    /// <summary>
    /// Vanish and become invisible
    /// </summary>
    public override void skill2()
    {
        if (canPerformAction(true) && canPerformAttack() && stats.loseStamina(stamina_Skill2))
        {
            if (invisible) stopInvisible();
            if (invisRoutine != null) StopCoroutine(invisRoutine);
            invisRoutine = StartCoroutine(manageInvisibility());
        }
    }

    /// <summary>
    /// Teleport to closest enemy on same height and deal damage
    /// </summary>
    public override void skill3()
    {
        if (canPerformAction(true) && canPerformAttack() && stats.loseStamina(stamina_Skill3))
        {
            if (invisible) stopInvisible();
            StartCoroutine(shadowStepHit());
        }
    }

    /// <summary>
    /// Shoot pistol and knock back
    /// </summary>
    public override void skill4()
    {
        if (invisible) stopInvisible();
        doRangeSkill(ref animCon.trigSkill4, delay_Skill4, bulletPrefab, 5, damage_Skill4, skillEffect.knockback, 2, stamina_Skill4);
    }

    #endregion

    #region Basic Attack

    public override void basicAttack(bool shouldAttack)
    {
        if (shouldAttack && canPerformAttack() && !dashing)
        {
            if (!animCon.m_Grounded && !invisible && doubleJumped) //Jump attack only when falling and double jumped
            {
                //Jump Attack
                StartCoroutine(jumpAttack());
                //reset combo
                abortCombo();
            }
            else if (animCon.m_Grounded && invisible)
            {
                attackCount = 4;
            }
            // Determining the attackCount
            else if (animCon.m_Grounded && attackCount == 0 && !inCombo)
            {
                //First attack - initialize combo coroutine
                resetComboTime();
                attackCount = 1;
            }
            else if (animCon.m_Grounded && inCombo)
            {
                //Already in combo
                resetComboTime();
                attackCount++;
            }

            //Playing the correct animation depending on the attackCount and setting attacking status
            if (stats.hasSufficientStamina(stamina_BasicAttack1))
            {
                switch (attackCount)
                {
                    case 1:
                        // do basic attack
                        doMeeleSkill(ref animCon.trigBasicAttack1, delay_BasicAttack1, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1);
                        break;
                    case 2:
                        // do basic attack
                        doMeeleSkill(ref animCon.trigBasicAttack2, delay_BasicAttack2, damage_BasicAttack2, skillEffect.nothing, 0, stamina_BasicAttack2);
                        break;
                    case 3:
                        // do basic attack
                        doMeeleSkill(ref animCon.trigBasicAttack3, delay_BasicAttack3, damage_BasicAttack3, skillEffect.bleed, 0, stamina_BasicAttack3);

                        //reset Combo
                        abortCombo();
                        break;
                    case 4:
                        // do skill 3
                        doMeeleSkill(ref animCon.trigSkill3, delay_Skill3, damage_Skill3, skillEffect.nothing, 3, stamina_Skill3);
                        //reset Combo
                        abortCombo();
                        //end invisibility
                        stopInvisible();
                        break;
                }
            }
        }
    }

    #endregion

    #region Double Jump

    public override void jump(bool jump)
    {
        if (animCon.m_Grounded && jump)
            base.jump(jump);
        else if (!animCon.m_Grounded && jump && !doubleJumped)
        {
            // Add a vertical force to the player.
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));

            // set animation
            ((AssassinAnimationController)animCon).trigDoubleJump = true;

            // set variable to prevent third jump
            doubleJumped = true;
        }
    }

    #endregion

    #region Teleport Skill

    private IEnumerator shadowStepHit()
    {
        //Add walls and Ground to layermask so they are an obstacle for the raycast
        LayerMask temp = m_whatToHit;
        temp |= (1 << LayerMask.NameToLayer("Walls"));
        temp |= (1 << LayerMask.NameToLayer("Ground"));

        // set animation
        animCon.trigSkill3 = true;

        int targetLocation = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1000f, temp); //Right hit
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1000f, temp); //Left hit

        if ((hit == true && hit.transform.tag == "Player") && (hitLeft == true && hitLeft.transform.gameObject.tag == "Player"))
        {
            //There is a target on both sides, check for distance
            if (hit.distance <= hitLeft.distance) targetLocation = 1; //Teleport to target on the right   
            else targetLocation = -1; //Teleport to target on the left
        }
        else if (hit == true && hit.transform.gameObject.tag == "Player") targetLocation = 1;
        else if (hitLeft == true && hitLeft.transform.gameObject.tag == "Player") targetLocation = -1;

        if (targetLocation != 0)
        {
            stats.immovable = true;
            if (targetLocation == 1)
            {
                m_Rigidbody2D.position = hit.transform.position + Vector3.left; //Viable target on the right
                if (!m_FacingRight) Flip();
            }
            else if (targetLocation == -1)
            {
                m_Rigidbody2D.position = hitLeft.transform.position + Vector3.right; //Viable target on the left
                if (m_FacingRight) Flip();
            }
            m_Rigidbody2D.velocity = Vector2.zero;
            doMeeleSkill(ref animCon.trigSkill3, delay_Skill3, 5, skillEffect.stun, 3, stamina_Skill3);
            yield return new WaitForSeconds(delay_Skill4);
            stats.immovable = false;
        }
    }

    #endregion

    #region Invisibility

    private IEnumerator manageInvisibility()
    {
        yield return new WaitForSeconds(delay_Skill2);
        invisible = true;

        // set animation
        animCon.trigSkill2 = true;

        // set transparency
        Color oldCol = GetComponentInChildren<SpriteRenderer>().color;
        oldCol.a = 0.5f;
        GetComponentInChildren<SpriteRenderer>().color = oldCol;

        yield return new WaitForSeconds(5);
        stopInvisible();
    }

    private void stopInvisible()
    {
        invisible = false;
        if (invisRoutine != null) StopCoroutine(invisRoutine);
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    #endregion
}

