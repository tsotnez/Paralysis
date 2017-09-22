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
    Coroutine attackingRoutine = null;
    [Header("Attack Delays")]
    [SerializeField]
    private float delay_BasicAttack1;
    [SerializeField]
    private float delay_BasicAttack2;
    [SerializeField]
    private float delay_BasicAttack3;
    [SerializeField]
    private float delay_StunAttack;
    [SerializeField]
    private float delay_ShadowStep;
    [SerializeField]
    private float delay_AmbushAttack;
    [SerializeField]
    private float delay_Shoot;

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

    /// <summary>
    /// Stun attack
    /// </summary>
    public override void skill1()
    {
        if (!attacking)
        {
            if (invisible) stopInvisible();
            attackingRoutine = StartCoroutine(setAttacking(attackLength[4]));
            m_Anim.SetTrigger("stunAttack");
            Invoke("stunAttackHit", delay_StunAttack); //Invoke damage function
        }
    }

    /// <summary>
    /// Vanish and become invisible
    /// </summary>
    public override void skill2()
    {
        if (!attacking)
        {
            if (invisible) stopInvisible();
            attackingRoutine = StartCoroutine(setAttacking(attackLength[5]));
            m_Anim.SetTrigger("vanish");
            if (invisRoutine != null) StopCoroutine(invisRoutine);
            invisRoutine = StartCoroutine(manageInvisibility());
        }
    }

    /// <summary>
    /// Teleport to closest enemy on same height and deal damage
    /// </summary>
    public override void skill3()
    {
        if (!attacking)
        {
            if (invisible) stopInvisible();
            shadowStepHit();
            attackingRoutine = StartCoroutine(setAttacking(attackLength[6]));
            m_Anim.SetTrigger("shadowstep");
        }
    }

    /// <summary>
    /// Shoot pistol and knock back
    /// </summary>
    public override void skill4()
    {
        if (!attacking)
        {
            if (invisible) stopInvisible();
            attackingRoutine = StartCoroutine(setAttacking(attackLength[7]));
            m_Anim.SetTrigger("shoot");
            Invoke("shootAttackHit", delay_Shoot);
        }
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
                    Invoke("basicAttackHit", delay_BasicAttack1);
                    break;
                case 2:
                    m_Anim.SetTrigger("Attack2");
                    if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[1] - 0.08f));
                    Invoke("basicAttackHit", delay_BasicAttack2);
                    break;
                case 3:
                    m_Anim.SetTrigger("Attack3");
                    if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[2] - 0.08f));
                    Invoke("bleedAttackHit", delay_BasicAttack3);
                    inCombo = false;
                    attackCount = 0;
                    timer.timerStop();
                    timer.reset();
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if the basic attack hit anything
    /// </summary>
    private RaycastHit2D tryToHit(float range)
    {
        Vector2 direction;// Direction to check in
        if (m_FacingRight) direction = Vector2.right;

        else direction = Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, whatToHit); //Send raycast
        return hit;
    }

    private void basicAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        if(hit == true) hit.transform.gameObject.GetComponent<CharacterStats>().takeDamage(9); //Let the hit character take damage
    }

    private void bleedAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        CharacterStats target;
        if (hit == true)
        {
            target = hit.transform.gameObject.GetComponent<CharacterStats>();
            target.startBleeding(10);
            target.takeDamage(9);
            target.GetComponent<Rigidbody2D>().AddForce(Vector2.right);
        }
    }

    private void shootAttackHit()
    {
        RaycastHit2D hit = tryToHit(3f);
        if (hit == true) hit.transform.gameObject.GetComponent<CharacterStats>().startKnockBack(transform.position);
    }

    private void stunAttackHit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        CharacterStats target;
        if (hit == true)
        {
            target = hit.transform.gameObject.GetComponent<CharacterStats>();
            target.startStunned(3);
            target.takeDamage(9);
        }
    }

    private void shadowStepHit()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right,  1000f, whatToHit);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1000f, whatToHit);

        if(hit == true && hitLeft == true)
        {
            Debug.Log("both hit");
            if (hit.distance < hitLeft.distance)
            {
                //Teleport to target on the right
                transform.position = hit.transform.position + Vector3.left;
            }
            else
            {
                //Teleport to target on the left
                transform.position = hitLeft.transform.position + Vector3.right;
            }
        }
        else if(hit == true && hitLeft == false) transform.position = hit.transform.position + Vector3.left;
        else if(hit == false && hitLeft == true) transform.position = hitLeft.transform.position + Vector3.right;

        CharacterStats target;

        if(hit == true)
        {

        }
    }

    private void stopInvisible()
    {
        invisible = false;
        if(invisRoutine !=null) StopCoroutine(invisRoutine);
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}

