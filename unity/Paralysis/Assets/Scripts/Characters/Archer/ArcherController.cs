using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{
    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<ArcherAnimationController>();
    }

    public override void basicAttack(bool shouldAttack)
    {
        if(shouldAttack) animCon.trigBasicAttack1 = true;
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
