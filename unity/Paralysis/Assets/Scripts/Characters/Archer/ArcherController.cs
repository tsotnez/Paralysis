using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{
    private bool disengaging = false; //True while performing the disengage skill
    private Coroutine disengageRoutine = null;
    private float disengageSpeed = 15;

    #region default

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<ArcherAnimationController>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Move while disengaging
        if (disengaging)
        {
            m_Rigidbody2D.velocity = new Vector2(disengageSpeed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion

    #region Basic Attack & Skills

    public override void BasicAttack()
    {
        // Shoot a basic arrow 
        if (animCon.propGrounded)
            DoRangeSkill(ref animCon.trigBasicAttack1, Skill.SkillType.BasicAttack1);
    }

    public override IEnumerator JumpAttack()
    {
        DoRangeSkill(ref animCon.trigJumpAttack, Skill.SkillType.JumpAttack);
        yield return new WaitForSeconds(0.1f);
    }

    public override void Skill1()
    {
        // Shoot a stronger arrow, causing knockback
        DoRangeSkill(ref animCon.trigSkill1, Skill.SkillType.Skill1);
    }

    public override void Skill2()
    {
        RangedSkill skill2 = GetRangeSkillByType(Skill.SkillType.Skill2);
        if (CanPerformAction(true) && CanPerformAttack() && skill2.notOnCooldown && stats.LoseStamina(skill2.staminaCost))
        {
            //Puts down a trap
            hotbar.StartCoroutine(hotbar.flashBlack(skill2.type));
            animCon.trigSkill2 = true;
            StartCoroutine(PlaceTrapCoroutine(skill2.delay));
            StartCoroutine(SetSkillOnCooldown(skill2));
        }
    }

    private IEnumerator PlaceTrapCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.offlineMode)
        {
            RPC_PlaceTrap(m_GroundCheck.position);
        }
        else
        {
            photonView.RPC("RPC_PlaceTrap", PhotonTargets.All, m_GroundCheck.position);
        }
    }

    [PunRPC]
    public void RPC_PlaceTrap(Vector3 position)
    {
        RangedSkill skill2 = GetRangeSkillByType(Skill.SkillType.Skill2);
        GameObject trap = Instantiate((skill2).prefab, position, Quaternion.identity);
        ArcherTrapBehaviour trapScript = trap.GetComponent<ArcherTrapBehaviour>();
        trapScript.creator = gameObject;
        trapScript.damage = skill2.damage;
        trapScript.whatToHit = m_whatToHit;
        trapScript.ready = true;
    }

    public override void Skill3()
    {
        //Stomp the ground, stunning everyone in a given radius
        DoMeleeSkill(ref animCon.trigSkill3, Skill.SkillType.Skill3);
    }

    public override void Skill4()
    {
        RangedSkill skill4 = GetRangeSkillByType(Skill.SkillType.Skill4);
        if (CanPerformAction(true) && CanPerformAttack() && skill4.notOnCooldown && stats.LoseStamina(skill4.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill4.type));

            if (PhotonNetwork.offlineMode)
            {
                RPC_PlaceTrap(m_GroundCheck.position);
            }
            else
            {
                photonView.RPC("RPC_PlaceTrap", PhotonTargets.All, m_GroundCheck.position);
            }

            //Jump back while being invincible
            if (disengageRoutine != null)
                StopCoroutine(disengageRoutine);
            disengageRoutine = StartCoroutine(Disengage());
        }
    }

    private IEnumerator Disengage()
    {
        animCon.trigSkill4 = true;
        stats.immovable = true;

        //Determing direction to disengage in
        float direction;
        if (FacingRight)
            direction = -1;
        else
            direction = 1;

        disengageSpeed = Mathf.Abs(disengageSpeed) * direction;

        disengaging = true;
        stats.invincible = true;

        yield return new WaitUntil(() => animCon.CurrentAnimation == AnimationController.AnimationTypes.Skill4);
        //Wait until skill Animation is over
        yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimationTypes.Skill4);
        stats.invincible = false;
        disengaging = false;
        stats.immovable = false;
        StartCoroutine(SetSkillOnCooldown(GetRangeSkillByType(Skill.SkillType.Skill4)));
    }

    #endregion
}