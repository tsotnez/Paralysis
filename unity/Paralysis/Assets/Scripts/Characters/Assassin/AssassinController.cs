using System.Collections;
using UnityEngine;

public class AssassinController : ChampionClassController
{
    [Header("Assassin Specific")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private int ambushAttack_damage = 13;

    #region default

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<AssassinAnimationController>();

        // Instantiate skill variables
        //basicAttack1_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        //basicAttack2_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.BasicAttack2, delay_BasicAttack2, damage_BasicAttack2, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack2, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack2, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        //basicAttack3_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.BasicAttack3, delay_BasicAttack3, damage_BasicAttack3, Skill.SkillEffect.bleed, 6, 1, stamina_BasicAttack3, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack3, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);

        //skill1_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.Skill1, delay_Skill1, damage_Skill1, Skill.SkillEffect.nothing, 0, 0, stamina_Skill1, Skill.SkillTarget.SingleTarget, cooldown_Skill1, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        //skill2_var = new Skill(ChampionAndTrinketDatabase.Keys.Skill2, ChampionAndTrinketDatabase.Champions.Alchemist, cooldown_Skill2);
        //skill3_var = new Skill(ChampionAndTrinketDatabase.Keys.Skill3, ChampionAndTrinketDatabase.Champions.Alchemist, cooldown_Skill3);
        //skill4_var = new RangedSkill(ChampionAndTrinketDatabase.Keys.Skill4, true, new Vector2(7, 0), bulletPrefab, delay_Skill4, damage_Skill4, Skill.SkillEffect.knockback, 2, 0, stamina_Skill4, Skill.SkillTarget.SingleTarget, cooldown_Skill4, 5, ChampionAndTrinketDatabase.Champions.Alchemist);
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
                DoMeleeSkill(ref animCon.trigSkill3, new MeleeSkill(0, skill3_var.delay, ambushAttack_damage, Skill.SkillEffect.nothing, 3, 0, basicAttack1_var.staminaCost, Skill.SkillTarget.SingleTarget, 0, GameConstants.MeeleRange, "ÄNDERN!"));
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
        DoMeleeSkill(ref animCon.trigSkill1, (MeleeSkill)skill1_var);
    }

    /// <summary>
    /// Vanish and become invisible
    /// </summary>
    public override void Skill2()
    {
        if (CanPerformAction(true) && CanPerformAttack() && skill2_var.notOnCooldown && stats.LoseStamina(skill2_var.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill2_var.type));
            if (stats.invisible) stats.StopInvisible();
            StartCoroutine(SetSkillOnCooldown(skill2_var));
            animCon.trigSkill2 = true;
            StartCoroutine(Skill2Routine());
        }
    }

    /// <summary>
    /// Starts the Invis routine after a appropiat delay
    /// </summary>
    IEnumerator Skill2Routine()
    {
        yield return new WaitForSeconds(skill2_var.delay);
        stats.StartInvisible(5);
    }

    /// <summary>
    /// Teleport to closest enemy on same height and deal damage
    /// </summary>
    public override void Skill3()
    {
        if (CanPerformAction(true) && CanPerformAttack() && skill3_var.notOnCooldown && stats.LoseStamina(skill3_var.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill3_var.type));
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
        DoRangeSkill(ref animCon.trigSkill4, (RangedSkill)skill4_var);
    }

    #endregion

    #region Teleport Skill

    private IEnumerator ShadowStepHit()
    {
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
            yield return new WaitForSeconds(skill3_var.delay);
            stats.DealDamage(targetStats, skill3_var.damage, false);

            yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimationTypes.Skill3);
            stats.immovable = false;
        }
        stats.invincible = false;
        StartCoroutine(SetSkillOnCooldown(skill3_var));
    }

    #endregion

}