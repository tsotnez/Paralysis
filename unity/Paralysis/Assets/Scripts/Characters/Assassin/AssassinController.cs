using System.Collections;
using UnityEngine;

public class AssassinController : ChampionClassController
{
    #region default

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<AssassinAnimationController>();
    }

    protected override void Update()
    {
        base.Update();
        if ((stats.knockedBack || stats.stunned) && stats.invisible) stats.StopInvisible();
    }

    #endregion

    #region Basic Attack & Skills

    public override void BasicAttack()
    {
        if (CanPerformAttack())
        {
            if (animCon.propGrounded && stats.invisible)
            {
                // do ambush attack
                DoMeleeSkill(ref animCon.trigSkill3, Skill.SkillType.Skill3);
                // reset Combo
                AbortCombo();
                // end invisibility
                stats.StopInvisible();
            }
            else
            {
                DoComboBasicAttack(ref animCon.trigBasicAttack1, ref animCon.trigBasicAttack2, ref animCon.trigBasicAttack3);
            }
        }
    }

    /// <summary>
    /// Stun attack
    /// </summary>
    public override void Skill1()
    {
        if (stats.invisible) stats.StopInvisible();
        DoMeleeSkill(ref animCon.trigSkill1, Skill.SkillType.Skill1);
    }

    /// <summary>
    /// Vanish and become invisible
    /// </summary>
    public override void Skill2()
    {
        MeleeSkill skill2 = (MeleeSkill)getSkillByType(Skill.SkillType.Skill2);
        if (CanPerformAction(true) && CanPerformAttack() && skill2.notOnCooldown && stats.LoseStamina(skill2.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill2.type));
            if (stats.invisible) stats.StopInvisible();
            StartCoroutine(SetSkillOnCooldown(skill2));
            animCon.trigSkill2 = true;
            StartCoroutine(Skill2Routine());
        }
    }

    /// <summary>
    /// Starts the Invis routine after a appropiat delay
    /// </summary>
    IEnumerator Skill2Routine()
    {
        yield return new WaitForSeconds(getSkillByType(Skill.SkillType.Skill2).delay);
        stats.StartInvisible(5);
    }

    /// <summary>
    /// Teleport to closest enemy on same height and deal damage
    /// </summary>
    public override void Skill3()
    {
        MeleeSkill skill3 = (MeleeSkill)getSkillByType(Skill.SkillType.Skill3);
        if (CanPerformAction(true) && CanPerformAttack() && skill3.notOnCooldown && stats.LoseStamina(skill3.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill3.type));
            if (stats.invisible) stats.StopInvisible();
            StartCoroutine(ShadowStepHit());
        }
    }

    /// <summary>
    /// Shoot pistol and knock back
    /// </summary>
    public override void Skill4()
    {
        if (stats.invisible) stats.StopInvisible();
        DoRangeSkill(ref animCon.trigSkill4, Skill.SkillType.Skill4);
    }

    #endregion

    #region Teleport Skill

    private IEnumerator ShadowStepHit()
    {
        MeleeSkill skill3 = (MeleeSkill)getSkillByType(Skill.SkillType.Skill3);
        animCon.trigSkill2 = true;
        stats.invincible = true;

        yield return new WaitUntil(() => animCon.CurrentAnimation == AnimationController.AnimationTypes.Skill2);
        yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimationTypes.Skill2); //Wait until intro animation is finished

        //Add walls and Ground to layermask so they are an obstacle for the raycast
        LayerMask temp = m_whatToHit;
        temp |= (1 << LayerMask.NameToLayer("Walls"));
        temp |= (1 << LayerMask.NameToLayer("Ground"));

        int targetLocation = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1000f, temp); //Right hit
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1000f, temp); //Left hit

        bool hitIsEnemy = false;
        if (hit == true)
            hitIsEnemy = m_whatToHit == (m_whatToHit | (1 << hit.collider.gameObject.layer)); //Check if hits layer is in WhatToHit

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
                targetStats.StartStunned(2);
                if (!FacingRight) Flip();
            }
            else if (targetLocation == -1)
            {
                m_Rigidbody2D.position = hitLeft.transform.position + Vector3.right; //Viable target on the left
                target = hitLeft.collider.gameObject;
                targetStats = target.GetComponent<CharacterStats>();
                targetStats.StartStunned(2);
                if (FacingRight) Flip();
            }
            m_Rigidbody2D.velocity = Vector2.zero;
            animCon.trigSkill3 = true;
            yield return new WaitForSeconds(skill3.delay);
            stats.DealDamage(targetStats, skill3.damage, false);

            yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimationTypes.Skill3);
            stats.immovable = false;
        }
        stats.invincible = false;
        StartCoroutine(SetSkillOnCooldown(skill3));
    }

    #endregion

}