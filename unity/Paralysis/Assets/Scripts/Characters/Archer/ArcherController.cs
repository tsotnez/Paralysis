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

        basicAttack1_var = new RangedSkill(AnimationController.AnimatorStates.BasicAttack1, false, new Vector2(9, 0), standartArrowPrefab, delay_BasicAttack1, damage_BasicAttack1, Skill.skillEffect.nothing, 0, stamina_BasicAttack1, true, cooldown_BasicAttack1, 6);

        skill1_var = new RangedSkill(AnimationController.AnimatorStates.Skill1, false, new Vector2(9, 0), greatArrowPrefab, delay_Skill1, damage_Skill1, Skill.skillEffect.knockback, 0, stamina_Skill1, true, cooldown_Skill1, 7);
        skill2_var = new Skill(AnimationController.AnimatorStates.Skill2, cooldown_Skill2);
        skill3_var = new MeleeSkill(AnimationController.AnimatorStates.Skill3, delay_Skill3, damage_Skill3, Skill.skillEffect.stun, 3, stamina_Skill3, false, cooldown_Skill3, 3);
        skill4_var = new Skill(AnimationController.AnimatorStates.Skill4, cooldown_Skill4);
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
                doRangeSkill(ref animCon.trigBasicAttack1, (RangedSkill) basicAttack1_var);
            else if(doubleJumped) //jump Attack
                doRangeSkill(ref animCon.trigJumpAttack, new RangedSkill(0, false, new Vector2(4, -9), jumpAttackArrowPrefab, 0.2f, damage_BasicAttack1, Skill.skillEffect.nothing, 0, stamina_BasicAttack1, true , 0, 100, false));
        }
    }

    public override void skill1()
    {
        //Shoot a stronger arrow, causing knockback
        doRangeSkill(ref animCon.trigSkill1, (RangedSkill) skill1_var);
    }

    public override void skill2()
    {
        if (canPerformAction(true) && canPerformAttack() && skill2_var.notOnCooldown && stats.loseStamina(stamina_Skill2))
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
        trapScript.creator = gameObject;
        trapScript.damage = damage_Skill2;
        trapScript.whatToHit = m_whatToHit;
        trapScript.ready = true;
        StartCoroutine(setSkillOnCooldown(skill2_var));
    }

    public override void skill3()
    {
        //Stomp the ground, stunning everyone in a given radius
        doMeeleSkill(ref animCon.trigSkill3, (MeleeSkill) skill3_var);
    }

    public override void skill4()
    {
        if (canPerformAction(true) && canPerformAttack() && skill4_var.notOnCooldown && stats.loseStamina(stamina_Skill4))
        {
            placeTrap();
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
        StartCoroutine(setSkillOnCooldown(skill4_var));
    }
}
