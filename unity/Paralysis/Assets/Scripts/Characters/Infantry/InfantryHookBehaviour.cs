﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class InfantryHookBehaviour : ProjectileBehaviour
{
    public CharacterStats targetStats = null;
    public int hitted = 0;

    float DistanceToCreator = 0;
    float LengthOfChain = 0;

    protected new void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject != creator)
        {
            //On collision, check if collider is in whatToHit layermask
            if (whatToHit == (whatToHit | (1 << collision.gameObject.layer)))
            {
                targetStats = collision.gameObject.GetComponent<CharacterStats>();
                hitted = 1;               
            }
            else
            {
                hitted = -1;
            }
            StartCoroutine(GetStuck());
        }
    }

    protected new void Update()
    {
        base.Update();

        DistanceToCreator = Math.Abs(this.transform.position.x - startPos.x);
        if (DistanceToCreator-LengthOfChain >= 100)
        {
            // Generate new ChainElement

            // Increase length of chain
        }
    }
}
