using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{
<<<<<<< HEAD
    public GameObject arrowPrefab;

    public override void basicAttack(bool shouldAttack)
    {
        if (shouldAttack)
        {
            doRangeSkill(ref trigBasicAttack1, delay_BasicAttack1, arrowPrefab, 5, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1);
        }
=======
    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<ArcherAnimationController>();
    }

    public override void basicAttack(bool shouldAttack)
    {
        if(shouldAttack) animCon.trigBasicAttack1 = true;
>>>>>>> 20f64c2f6b510f4ab7ecc418c5e91cc78d5dabe0
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
}
