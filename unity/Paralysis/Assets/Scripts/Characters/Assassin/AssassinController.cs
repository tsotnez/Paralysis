using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinController : ChampionClassController
{
    [Header("Assassin Specific")]
    [SerializeField]
    protected float m_DoubleJumpForce = 400f;                             // Force added when doublejumping  
    private bool doubleJumped = false;                                    // Has the character double jumped already?
    public bool invisible = false;
    Coroutine invisRoutine = null;
    [SerializeField]
    private GameObject bulletPrefab;

    public override void jump(bool jump)
    {
        if (m_Grounded && jump) /*m_Anim.GetBool("Ground")*/
            base.jump(jump);
        else if (!m_Grounded && jump && !doubleJumped)
        {
            // Add a vertical force to the player.
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));
            //m_Anim.SetTrigger("DoubleJump");
            doubleJumped = true;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(m_Grounded) doubleJumped = false;
    }

    protected override void Update()
    {
        base.Update();
        if(stats.stunned && invisible) stopInvisible();
        if(stats.knockedBack && invisible) stopInvisible();
    }

    /// <summary>
    /// Stun attack
    /// </summary>
    public override void skill1()
    {
        if (invisible) stopInvisible();
        doMeeleSkill(ref trigBasicAttack1, delay_BasicAttack1, damage_BasicAttack1, skillEffect.stun, 3, stamina_BasicAttack1);
    }

    /// <summary>
    /// Vanish and become invisible
    /// </summary>
    public override void skill2()
    {
        if (m_Grounded && !dashing && canPerformAttack() && stats.hasSufficientStamina(stamina_Skill2))
        {
            if (invisible) stopInvisible();

            if (invisRoutine != null) StopCoroutine(invisRoutine);
            invisRoutine = StartCoroutine(manageInvisibility());
            stats.loseStamina(stamina_Skill2);
        }
    }

    /// <summary>
    /// Teleport to closest enemy on same height and deal damage
    /// </summary>
    public override void skill3()
    {
        if (m_Grounded && !dashing && canPerformAttack() && stats.hasSufficientStamina(stamina_Skill3))
        {
            if (invisible) stopInvisible();
            StartCoroutine(shadowStepHit());
            stats.loseStamina(stamina_Skill3);
        }
    }

    /// <summary>
    /// Shoot pistol and knock back
    /// </summary>
    public override void skill4()
    { 
        if (invisible) stopInvisible();
        rangedAttack(bulletPrefab, 5, damage_Skill4, skillEffect.knockback, 2);
    }

    private IEnumerator manageInvisibility()
    {
        yield return new WaitForSeconds(delay_Skill3/*attackLength[5] - 0.05f*/);
        invisible = true;

        //Set transparency
        Color oldCol = GetComponentInChildren<SpriteRenderer>().color;
        oldCol.a = 0.5f;
        GetComponentInChildren<SpriteRenderer>().color = oldCol;

        yield return new WaitForSeconds(5);
        stopInvisible();
    }

    public override void basicAttack(bool shouldAttack)
    {
        if (shouldAttack && canPerformAttack() && !dashing)
        {
            if (!m_Grounded && !invisible && doubleJumped) //Jump attack only when falling and double jumped
            {
                //Jump Attack
                StartCoroutine(jumpAttack());
                //reset combo
                abortCombo();
            }
            else if (m_Grounded && invisible)
            {
                attackCount = 4;
            }
            // Determining the attackCount
            else if (m_Grounded && attackCount == 0 && !inCombo)
            {
                //First attack - initialize combo coroutine
                resetComboTime();
                attackCount = 1;
            }
            else if (m_Grounded && inCombo)
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
                        //set animation
                        //m_Anim.SetTrigger("Attack");
                        // do basic attack
                        Invoke("basicAttackHit", delay_BasicAttack1);
                        break;
                    case 2:
                        //set animation
                        //m_Anim.SetTrigger("Attack2");
                        // do basic attack
                        Invoke("basicAttackHit", delay_BasicAttack2);
                        break;
                    case 3:
                        //set animation
                        //m_Anim.SetTrigger("Attack3");
                        // do bleed attack
                        Invoke("bleedAttackHit", delay_BasicAttack3);

                        //reset Combo
                        abortCombo();
                        break;
                    case 4:
                        //set animation
                        //m_Anim.SetTrigger("shadowstep");
                        //do ambush attack
                        Invoke("ambushAttackHit", delay_Skill3);
                        //reset Combo
                        abortCombo();
                        //end invisibility
                        stopInvisible();
                        break;
                }
                //pay stamina for attack
                stats.loseStamina(stamina_BasicAttack1);
            }
        }
    }

    private void basicAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);

        //Dealing damage accordingly
        int dmg = 0;
        if (attackCount == 1) dmg = 5;
        else if (attackCount == 2) dmg = 7;

        if(hit == true) hit.transform.gameObject.GetComponent<CharacterStats>().takeDamage(dmg, true); //Let the hit character take damage
    }

    private void ambushAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        if (hit == true) hit.transform.gameObject.GetComponent<CharacterStats>().takeDamage(13, true); //Let the hit character take damage
    }

    private void bleedAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        CharacterStats target;
        if (hit == true)
        {
            target = hit.transform.gameObject.GetComponent<CharacterStats>();
            target.startBleeding(6);
            target.takeDamage(5, true);
        }
    }

    private void shootAttackHit()
    {
        rangedAttack(bulletPrefab, 5, 20, skillEffect.knockback, 0);
    }

    private void stunAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        CharacterStats target;
        if (hit == true)
        {
            target = hit.transform.gameObject.GetComponent<CharacterStats>();
            target.startStunned(3);
            target.takeDamage(5, false);
        }
    }

    private IEnumerator shadowStepHit()
    {
        //Add walls and Ground to layermask so they are an obstacle for the raycast
        LayerMask temp = whatToHit;
        temp |= (1 << LayerMask.NameToLayer("Walls"));
        temp |= (1 << LayerMask.NameToLayer("Ground"));


        int targetLocation = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right,  1000f,temp); //Right hit
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
            dontMove = true;
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
            doMeeleSkill(ref trigSkill3, delay_Skill3, 5, skillEffect.stun, 3, stamina_Skill3);
            yield return new WaitForSeconds(delay_Skill4);
            dontMove = false;
        }
    }

    private void stopInvisible()
    {
        invisible = false;
        if(invisRoutine !=null) StopCoroutine(invisRoutine);
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    #region Character specific animation

    protected override bool additionalNotInterruptCondition(AnimationController.AnimatorStates activeAnimation)
    {
        return false;
    }

    protected override bool additionalAnimationCondition(AnimationController animCon)
    {
        return false;
    }

    #endregion
}

