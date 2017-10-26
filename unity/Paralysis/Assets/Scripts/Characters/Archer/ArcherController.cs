using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{
    public GameObject standartArrowPrefab;
    public override void basicAttack(bool shouldAttack)
    {
        if(shouldAttack)
            doRangeSkill(ref trigBasicAttack1, delay_BasicAttack1, standartArrowPrefab, 5, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1, 9);
    }

    public override void skill1()
    {

    }

    public override void skill2()
    {
    }

    public override void skill3()
    {

    }

    public override void skill4()
    {

    }

    #region Character specific animation

    protected override bool additionalAnimationCondition(AnimationController animCon)
    {
        return false;
    }

    protected override bool additionalNotInterruptCondition(AnimationController.AnimatorStates activeAnimation)
    {
        return false;
    }

    #endregion
}
