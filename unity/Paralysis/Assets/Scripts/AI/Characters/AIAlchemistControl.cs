using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlchemistControl : AIUserControl {
    
    private float timeSinceLastDodge = 0f;
    private float timeSinceLastTooClose = 0f;

    private const float DODGE_CD = 6;
    private const float DODGE_DURATION = 1.5f;
    private const float ENEMY_TOO_CLOSE_CD = 4f;

    protected override float enemyTooCloseDistance()
    {
        return 1f;
    }

    protected override float getCloseRangeAttackDistance()
    {
        return 6.5f;
    }

    protected override float getMediumDistanceAttackDistance()
    {
        return 6.5f;
    }

    protected override float getLongRangeAttackDistance()
    {
        return 6.5f;
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
            else if (checkDodgeOrTeleport())
            {
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

    public override TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth, bool retreating)
    {
        if (isRetreating)
        {
            if (targetDistance <= 3 && checkCloseAOE())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
            else
            {
                return TRIGGER_GOALS.CONTINUE;
            }
        }
        else
        {
            //TODO....
            if (targetDistance < 3 && checkDodgeOrTeleport())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            } 
            else
            {
                return retreatForDuration(5);
            }
        }
    }

    protected override TRIGGER_GOALS lockedOnToTarget()
    {
        return TRIGGER_GOALS.CONTINUE;
    }

    public override TRIGGER_GOALS lowStaminaAttacking()
    {
        if(targetStunned) return TRIGGER_GOALS.CONTINUE;

        if (currentHealth <= targetStats.CurrentHealth)
        {
            return retreatUntilStamina(50);
        }
        else if (currentStamina < targetStats.CurrentStamina)
        {
            return retreatUntilStamina(50);
        }
        else
        {
            return TRIGGER_GOALS.CONTINUE;
        }
    }

    public override TRIGGER_GOALS enemyTooCloseAttacking()
    {
        if (Time.time > timeSinceLastTooClose + ENEMY_TOO_CLOSE_CD)
        {
            timeSinceLastTooClose = Time.time;
            return retreatForDuration(2);
        }
        else
        {
            return TRIGGER_GOALS.CONTINUE;
        }
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

    public bool checkDodgeOrTeleport()
    {
        if (canPerformAttack(true) && targetDistance <= 3 && !targetStunned)
        {
            MeleeSkill teleport = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill2);
            if (teleport.notOnCooldown && teleport.staminaCost <= charStats.CurrentStamina)
            {
                DODGE_DIR dodge = getDodgeDirection();

                inputSkill2 = true;
                if (dodge.goRight)
                {
                    inputMove = 1;
                } else
                {
                    inputMove = -1;
                }
                triggerWait = teleport.delay;

                return true;
            } 
            else if (Time.time - timeSinceLastDodge > DODGE_CD && currentStamina >= champClassCon.m_dashStaminaCost)
            {
                DODGE_DIR dodge = getDodgeDirection();
                inputDash(dodge.goRight, dodge.goRight);
                timeSinceLastDodge = Time.time;
                triggerWait = DODGE_DURATION;
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    #endregion
}
