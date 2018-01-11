﻿using System;
using System.Collections;
using UnityEngine;

public class InfantryContoller : ChampionClassController
{
    [SerializeField]
    private GameObject Skill1_Hook;
    [SerializeField]
    private GameObject Skill1_Chain;

    private bool skill1_hook = false;
    private float skill1_hook_speed = 10f;

    #region default

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<InfantryAnimationController>();

        //Instantiate skill variables
        basicAttack1_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, MeeleRange);
        basicAttack2_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack2, delay_BasicAttack2, damage_BasicAttack2, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack2, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack2, MeeleRange);
        basicAttack3_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack3, delay_BasicAttack3, damage_BasicAttack3, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack3, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack3, MeeleRange);

        skill1_var = new Skill(AnimationController.AnimatorStates.Skill1, delay_Skill1, damage_Skill1, Skill.SkillEffect.stun, 3, 0, stamina_Skill1, Skill.SkillTarget.SingleTarget, cooldown_Skill1, 10f);
        skill2_var = new MeleeSkill(AnimationController.AnimatorStates.Skill2, delay_Skill2, damage_Skill2, Skill.SkillEffect.stun, 3, 0, stamina_Skill2, Skill.SkillTarget.InFront, cooldown_Skill2, MeeleRange);
        skill3_var = new MeleeSkill(AnimationController.AnimatorStates.Skill3, delay_Skill3, damage_Skill3, Skill.SkillEffect.nothing, 0, 0, stamina_Skill3, Skill.SkillTarget.MultiTarget, cooldown_Skill3, MeeleRange);
        skill4_var = new MeleeSkill(AnimationController.AnimatorStates.Skill4, delay_Skill4, damage_Skill4, Skill.SkillEffect.knockback, 0, 0, stamina_Skill4, Skill.SkillTarget.InFront, cooldown_Skill4, MeeleRange);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Add force to infantry when leaping (Skill 1)
        if (skill1_hook)
        {
            m_Rigidbody2D.velocity = new Vector2(skill1_hook_speed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion

    #region BasicAttack and Skills

    /// <summary>
    /// Attack combo. 1 normal hit, 2 strong hits
    /// 
    /// normal hit: 5 dmg
    /// strong hit: 7 dmg
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void BasicAttack()
    {
        DoComboBasicAttack(ref animCon.trigBasicAttack1, ref animCon.trigBasicAttack2, ref animCon.trigBasicAttack2);
    }

    /// <summary>
    /// Retract Hook (Knockback)
    /// Infantry launches a hook to a flexibly fixed distance and altitude the hook latches on to the first enemy it comes in contact with. 
    /// The Infantry then repels to that target and kicks, causing a knockback effect to the enemy
    /// 
    /// Damage: 5
    /// Effect: Knockback
    /// Cooldown: 15 sec
    /// Stamina: 10
    /// </summary>
    public override void Skill1()
    {
        // Validate if skill can be performed
        if (CanPerformAction(true) && CanPerformAttack() && skill1_var.notOnCooldown && stats.LoseStamina(skill1_var.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill1_var.name));
            StartCoroutine(DoSkill1_Hook());
        }
    }

    private IEnumerator DoSkill1_Hook()
    {
        stats.immovable = true;

        // calculate direction
        int direction;
        if (FacingRight) direction = 1;
        else direction = -1;

        // generate GameObject
        GameObject goProjectile = Skill1_Hook;
        InfantryHookBehaviour projectile = goProjectile.GetComponent<InfantryHookBehaviour>();

        // assign variables to projectile Script
        projectile.direction = direction;
        projectile.creator = this.gameObject;
        projectile.whatToHit = m_whatToHit;
        projectile.SkillValues = (RangedSkill)skill1_var;
        projectile.ChainPrefab = Skill1_Chain;

        goProjectile = Instantiate(goProjectile, transform.position + new Vector3(1f * direction, 0.3f), new Quaternion(goProjectile.transform.rotation.x,
            goProjectile.transform.rotation.y, goProjectile.transform.rotation.z * direction, goProjectile.transform.rotation.w));

        projectile = goProjectile.GetComponent<InfantryHookBehaviour>();

        yield return new WaitUntil(() => projectile.hitted != 0);

        if (projectile.hitted == 1)
        {
            // get the target and stun it
            CharacterStats target = projectile.targetStats;
            target.StartStunned();

            // Throw chain-hook to target
            float DistanceToTarget = Math.Abs(target.transform.position.x - this.transform.position.x);

            // Calculate correct direction
            if (!FacingRight)
                skill1_hook_speed = Math.Abs(skill1_hook_speed) * (-1);
            else
                skill1_hook_speed = Math.Abs(skill1_hook_speed);

            // Add a vertical force to the player.
            skill1_hook = true;
            animCon.trigSkill1 = true;

            if (DistanceToTarget > 1)
            {
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(new Vector2(0, (68 * DistanceToTarget))); // Distance 5.8f --> AddForce 400f --> 68 * Distance

                // Wait until force is applied
                yield return new WaitUntil(() => !animCon.m_Grounded);
                // Wait while not on ground
                yield return new WaitUntil(() => animCon.m_Grounded);
            }

            ((InfantryAnimationController)animCon).trigSkill1End = true;
            Camera.main.GetComponent<CameraBehaviour>().startShake(); //Shake the camera

            // Do melee hit after landing and remove stuneffect
            stats.DealDamage(target, skill1_var.damage, true);
            target.StopStunned();
        }

        // start cooldown and let the method end
        StartCoroutine(SetSkillOnCooldown(skill1_var));
        skill1_hook = false;
        stats.immovable = false;
    }

    /// <summary>
    /// Ground Break (Stun)
    /// The Infantry strikes the ground with his sword producing a force that can stun any enemys directly in front of him.
    /// 
    /// Damage: 5
    /// Effect: Stun
    /// Cooldown: 20 sec
    /// Stamina: 15
    /// </summary>
    public override void Skill2()
    {
        DoMeleeSkill(ref animCon.trigSkill2, (MeleeSkill)skill2_var);
    }

    /// <summary>
    /// Bladestorm (Damage)
    /// Infantry swings his sword in all directions. Any enemy directly in front or behind the Infantry will take extensive damage
    ///  
    /// Damage: 15
    /// Effect: nothing
    /// Cooldown: 25 sec
    /// Stamina: 20
    /// </summary>
    public override void Skill3()
    {
        DoMeleeSkill(ref animCon.trigSkill3, (MeleeSkill)skill3_var);
    }

    /// <summary>
    /// Hit/Throw (Knockback)
    /// Infantry swings his sword upwards, along with any enemy directly in front of him. This causes a knockback to the enemy.
    /// 
    /// Damage: 10
    /// Effect: Knockback
    /// Cooldown: 20 sec
    /// Stamina: 15
    /// </summary>
    public override void Skill4()
    {
        DoMeleeSkill(ref animCon.trigSkill4, (MeleeSkill)skill4_var);
    }

    #endregion
}