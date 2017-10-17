using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{

    public override void basicAttack(bool shouldAttack)
    {
        if(shouldAttack) trigBasicAttack1 = true;
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
