using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{
    public GameObject standartArrowPrefab;
    public GameObject jumpAttackArrowPrefab;
    public GameObject greatArrowPrefab;
    public GameObject trapPrefab;

    private bool disengaging = false; //True while performing the disengage skill
    private Coroutine disengageRoutine = null;
    private float disengageSpeed = 15;
    #region default

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<ArcherAnimationController>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Move while disengaging
        if(disengaging)
        {
            m_Rigidbody2D.velocity = new Vector2(disengageSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion

    public override void basicAttack(bool shouldAttack)
    {
        //Shoot a basic arrow 
        if (shouldAttack)
        {
            if(animCon.m_Grounded)
                doRangeSkill(ref animCon.trigBasicAttack1, delay_BasicAttack1, standartArrowPrefab, 5, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1, new Vector2(9, 0));
            else //jump Attack
                doRangeSkill(ref animCon.trigJumpAttack, 0.2f, jumpAttackArrowPrefab, 100, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1, new Vector2(11, -9), false ,false);
        }
    }

    public override void skill1()
    {
        //Shoot a stronger arrow, causing knockback
        doRangeSkill(ref animCon.trigSkill1, delay_Skill1, greatArrowPrefab, 7, damage_Skill1, skillEffect.knockback, 0, stamina_Skill1, new Vector2(9, 0));
    }

    public override void skill2()
    {
        if (canPerformAction(true) && canPerformAttack() && stats.loseStamina(stamina_Skill2))
        {
            //Puts down a trap
            animCon.trigSkill2 = true;
            Invoke("placeTrap", delay_Skill2);
        }
    }

    private void placeTrap()
    {
        GameObject trap = Instantiate(trapPrefab, m_GroundCheck.position, Quaternion.identity);
        ArcherTrapBehaviour trapScript = trap.GetComponent<ArcherTrapBehaviour>();
        trapScript.damage = damage_Skill2;
        trapScript.whatToHit = m_whatToHit;
        trapScript.ready = true;
    }

    public override void skill3()
    {
        //Stomp the ground, stunning everyone in a given radius
        doMeeleSkill(ref animCon.trigSkill3, delay_Skill3, damage_Skill3, skillEffect.stun, 3, stamina_Skill3, false, 3);
    }

    public override void skill4()
    {
        if (canPerformAction(true) && canPerformAttack() && stats.loseStamina(stamina_Skill4))
        {
            //Jump back while being invincible
            if (disengageRoutine != null)
                StopCoroutine(disengageRoutine);
            disengageRoutine = StartCoroutine(disengage());
        }
    }

    private IEnumerator disengage()
    {
        animCon.trigSkill4 = true;
        stats.immovable = true;

        //Determing direction to disengage in
        float direction;
        if (m_FacingRight)
            direction = -1;
        else
            direction = 1;

        disengageSpeed = Mathf.Abs(disengageSpeed) * direction;

        disengaging = true;
        stats.invincible = true;

        yield return new WaitUntil(() => animCon.currentAnimation == AnimationController.AnimatorStates.Skill4);
        //Wait until skill Animation is over
        yield return new WaitUntil(() => animCon.currentAnimation != AnimationController.AnimatorStates.Skill4);
        stats.invincible = false;
        disengaging = false;
        stats.immovable = false;
    }
}
