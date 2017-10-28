using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : ChampionClassController
{
    public GameObject standartArrowPrefab;
    public GameObject greatArrowPrefab;
    public GameObject trapPrefab;

    #region default

    // Use this for initialization
    void Start()
    {
        animCon = graphics.GetComponent<ArcherAnimationController>();
    }

    #endregion

    public override void basicAttack(bool shouldAttack)
    {
        //Shoot a basic arrow 
        if(shouldAttack)
            doRangeSkill(ref animCon.trigBasicAttack1, delay_BasicAttack1, standartArrowPrefab, 5, damage_BasicAttack1, skillEffect.nothing, 0, stamina_BasicAttack1, 9);
    }

    public override void skill1()
    {
        //Shoot a stronger arrow, causing knockback
        doRangeSkill(ref animCon.trigSkill1, delay_Skill1, greatArrowPrefab, 7, damage_Skill1, skillEffect.knockback, 1, stamina_Skill1, 9);
    }

    public override void skill2()
    {
        //Puts down a trap
        animCon.trigSkill2 = true;
        Invoke("placeTrap", delay_Skill2);
    }

    private void placeTrap()
    {
        Instantiate(trapPrefab, transform.position, Quaternion.identity);
    }

    public override void skill3()
    {
        //Stomp the ground, stunning everyone in a given radius
        doMeeleSkill(ref animCon.trigSkill3, delay_Skill3, damage_Skill3, skillEffect.stun, 3, stamina_Skill3, false, 3);
    }

    public override void skill4()
    {

    }
}
