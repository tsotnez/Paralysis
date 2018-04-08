using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlchemistControl : AIUserControl {
    
    private float timeSinceLastDodge = 0f;
    private const float DODGE_CD = 6;
    private const float DODGE_DURATION = 2f;

    protected override int getLowStamiaTrigger()
    {
        //Just enough stamina to do a jump
        return champClassCon.stamina_Jump + 5;
    }

    protected override float getCloseRangeAttackDistance()
    {
        return 7;
    }

    protected override float getMediumDistanceAttackDistance()
    {
        return 7f;
    }

    protected override float getLongRangeAttackDistance()
    {
        return 7f;
    }

    protected override float getMinDistanceToNode() {
        return .35f;
    }

    #region OverrideEvents

    protected override TRIGGER_GOALS closeRangeAttack()
    {
        if (canPerformAttack(true))
        {
            if (targetDistance <= 3 && checkCloseAOE())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (targetDistance <= 3 && checkTeleport(true, true))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (targetDistance <= 3 && Time.time - timeSinceLastDodge > DODGE_CD && checkDodge(true, true, DODGE_DURATION))
            {
                timeSinceLastDodge = Time.time;
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (facingTarget && targetDistance <= 5 && checkMeltedStone())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (facingTarget && targetDistance <= 7 && checkFrostBolt())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (facingTarget && targetDistance <= 7 && checkBasicAttack())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS mediumRangeAttack()
    {
        return closeRangeAttack();
    }

    protected override TRIGGER_GOALS longRangeAttack()
    {
        return closeRangeAttack();
    }

    public override TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth, RaycastHit2D rightWallRay, RaycastHit2D leftWallRay, bool retreating)
    {
        float rightWall = rightWallRay.distance;
        float leftWall = leftWallRay.distance;

        if (rightWall > 8) rightWall = 8;
        if (leftWall > 8) leftWall = 8;

        bool targetOnRight = false;
        bool goLeft = false;
        bool goRight = false;

        //target on right, not left
        if ((facingTarget && facingDirection == 1) || (!facingTarget && facingDirection == -1))
        {
            targetOnRight = true;
        }

        if (leftWall < rightWall && targetOnRight) goLeft = true;
        else if (rightWall > leftWall && !targetOnRight) goRight = true;
        else
        {
            goRight = !targetOnRight;
            goLeft = targetOnRight;
        }

        if (checkTeleport(goRight, goLeft))
        {
            return TRIGGER_GOALS.WAIT_FOR_INPUT;
        }
        else if (checkDodge(goRight, goLeft, DODGE_DURATION))
        {
            return TRIGGER_GOALS.WAIT_FOR_INPUT;
        }
        else
        {
            retreatDuration = 5;
            retreatUntilStamina = 99999;
            return TRIGGER_GOALS.RETREAT;
        }

        return TRIGGER_GOALS.CONTINUE;
    }

    protected override TRIGGER_GOALS lockedOnToTarget()
    {
        return TRIGGER_GOALS.CONTINUE;
    }

    public override TRIGGER_GOALS lowStamina(int currentStamina)
    {
        //if (charStats.CurrentHealth < targetStats.CurrentHealth)
        //{
            retreatDuration = 9999;
            retreatUntilStamina = 50;
            return TRIGGER_GOALS.RETREAT;
        //}
        //else
        //{
        //    return TRIGGER_GOALS.CONTINUE;
        //}
    }

    #endregion

    #region CheckAndPerformSkills

    private bool checkCloseAOE()
    {
        if (Mathf.Abs(yDiff) <= 2.5)
        {
            //Invisiable dash
            MeleeSkill closeRangeAOE = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill4);
            if (closeRangeAOE.notOnCooldown && closeRangeAOE.staminaCost <= charStats.CurrentStamina)
            {
                inputSkill4 = true;
                triggerWait = closeRangeAOE.delay;
                return true;
            }
        }
        return false;
    }

    private bool checkTeleport(bool goRight, bool goLeft)
    {
        MeleeSkill teleport = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill2);
        if (teleport.notOnCooldown && teleport.staminaCost <= charStats.CurrentStamina)
        {
            inputSkill2 = true;
            if (goRight)
            {
                inputMove = 1;
            }
            else
            {
                inputMove = -1;
            }
            triggerWait = teleport.delay;
            return true;
        }

        return false;
    }

    private bool checkBasicAttack()
    {
        if (Mathf.Abs(yDiff) < .5f)
        {
            //Basic attack one
            RangedSkill basicAttack = champClassCon.GetRangeSkillByType(Skill.SkillType.BasicAttack1);
            if (basicAttack.notOnCooldown && basicAttack.staminaCost <= charStats.CurrentStamina)
            {
                inputAttack = true;
                triggerWait = basicAttack.delay;
                return true;
            }
        }
        return false;
    }

    private bool checkFrostBolt()
    {
        if (Mathf.Abs(yDiff) < .5f)
        {
            //Basic attack one
            RangedSkill basicAttack = champClassCon.GetRangeSkillByType(Skill.SkillType.Skill1);
            if (basicAttack.notOnCooldown && basicAttack.staminaCost <= charStats.CurrentStamina)
            {
                inputSkill1 = true;
                triggerWait = basicAttack.delay;
                return true;
            }
        }
        return false;
    }

    private bool checkMeltedStone()
    {
        if (Mathf.Abs(yDiff) < .5f)
        {
            //Basic attack one
            RangedSkill meltedStone = champClassCon.GetRangeSkillByType(Skill.SkillType.Skill3);
            if (meltedStone.notOnCooldown && meltedStone.staminaCost <= charStats.CurrentStamina)
            {
                inputSkill3 = true;
                triggerWait = meltedStone.delay;
                return true;
            }
        }
        return false;
    }

    #endregion
}
