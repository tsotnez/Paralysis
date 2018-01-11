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
    protected override void Start()
    {
        base.Start();
        FacingRight = true;
        animCon = graphics.GetComponent<ArcherAnimationController>();

        //Instantiate skill variables
        basicAttack1_var = new RangedSkill(ChampionAndTrinketDatabase.Keys.BasicAttack1, false, new Vector2(9, 0), standartArrowPrefab, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, 6, ChampionAndTrinketDatabase.Champions.Alchemist);
        jumpAttack_var = new RangedSkill(0, false, new Vector2(4, -9), jumpAttackArrowPrefab, 0.2f, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, 0, 100, ChampionAndTrinketDatabase.Champions.Alchemist, false);

        skill1_var = new RangedSkill(ChampionAndTrinketDatabase.Keys.Skill1, false, new Vector2(9, 0), greatArrowPrefab, delay_Skill1, damage_Skill1, Skill.SkillEffect.knockback, 0, 0, stamina_Skill1, Skill.SkillTarget.SingleTarget, cooldown_Skill1, 7, ChampionAndTrinketDatabase.Champions.Alchemist);
        skill2_var = new Skill(ChampionAndTrinketDatabase.Keys.Skill2, ChampionAndTrinketDatabase.Champions.Alchemist, cooldown_Skill2);
        skill3_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.Skill3, delay_Skill3, damage_Skill3, Skill.SkillEffect.stun, 3, 0, stamina_Skill3, Skill.SkillTarget.MultiTarget, cooldown_Skill3, 3, ChampionAndTrinketDatabase.Champions.Alchemist);
        skill4_var = new Skill(ChampionAndTrinketDatabase.Keys.Skill4, ChampionAndTrinketDatabase.Champions.Alchemist, cooldown_Skill4);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Move while disengaging
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
        if (animCon.m_Grounded)
            DoRangeSkill(ref animCon.trigBasicAttack1, (RangedSkill)basicAttack1_var);
    }

    public override IEnumerator JumpAttack()
    {
        DoRangeSkill(ref animCon.trigJumpAttack, (RangedSkill)jumpAttack_var);
        yield return new WaitForSeconds(0.1f);
    }

    public override void Skill1()
    {
        //Shoot a stronger arrow, causing knockback
        DoRangeSkill(ref animCon.trigSkill1, (RangedSkill)skill1_var);
    }

    public override void Skill2()
    {
        if (CanPerformAction(true) && CanPerformAttack() && skill2_var.notOnCooldown && stats.LoseStamina(stamina_Skill2))
        {
            //Puts down a trap
            hotbar.StartCoroutine(hotbar.flashBlack(skill2_var.type));
            animCon.trigSkill2 = true;
            Invoke("PlaceTrap", delay_Skill2);
        }
    }

    private void PlaceTrap()
    {
        GameObject trap = Instantiate(trapPrefab, m_GroundCheck.position, Quaternion.identity);
        ArcherTrapBehaviour trapScript = trap.GetComponent<ArcherTrapBehaviour>();
        trapScript.creator = gameObject;
        trapScript.damage = damage_Skill2;
        trapScript.whatToHit = m_whatToHit;
        trapScript.ready = true;
        StartCoroutine(SetSkillOnCooldown(skill2_var));
    }

    public override void Skill3()
    {
        //Stomp the ground, stunning everyone in a given radius
        DoMeleeSkill(ref animCon.trigSkill3, (MeleeSkill)skill3_var);
    }

    public override void Skill4()
    {
        if (CanPerformAction(true) && CanPerformAttack() && skill4_var.notOnCooldown && stats.LoseStamina(stamina_Skill4))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill4_var.type));
            PlaceTrap();
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

        yield return new WaitUntil(() => animCon.CurrentAnimation == AnimationController.AnimatorStates.Skill4);
        //Wait until skill Animation is over
        yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimatorStates.Skill4);
        stats.invincible = false;
        disengaging = false;
        stats.immovable = false;
        StartCoroutine(SetSkillOnCooldown(skill4_var));
    }

    #endregion
}