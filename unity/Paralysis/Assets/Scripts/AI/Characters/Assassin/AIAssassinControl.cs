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

    protected override DISTANCE_RET closeRangeAttack(bool facingTarget, float distance, float yDiff)
    {
        if (champClassCon.CanPerformAttack())
        {
            MeleeSkill skill = champClassCon.GetMeleeSkillByType(Skill.SkillType.BasicAttack1);
            if (skill.notOnCooldown && skill.staminaCost <= charStats.CurrentStamina)
            {
                inputAttack = true;
                distWaitTime = .5f;
                return DISTANCE_RET.WAIT;
            }
        }

        return DISTANCE_RET.MOVE_CLOSER;
    }

    protected override DISTANCE_RET mediumRangeAttack(bool facingTarget, float distance, float yDiff)
    {
        if (champClassCon.CanPerformAttack())
        {
            if (Mathf.Abs(yDiff) < .5f)
            {
                MeleeSkill skill = champClassCon.GetMeleeSkillByType(Skill.SkillType.Skill3);
                if (skill.notOnCooldown && skill.staminaCost <= charStats.CurrentStamina)
                {
                    inputSkill3 = true;
                    distWaitTime = 1f;
                    return DISTANCE_RET.WAIT;
                }
            }
        }

        return DISTANCE_RET.MOVE_CLOSER;
    }

    protected override DISTANCE_RET longRangeAttack(bool facingTarget, float distance, float yDiff)
    {
        if (champClassCon.CanPerformAttack())
        {
            if (Mathf.Abs(yDiff) < 1.5f)
            {
                RangedSkill skill = champClassCon.GetRangeSkillByType(Skill.SkillType.Skill4);
                if (skill.notOnCooldown && skill.staminaCost <= charStats.CurrentStamina)
                {
                    inputSkill4 = true;
                    distWaitTime = .5f;
                    return DISTANCE_RET.WAIT;
                }
            }
        }

        return DISTANCE_RET.MOVE_CLOSER;
    }
}
