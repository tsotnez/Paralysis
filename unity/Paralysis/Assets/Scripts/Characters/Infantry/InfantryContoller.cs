using System;
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
    void Start()
    {
        animCon = graphics.GetComponent<InfantryAnimationController>();

        basicAttack1_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, meeleRange);
        basicAttack3_var = new MeleeSkill(AnimationController.AnimatorStates.BasicAttack2, delay_BasicAttack2, damage_BasicAttack2, Skill.SkillEffect.nothing, 0, stamina_BasicAttack2, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack2, meeleRange);

        skill1_var = new Skill(AnimationController.AnimatorStates.Skill1, delay_Skill1, damage_Skill1, Skill.SkillEffect.stun, 3, stamina_Skill1, Skill.SkillTarget.SingleTarget, cooldown_Skill1, 10f);
        skill2_var = new MeleeSkill(AnimationController.AnimatorStates.Skill2, delay_Skill2, damage_Skill2, Skill.SkillEffect.stun, 3, stamina_Skill2, Skill.SkillTarget.InFront, cooldown_Skill2, meeleRange);
        skill3_var = new MeleeSkill(AnimationController.AnimatorStates.Skill3, delay_Skill3, damage_Skill3, Skill.SkillEffect.nothing, 0, stamina_Skill3, Skill.SkillTarget.MultiTarget, cooldown_Skill3, meeleRange);
        skill4_var = new MeleeSkill(AnimationController.AnimatorStates.Skill4, delay_Skill4, damage_Skill4, Skill.SkillEffect.knockback, 0, stamina_Skill4, Skill.SkillTarget.InFront, cooldown_Skill4, meeleRange);
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

    #region BasicAttack

    /// <summary>
    /// Attack combo. 1 normal hit, 2 strong hits
    /// 
    /// normal hit: 5 dmg
    /// strong hit: 7 dmg
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void BasicAttack(bool shouldAttack)
    {
        if (shouldAttack && CanPerformAction(false) && CanPerformAttack())
        {
            if (animCon.m_Grounded)
            {
                // Check if enough stamina for attack
                if (stats.HasSufficientStamina(stamina_BasicAttack1) && (attackCount == 0) || //Basic Attack
                    stats.HasSufficientStamina(stamina_BasicAttack2) && (attackCount == 1 || attackCount == 2)) // Strong Attack
                {
                    // Already in combo?
                    if (!inCombo)
                    {
                        // First attack - initialize combo coroutine
                        ResetComboTime();
                        attackCount = 0;
                    }

                    // AttackCount increase per attack
                    attackCount++;

                    // Playing the correct animation depending on the attackCount and setting attacking status
                    switch (attackCount)
                    {
                        case 1:
                            // do meele attack
                            DoMeleeSkill(ref animCon.trigBasicAttack1, (MeleeSkill)basicAttack1_var);
                            // Reset timer of combo
                            ResetComboTime();
                            break;
                        case 2:
                            // do meele attack
                            DoMeleeSkill(ref animCon.trigBasicAttack2, (MeleeSkill)basicAttack3_var);
                            break;
                        case 3:
                            // do meele attack
                            DoMeleeSkill(ref animCon.trigBasicAttack2, (MeleeSkill)basicAttack3_var);
                            // Reset Combo after combo-hit
                            AbortCombo();
                            break;
                        default:
                            // Should not be triggered
                            AbortCombo();
                            break;

                    }
                }
            }
            // Jump attack only when falling
            else
            {
                // Check if enough stamina is left
                if (stats.LoseStamina(stamina_JumpAttack))
                {
                    // Jump Attack
                    StartCoroutine(JumpAttack());
                    // Abort combo
                    AbortCombo();
                }
            }
        }
    }

    #endregion

    #region Skills

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
        if (CanPerformAction(true) && CanPerformAttack() && skill2_var.notOnCooldown && stats.LoseStamina(skill2_var.staminaCost))
        {
            StartCoroutine(DoSkill1_Hook());
        }
    }

    private IEnumerator DoSkill1_Hook()
    {
        stats.immovable = true;
        skill1_hook = true;

        // Try to find a target
        RaycastHit2D hit = TryToHit(skill1_var.range);
        if (hit)
        {
            // get the target and stun it
            CharacterStats target = hit.transform.gameObject.GetComponent<CharacterStats>();
            target.StartStunned();

            // Throw chain-hook to target
            float DistanceToTarget = hit.distance;

            // Calculate correct direction
            if (!m_FacingRight)
                skill1_hook_speed = Math.Abs(skill1_hook_speed) * (-1);
            else
                skill1_hook_speed = Math.Abs(skill1_hook_speed);

            // Add a vertical force to the player.
            animCon.trigSkill1 = true;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.AddForce(new Vector2(0, (68 * DistanceToTarget))); // Distance 5.8f --> AddForce 400f --> 68 * Distance

            // Wait until force is applied
            yield return new WaitUntil(() => !animCon.m_Grounded);
            // Wait while not on ground
            yield return new WaitUntil(() => animCon.m_Grounded);
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