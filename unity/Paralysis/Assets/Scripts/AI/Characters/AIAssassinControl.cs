using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAssassinControl : AIUserControl {

    private bool canShadowStep = true;

    protected override float getCloseRangeAttackDistance()
    {
        return 1.5f;
    }

    protected override float getMediumDistanceAttackDistance()
    {
        return 4f;
    }

    protected override float getLongRangeAttackDistance()
    {
        return 7f;
    }

    protected override float getMinDistanceToNode() {
        return .115f;
    }

    #region OverrideEvents

    protected override TRIGGER_GOALS closeRangeAttack()
    {
        if (canPerformAttack(true) && facingTarget)
        {
            if (checkShadowStep(yDiff) || basicMelee())
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
        }
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS mediumRangeAttack()
    {
        if (canPerformAttack(true) && facingTarget)
        {
            if (checkShadowStep(yDiff) || checkShoot(yDiff))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
        }
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS longRangeAttack()
    {
        if (canPerformAttack(true) && facingTarget)
        {
            if (checkShoot(yDiff) || checkShadowStep(yDiff))
            {
                return TRIGGER_GOALS.WAIT_FOR_INPUT;
            }
        }
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    public override TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth, bool retreating)
    {
        if (newHealth < 20 && newHealth < targetHealth)
        {
            //TODO retreat?
        }
        //DO nothing
        return TRIGGER_GOALS.CONTINUE;
    }


    protected override TRIGGER_GOALS lockedOnToTarget()
    {
        //Check vanish if we locked on...
        MeleeSkill vanish = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill2);
        if (vanish.notOnCooldown && vanish.staminaCost <= charStats.CurrentStamina)
        {
            inputSkill2 = true;
            return setTriggerWait(vanish.delay);
        }

        return TRIGGER_GOALS.CONTINUE;
    }

    public override TRIGGER_GOALS lowStaminaAttacking()
    {
        if (charStats.CurrentHealth < targetStats.CurrentHealth)
        {
            return retreatUntilStamina(50);
        }
        else
        {
            return TRIGGER_GOALS.CONTINUE;
        }
    }

    public override TRIGGER_GOALS IncommingProjectile(float distRight, float distLeft) {
        //66% chance
        if (Random.Range(0, 3) < 2)
        {
            if (distLeft < .5f || distRight < .5f)
            {
                return TRIGGER_GOALS.CONTINUE;
            }
            if (distLeft < 1.5f || distRight < 1.5f)
            {
                return setTriggerBlock(.75f);
            }
            else 
            {
                return setTriggerBlock(1f);
            }
        }
        return TRIGGER_GOALS.CONTINUE;
    }

    #endregion

    #region CheckAndPerformSkills

    private bool checkShadowStep(float yDiff)
    {
        if (Mathf.Abs(yDiff) < .5f)
        {
            //Invisiable dash
            MeleeSkill shadowStep = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill3);
            if (shadowStep.notOnCooldown && shadowStep.staminaCost <= charStats.CurrentStamina && canShadowStep)
            {
                inputSkill3 = true;
                setTriggerWait(shadowStep.delay);
                return true;
            }
        }
        return false;
    }

    private bool checkShoot(float yDiff)
    {
        if (Mathf.Abs(yDiff) < .5f)
        {
            //Shoot GUN
            RangedSkill gunSkill = champClassCon.GetRangeSkillByType(Skill.SkillType.Skill4);
            if (gunSkill.notOnCooldown && gunSkill.staminaCost <= charStats.CurrentStamina)
            {
                inputSkill4 = true;
                setTriggerWait(gunSkill.delay);
                //Don't dash right after shooting
                canShadowStep = false;
                Invoke("setCanDash", 3f);
                return true;
            }
        }
        return false;
    }


    private bool basicMelee()
    {
        //Basic attack one
        MeleeSkill basicAttack = champClassCon.GetMeleeSkillByType(Skill.SkillType.BasicAttack1);
        if (basicAttack.notOnCooldown && basicAttack.staminaCost <= charStats.CurrentStamina)
        {
            inputAttack = true;
            setTriggerWait(basicAttack.delay);
            return true;
        }
        return false;
    }
    #endregion

    private void setCanDash()
    {
        canShadowStep = true;
    }
}
