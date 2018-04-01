using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAssassinControl : AIUserControl {

    private float timeShotGun = 0f;

    protected override int getLowStamiaTrigger()
    {
        //Just enough stamina to do a jump
        return champClassCon.stamina_Jump + 5;
    }

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
        return 8f;
    }

    protected override TRIGGER_GOALS closeRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        if (champClassCon.CanPerformAttack())
        {
            if (facingTarget)
            {
                //Basic attack one
                MeleeSkill basicAttack = champClassCon.GetMeleeSkillByType(Skill.SkillType.BasicAttack1);
                if (basicAttack.notOnCooldown && basicAttack.staminaCost <= charStats.CurrentStamina)
                {
                    inputAttack = true;
                    triggerWait = basicAttack.delay;
                    return TRIGGER_GOALS.WAIT_FOR_ATTACK;
                }
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS mediumRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        if (champClassCon.CanPerformAttack() && facingTarget)
        {
            if (Mathf.Abs(yDiff) < .1f)
            {
                //Invisiable dash
                MeleeSkill shadowStep = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill3);

                if (shadowStep.notOnCooldown && shadowStep.staminaCost <= charStats.CurrentStamina && Time.time + 3 > timeShotGun)
                {
                    inputSkill3 = true;
                    triggerWait = shadowStep.delay;
                    return TRIGGER_GOALS.WAIT_FOR_ATTACK;
                }
            }

            //Poison blades
            /*
                MeleeSkill poisonBlades = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill1);
                if (poisonBlades.notOnCooldown && poisonBlades.staminaCost <= charStats.CurrentStamina)
                {
                    inputSkill1 = true;
                    triggerWait = poisonBlades.delay;
                    return TRIGGER_GOALS.WAIT_FOR_ATTACK;
                }
                */
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS longRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        if (champClassCon.CanPerformAttack() && facingTarget)
        {
            if (Mathf.Abs(yDiff) < .5f)
            {
                //Shoot GUN
                RangedSkill gunSkill = champClassCon.GetRangeSkillByType(Skill.SkillType.Skill4);
                if (gunSkill.notOnCooldown && gunSkill.staminaCost <= charStats.CurrentStamina)
                {
                    inputSkill4 = true;
                    triggerWait = gunSkill.delay;
                    timeShotGun = Time.time;
                    return TRIGGER_GOALS.WAIT_FOR_ATTACK;
                }
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    public override TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth, RaycastHit2D rightWallRay, RaycastHit2D leftWallRay){

        if (newHealth < 20 && newHealth < targetHealth)
        {
            //TODO retreat?
        }
        //DO nothing
        return TRIGGER_GOALS.CONTINUE;
    }


    protected override TRIGGER_GOALS lockedOnToTarget(CharacterStats myStats, CharacterStats targetStats)
    {
        //Check vanish if we locked on...
        MeleeSkill vanish = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill2);
        if (vanish.notOnCooldown && vanish.staminaCost <= charStats.CurrentStamina)
        {
            inputSkill2 = true;
            triggerWait = vanish.delay;
            return TRIGGER_GOALS.WAIT_FOR_ATTACK;
        }

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

}
