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
    [SerializeField]
    private int ambushAttack_damage = 13;

    private bool doubleJumped = false;                                    // Has the character double jumped already?
    public bool invisible = false;
    Coroutine invisRoutine = null;

    #region default

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<AssassinAnimationController>();

        //Instantiate skill variables
        basicAttack1_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack1 , delay_BasicAttack1, damage_BasicAttack1, Skill.skillEffect.nothing, 0, stamina_BasicAttack1, true, cooldown_BasicAttack1, meeleRange);
        basicAttack2_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack2, delay_BasicAttack2, damage_BasicAttack2, Skill.skillEffect.nothing, 0, stamina_BasicAttack2, true, cooldown_BasicAttack2, meeleRange);
        basicAttack3_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack3, delay_BasicAttack3, damage_BasicAttack3, Skill.skillEffect.bleed, 6, stamina_BasicAttack3, true, cooldown_BasicAttack3, meeleRange);

        skill1_var = new MeleeSkill(AnimationController.AnimatorStates.Skill1, delay_Skill1, damage_Skill1, Skill.skillEffect.stun, 3, stamina_Skill1, true, cooldown_Skill1, meeleRange);
        skill2_var = new Skill(AnimationController.AnimatorStates.Skill2, cooldown_Skill2);
        skill3_var = new Skill(AnimationController.AnimatorStates.Skill3, cooldown_Skill3);
        skill4_var = new RangedSkill(AnimationController.AnimatorStates.Skill4, true, new Vector2(7, 0), bulletPrefab, delay_Skill4 , damage_Skill4, Skill.skillEffect.knockback, 2, stamina_Skill4, true, cooldown_Skill4, 5);
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

    #endregion

    #region Skills

    /// <summary>
    /// Stun attack
    /// </summary>
    public override void skill1()
    {
        if (invisible) stopInvisible();
        doMeeleSkill(ref animCon.trigSkill1, (MeleeSkill) skill1_var);
    }

    /// <summary>
    /// Vanish and become invisible
    /// </summary>
    public override void skill2()
    {
        if (canPerformAction(true) && canPerformAttack() && skill2_var.notOnCooldown && stats.loseStamina(stamina_Skill2))
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
        if (canPerformAction(true) && canPerformAttack() && skill3_var.notOnCooldown && stats.loseStamina(stamina_Skill3))
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
        doRangeSkill(ref animCon.trigSkill4, (RangedSkill) skill4_var);
    }

    #endregion

    #region Basic Attack

    public override void basicAttack(bool shouldAttack)
    {
        if (shouldAttack && canPerformAttack() && !dashing)
        {
            if (!animCon.m_Grounded && !invisible && doubleJumped) //Jump attack only when double jumped
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
                        doMeeleSkill(ref animCon.trigBasicAttack1, (MeleeSkill) basicAttack1_var);
                        break;
                    case 2:
                        // do basic attack
                        doMeeleSkill(ref animCon.trigBasicAttack2, (MeleeSkill) basicAttack2_var);
                        break;
                    case 3:
                        // do bleed attack
                        doMeeleSkill(ref animCon.trigBasicAttack3, (MeleeSkill) basicAttack3_var);
                        //reset Combo
                        abortCombo();
                        break;
                    case 4:
                        // do ambush attack
                        doMeeleSkill(ref animCon.trigSkill3, new MeleeSkill(0, delay_Skill3, ambushAttack_damage, Skill.skillEffect.nothing, 3, stamina_BasicAttack1, true, 0, meeleRange));
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
        else if (!animCon.m_Grounded && jump && !doubleJumped && canPerformAction(false) && canPerformAttack())
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
        animCon.trigSkill2 = true;
        stats.invincible = true;

        yield return new WaitUntil(() => animCon.currentAnimation == AnimationController.AnimatorStates.Skill2);
        yield return new WaitUntil(() => animCon.currentAnimation != AnimationController.AnimatorStates.Skill2); //Wait until intro animation is finished

        //Add walls and Ground to layermask so they are an obstacle for the raycast
        LayerMask temp = m_whatToHit;
        temp |= (1 << LayerMask.NameToLayer("Walls"));
        temp |= (1 << LayerMask.NameToLayer("Ground"));

        int targetLocation = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1000f, temp); //Right hit
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1000f, temp); //Left hit

        bool hitIsEnemy = false;
        if (hit == true)
          hitIsEnemy =  m_whatToHit == (m_whatToHit | (1 << hit.collider.gameObject.layer)); //Check if hits layer is in WhatToHit

        bool hitLeftIsEnemy = false;
        if (hitLeft == true)
            hitLeftIsEnemy = m_whatToHit == (m_whatToHit | (1 << hitLeft.collider.gameObject.layer));

        if (hitLeftIsEnemy && hitIsEnemy)
        {
            //There is a target on both sides, check for distance
            if (hit.distance <= hitLeft.distance) targetLocation = 1; //Teleport to target on the right   
            else targetLocation = -1; //Teleport to target on the left
        }
        else if (hitIsEnemy) targetLocation = 1;
        else if (hitLeftIsEnemy) targetLocation = -1;
        else yield return null;

        if (targetLocation != 0)
        {
            stats.immovable = true;
            GameObject target = null;
            CharacterStats targetStats = null;

            if (targetLocation == 1)
            {
                m_Rigidbody2D.position = hit.transform.position + Vector3.left; //Viable target on the right
                target = hit.collider.gameObject;
                targetStats = target.GetComponent<CharacterStats>();
                targetStats.startStunned(3);
                if (!m_FacingRight) Flip();
            }
            else if (targetLocation == -1)
            {
                m_Rigidbody2D.position = hitLeft.transform.position + Vector3.right; //Viable target on the left
                target = hitLeft.collider.gameObject;
                targetStats = target.GetComponent<CharacterStats>();
                targetStats.startStunned(3);
                if (m_FacingRight) Flip();
            }
            m_Rigidbody2D.velocity = Vector2.zero;
            stats.invincible = false;
            animCon.trigSkill3 = true;
            yield return new WaitForSeconds(delay_Skill3);
            targetStats.takeDamage(damage_Skill3, false);

            yield return new WaitUntil(() => animCon.currentAnimation != AnimationController.AnimatorStates.Skill3);
            stats.immovable = false;
        }
        StartCoroutine(setSkillOnCooldown(skill3_var));
    }

    #endregion

    #region Invisibility

    private IEnumerator manageInvisibility()
    {
        yield return new WaitForSeconds(delay_Skill2);
        invisible = true;
        StartCoroutine(setSkillOnCooldown(skill2_var));

        // set animation
        animCon.trigSkill2 = true;

        // set transparency
        Color oldCol = transform.Find("graphics").GetComponent<SpriteRenderer>().color;
        oldCol.a = 0.5f;
        transform.Find("graphics").GetComponent<SpriteRenderer>().color = oldCol;

        yield return new WaitForSeconds(5);
        stopInvisible();
    }

    private void stopInvisible()
    {
        invisible = false;
        if (invisRoutine != null) StopCoroutine(invisRoutine);
        transform.Find("graphics").GetComponent<SpriteRenderer>().color = Color.white;
    }

    #endregion
}

