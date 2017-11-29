using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class InfantryHookBehaviour : ProjectileBehaviour
{
    public GameObject ChainPrefab;
    public CharacterStats targetStats = null;
    public int hitted = 0;

    List<GameObject> ChainElements;
    float DistanceToCreator = 0;
    float LengthOfChain = 0;
    float LengthOfChainElement = 0;

    protected new void Start()
    {
        base.Start();

        ChainElements = new List<GameObject>();
        LengthOfChainElement = ChainPrefab.GetComponent<Renderer>().bounds.size.x;
    }

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
        //ChainBehaviour();
    }

    private void ChainBehaviour()
    {
        DistanceToCreator = Math.Abs(this.transform.position.x - startPos.x);
        if (DistanceToCreator - LengthOfChain >= LengthOfChainElement)
        {
            // Calculate next chain element position 
            Vector3 newPos;
            if (ChainElements.Count == 0)
            {
                newPos = this.transform.position;
                newPos.x = this.transform.position.x - this.GetComponent<HingeJoint2D>().anchor.x;
            }
            else
            {
                newPos = ChainElements.Last().transform.position;
                newPos.x = ChainElements.Last().transform.position.x - ChainElements.Last().GetComponent<HingeJoint2D>().anchor.x;
            }

            // Instantiate new ChainElement
            GameObject ChainElement = Instantiate(ChainPrefab, newPos, new Quaternion(ChainPrefab.transform.rotation.x,
            ChainPrefab.transform.rotation.y, ChainPrefab.transform.rotation.z * direction, ChainPrefab.transform.rotation.w));

            if (ChainElements.Count == 0)
            {
                //If the List is empty, its the first chain element and needs to be connected to the hook
                ChainElement.GetComponent<HingeJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            }
            else
            {
                //Connect last element with the new element
                ChainElement.GetComponent<HingeJoint2D>().connectedBody = ChainElements.Last<GameObject>().GetComponent<Rigidbody2D>();
            }
            ChainElements.Add(ChainElement);

            // Increase length of chain
            LengthOfChain += LengthOfChainElement;
        }
    }
}
