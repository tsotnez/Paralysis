﻿using System;
using System.Collections;
using UnityEngine;

public class AlchemistController : ChampionClassController
{
    #region default

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<AlchemistAnimationController>();
    }

    #endregion

    #region BasicAttack and Skills

    /// <summary>
    /// Only Basic Attack
    /// </summary>
    public override void BasicAttack()
    {
        if (animCon.propGrounded) // Basic Attack
            DoRangeSkill(ref animCon.trigBasicAttack1, Skill.SkillType.BasicAttack1);
    }

    /// <summary>
    /// Alchemist casts a frost bolt at a fixed distance and altitute.
    /// The first enemy hit by this effect will have a 50% decreased movement speed.
    /// 
    /// Damage: 5
    /// Effect: slow
    /// Cast time: 1.5s
    /// Stamina: 15
    /// </summary>
    public override void Skill1()
    {
        DoRangeSkill(ref animCon.trigSkill1, Skill.SkillType.Skill1);
    }

    /// <summary>
    /// Alchemist teleports in the direction that he is moving or dashing.
    /// If standing still, teleports in the direction he is facing.
    /// If jumping, teleports on top platform.
    /// If ducking, teleports on lower platform.
    /// 
    /// Cooldown: 15s
    /// Stamina: 15
    /// </summary>
    public override void Skill2()
    {
        MeleeSkill skill2 = GetMeleeSkillByType(Skill.SkillType.Skill2);
        if (CanPerformAction(false) && CanPerformAttack() && skill2.notOnCooldown && stats.LoseStamina(skill2.staminaCost))
        {
            // Get the direction first --> save the effects on skill enter
            UserControl uc = GetComponent<UserControl>();
            TeleportDirection direction = TeleportDirection.forward;
            // If ducking, teleports on lower platform.
            if (uc.XYAxis.y < 0)
            {
                direction = TeleportDirection.down;
            }
            // If jumping, teleports on top platform.
            else if (uc.XYAxis.y > 0 && uc.XYAxis.x == 0)
            {
                direction = TeleportDirection.up;
            }
            // If standing still, teleports in the direction he is facing.
            else
            {
                direction = TeleportDirection.forward;
            }

            StartCoroutine(DoSkill2(direction));
        }
    }

    /// <summary>
    /// Alchemist casts a spell at a fixed distance and altitude.
    /// The first enemy stuck by this will be stunned.
    /// 
    /// Damage: 5
    /// Effect: stun
    /// Cooldown: 15s
    /// Stamina: 15
    /// </summary>
    public override void Skill3()
    {
        DoRangeSkill(ref animCon.trigSkill3, Skill.SkillType.Skill3);
    }

    /// <summary>
    /// Alchemist uses his alchemy to explode and reconstruct himnself.
    /// As he explodes, any enemy within the radius will receive a knockback.
    /// 
    /// Damage: 10
    /// Effect: knockback
    /// Cooldown: 25s
    /// Stamina: 20
    /// </summary>
    public override void Skill4()
    {
        DoMeleeSkill(ref animCon.trigSkill4, Skill.SkillType.Skill4);
    }

    #endregion

    #region Teleport Skill

    private enum TeleportDirection
    {
        up, down, forward
    }

    private IEnumerator DoSkill2(TeleportDirection direction)
    {
        MeleeSkill skill2 = GetMeleeSkillByType(Skill.SkillType.Skill2);
        // trigger animation for getting invisible
        animCon.trigSkill2 = true;

        // the alchemist is invincible while teleporting
        stats.invincible = true;
        stats.immovable = true;
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); // Stop moving

        // Freeze Rigid that it can not fall down while Animation is played
        RigidbodyConstraints2D oldConstraints = m_Rigidbody2D.constraints;
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        // Wait till animation has started and ended so the alchemist is disappeard
        yield return new WaitUntil(() => animCon.CurrentAnimation == AnimationController.AnimationTypes.Skill2);
        yield return new WaitUntil(() => animCon.CurrentAnimationState == AnimationController.AnimationState.Waiting);

        // Build an layermask for finding Walls and Ground
        LayerMask layWallGround = (1 << LayerMask.NameToLayer("Walls"));
        layWallGround |= (1 << LayerMask.NameToLayer("Ground"));
        layWallGround |= (1 << LayerMask.NameToLayer("Through"));

        Vector2 newPos = transform.position;
        if (direction == TeleportDirection.forward)
        {
            // Check if an wall is too near for a ful range teleport
            RaycastHit2D hit;
            if (FacingRight)
                hit = Physics2D.Raycast(transform.position, Vector2.right, skill2.range, layWallGround);
            else
                hit = Physics2D.Raycast(transform.position, Vector2.left, skill2.range, layWallGround);

            if (hit)
            {
                // If a wall is to close teleport to this wall and positioning close to it
                newPos.x = hit.transform.position.x;
                if (FacingRight)
                    newPos.x -= GetComponent<BoxCollider2D>().bounds.size.x + 0.1f;
                else
                    newPos.x += GetComponent<BoxCollider2D>().bounds.size.x + 0.1f;
            }
            else
            {
                // If no wall is in the way, simply teleport
                if (FacingRight)
                    newPos.x += skill2.range;
                else
                    newPos.x -= skill2.range;
            }
        }
        else if (direction == TeleportDirection.up)
        {
            // Search for ground floors over the alchemist
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1000f, layWallGround);

            // If hit and tag is not topborder --> teleport on it else under it
            if (hit)
            {
                if (hit.collider.gameObject.tag != "TopBorder")
                {
                    newPos.y = hit.transform.position.y + (hit.collider.bounds.size.y / 2) + Math.Abs(m_GroundCheck.localPosition.y);
                }
                else if (hit.collider.gameObject.tag == "TopBorder")
                {
                    newPos.y = hit.transform.position.y - (hit.collider.bounds.size.y / 2) - Math.Abs(m_GroundCheck.localPosition.y);
                }
            }
        }
        else if (direction == TeleportDirection.down)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 1000f, layWallGround);

            // First hit ground where alchemist is standing - Second hit next ground
            if (hits.Length > 1)
            {
                newPos.y = hits[1].transform.position.y + (hits[1].collider.bounds.size.y / 2) + Math.Abs(m_GroundCheck.localPosition.y);
            }
        }

        // unfreeze to apply new position
        m_Rigidbody2D.constraints = oldConstraints;

        // Set alchemist to new position and let him stand still
        m_Rigidbody2D.position = newPos;
        m_Rigidbody2D.velocity = Vector2.zero;

        // freeze again to fall not down while appiering
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        // Let the alchemist appear at the new position
        ((AlchemistAnimationController)animCon).trigSkill2End = true;
        yield return new WaitForSeconds(skill2.delay);
        yield return new WaitUntil(() => animCon.CurrentAnimation != AnimationController.AnimationTypes.Skill2);

        // Reapply old constraints to fall down regulary
        m_Rigidbody2D.constraints = oldConstraints;

        // Make the alchemist moveable and hitable again
        stats.immovable = false;
        stats.invincible = false;

        // Start cooldown of the skillw2
        StartCoroutine(SetSkillOnCooldown(skill2));
    }

    #endregion
}