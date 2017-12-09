using System;
using System.Collections;
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
    float LengthOfHook = 0;

    bool setValuesForFalling = false;

    protected new void Start()
    {
        base.Start();

        ChainElements = new List<GameObject>();
        LengthOfHook = this.GetComponent<Renderer>().bounds.size.x;
        LengthOfChainElement = ChainPrefab.GetComponent<Renderer>().bounds.size.x;

        ProjectileRigid.freezeRotation = true;
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

    protected new IEnumerator GetStuck()
    {
        // Stop moving of all chain elements
        foreach (GameObject ChainElement in ChainElements)
        {
            Rigidbody2D rigChainElement = ChainElement.GetComponent<Rigidbody2D>();
            rigChainElement.velocity = new Vector2(0, 0); //Stop moving
            rigChainElement.gravityScale = 1;
        }
        yield return base.GetStuck();
    }

    protected override void Die()
    {
        // Destory every ChainElement
        foreach (GameObject ChainElement in ChainElements)
        {
            Destroy(ChainElement.gameObject);
        }

        // Destroy Hook
        base.Die();
    }

    protected new void Update()
    {
        base.Update();

        if (hitted == 0 && !falling)
        {
            ChainBehaviour();
        }
        else if (falling && !setValuesForFalling)
        {
            ProjectileRigid.freezeRotation = false;
            ProjectileRigid.constraints = RigidbodyConstraints2D.None;
            setValuesForFalling = false;
        }
    }

    private void ChainBehaviour()
    {
        DistanceToCreator = Math.Abs(this.transform.position.x - startPos.x);
        if (DistanceToCreator - LengthOfChain >= LengthOfChainElement)
        {
            Vector3 newPos;
            float prevObjLength;
            if (ChainElements.Count == 0)
            {
                // Hook Values
                newPos = this.transform.position;
                newPos.y += 0.013f;
                prevObjLength = (LengthOfHook / 1.55f);
            }
            else
            {
                // Chain Values
                newPos = ChainElements.Last().transform.position;
                prevObjLength = LengthOfChainElement;
            }

            // Calculate next chain element position 
            newPos.x += (prevObjLength * direction * -1) + (0.001f * direction);

            // Instantiate new ChainElement
            GameObject ChainElement = Instantiate(ChainPrefab, newPos, ChainPrefab.transform.rotation);

            // Flip chain element if direction is left
            if (direction == -1) Flip(ChainElement);

            if (ChainElements.Count == 0)
            {
                // If the List is empty, its the first chain element and needs to be connected to the hook
                ChainElement.GetComponent<HingeJoint2D>().connectedBody = ProjectileRigid;
            }
            else
            {
                // Connect last chain element with the new element
                ChainElement.GetComponent<HingeJoint2D>().connectedBody = ChainElements.Last().GetComponent<Rigidbody2D>();
            }
            ChainElements.Add(ChainElement);

            // Increase length of chain
            LengthOfChain += LengthOfChainElement;
        }
    }
}
