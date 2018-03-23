using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAssassinControl : AIUserControl {

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
            MeleeSkill skill = champClassCon.GetMeleeSkillByType(Skill.SkillType.BasicAttack1);
            if (skill.notOnCooldown && skill.staminaCost <= charStats.CurrentStamina)
            {
                inputAttack = true;
                triggerWait = .5f;
                return TRIGGER_GOALS.WAIT_FOR_ATTACK;
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS mediumRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        if (champClassCon.CanPerformAttack())
        {
            if (Mathf.Abs(yDiff) < .5f)
            {
                MeleeSkill skill = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill3);
                if (skill.notOnCooldown && skill.staminaCost <= charStats.CurrentStamina)
                {
                    inputSkill3 = true;
                    triggerWait = 1f;
                    return TRIGGER_GOALS.WAIT_FOR_ATTACK;
                }
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected override TRIGGER_GOALS longRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        if (champClassCon.CanPerformAttack())
        {
            if (Mathf.Abs(yDiff) < 1.5f)
            {
                RangedSkill skill = champClassCon.GetRangeSkillByType(Skill.SkillType.Skill4);
                if (skill.notOnCooldown && skill.staminaCost <= charStats.CurrentStamina)
                {
                    inputSkill4 = true;
                    triggerWait = .5f;
                    return TRIGGER_GOALS.WAIT_FOR_ATTACK;
                }
            }
        }

        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    public override TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth){
        //DO nothing
        return TRIGGER_GOALS.CONTINUE;
    }
}
