using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinController : ChampionClassController
{
    [SerializeField]
    protected float m_DoubleJumpForce = 400f;                             // Force added when doublejumping  
    private bool doubleJumped = false;                                    // Has the character double jumped already?
    public bool invisible = false;
    Coroutine invisRoutine = null;
    Coroutine attackingRoutine = null;

    public override void jump(bool jump)
    {
        if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            base.jump(jump);
        else if (!m_Grounded && jump && !doubleJumped)
        {
            // Add a vertical force to the player.
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));
            m_Anim.SetTrigger("DoubleJump");
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

    public override void skill1()
    {
        if (invisible) stopInvisible();
        attackingRoutine = StartCoroutine(setAttacking(attackLength[4]));
        m_Anim.SetTrigger("stunAttack");
    }

    public override void skill2()
    {
        if (invisible) stopInvisible();
        attackingRoutine = StartCoroutine(setAttacking(attackLength[5]));
        m_Anim.SetTrigger("vanish");
        if(invisRoutine != null) StopCoroutine(invisRoutine);
        invisRoutine = StartCoroutine(manageInvisibility());
    }

    public override void skill3()
    {
        if (invisible) stopInvisible();
        attackingRoutine = StartCoroutine(setAttacking(attackLength[6]));
        
        m_Anim.SetTrigger("shadowstep");
    }

    public override void skill4()
    {
        if (invisible) stopInvisible();
        attackingRoutine = StartCoroutine(setAttacking(attackLength[7]));
        m_Anim.SetTrigger("shoot");
    }

    private IEnumerator manageInvisibility()
    {
        yield return new WaitForSeconds(attackLength[5] - 0.05f);
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
        // When the timer is over the player is not allowed to continue his combo
        if (timer.hasFinished())
        {
            //reset combo
            inCombo = false;
            attackCount = 0;
            timer.timerStop();
            timer.reset();
        }

        if (shouldAttack && !attacking)
        {
            if (!m_Grounded && !invisible) //Jump attack only when falling
            {
                //Jump Attack
                StartCoroutine(jumpAttack());
                attackingRoutine = StartCoroutine(setAttacking(attackLength[3] - 0.08f));
                //reset combo
                inCombo = false;
                attackCount = 0;
                timer.timerStop();
                timer.reset();
            }
            // Determining the attackCount
            else if (m_Grounded && attackCount == 0 && !inCombo)
            {
                //First attack
                attackCount++;
                timer.timerStart();
                inCombo = true;
                if (invisible) stopInvisible();
            }
            else if (m_Grounded && inCombo)
            {
                //Already in combo
                attackCount++;
                timer.reset();
                if (invisible) stopInvisible();
            }

            //Playing the correct animation depending on the attackCount and setting attacking status
            switch (attackCount)
            {
                case 1:
                    m_Anim.SetTrigger("Attack");
                    if(attackingRoutine != null) StopCoroutine(attackingRoutine);
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[0] - 0.08f));
                    tryToHit(2, 9);
                    break;
                case 2:
                    m_Anim.SetTrigger("Attack2");
                    if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[1] - 0.08f));
                    tryToHit(2, 9);
                    break;
                case 3:
                    m_Anim.SetTrigger("Attack3");
                    if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[2] - 0.08f));
                    tryToHit(2, 9);
                    inCombo = false;
                    attackCount = 0;
                    timer.timerStop();
                    timer.reset();
                    break;
            }
        }
    }

    private void tryToHit(float range, int damage)
    {
        Vector2 direction;
        if (m_FacingRight) direction = Vector2.right;
        else direction = Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range);
        if (hit.transform.gameObject.layer == 12) hit.transform.gameObject.GetComponent<dummyController>().takeDamage(damage);
    }

    private void stopInvisible()
    {
        invisible = false;
        if(invisRoutine !=null) StopCoroutine(invisRoutine);
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}

