using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlchemistControl : AIUserControl {

    private bool canDash = true;

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
        return .3f;
    }

    #region OverrideEvents

    protected override TRIGGER_GOALS closeRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        float absDistance = Mathf.Abs(distance);
        if (champClassCon.CanPerformAttack())
        {
            if (absDistance <= 3 && checkCloseAOE(yDiff))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (facingTarget && absDistance < 5 && checkMeltedStone(yDiff))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (facingTarget && absDistance <= 7 && checkFrostBolt(yDiff))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else if (facingTarget && absDistance <= 7 && checkBasicAttack(yDiff))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS mediumRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        return closeRangeAttack(facingTarget, distance, yDiff, myStats, targetStats);
    }

    protected override TRIGGER_GOALS longRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        return closeRangeAttack(facingTarget, distance, yDiff, myStats, targetStats);
    }

    public override TRIGGER_GOALS healthDecreasedTenPercent(bool facingTarget, int oldHealth, int newHealth, int targetHealth, RaycastHit2D rightWallRay, RaycastHit2D leftWallRay, bool retreating)
    {
        if (checkTeleport(rightWallRay, leftWallRay))
        {
            return TRIGGER_GOALS.WAIT_FOR_INPUT;
        }
        return TRIGGER_GOALS.CONTINUE;
    }

    protected override TRIGGER_GOALS lockedOnToTarget(CharacterStats myStats, CharacterStats targetStats)
    {
        return TRIGGER_GOALS.CONTINUE;
    }

    public override TRIGGER_GOALS lowStamina(int currentStamina, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats){

        if (myStats.CurrentHealth < targetStats.CurrentHealth)
        {
            retreatDuration = 9999;
            retreatUntilStamina = 50;
            return TRIGGER_GOALS.RETREAT;
        }
        else
        {
            return TRIGGER_GOALS.CONTINUE;
        }
    }

    #endregion

    #region CheckAndPerformSkills

    private bool checkCloseAOE(float yDiff)
    {
        if (Mathf.Abs(yDiff) <= 2.5)
        {
            //Invisiable dash
            MeleeSkill closeRangeAOE = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill4);
            if (closeRangeAOE.notOnCooldown && closeRangeAOE.staminaCost <= charStats.CurrentStamina && canDash)
            {
                inputSkill4 = true;
                triggerWait = closeRangeAOE.delay;
                return true;
            }
        }
        return false;
    }

    private bool checkTeleport(RaycastHit2D rightWallRay, RaycastHit2D leftWallRay)
    {
        //if ()
        {
            MeleeSkill teleport = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill2);
            if (teleport.notOnCooldown && teleport.staminaCost <= charStats.CurrentStamina)
            {
                inputSkill2 = true;
                triggerWait = teleport.delay;
                return true;
            }
        }
        return false;
    }

    private bool checkBasicAttack(float yDiff)
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

    private bool checkFrostBolt(float yDiff)
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

    private bool checkMeltedStone(float yDiff)
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

    #endregion
}
