using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughPlatform : MonoBehaviour {

    private BoxCollider2D myCollider;
    private List<GameObject> currentlyInside = new List<GameObject>();

    private void Start()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D collider in colliders)
        {
            if(!collider.isTrigger)
            {
                myCollider = collider;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == GameConstants.MAIN_PLAYER_TAG)
        {
            Rigidbody2D rBody2d = collider.GetComponent<Rigidbody2D>();
            if(rBody2d.velocity.y > 0)
            {
                changeCollisions(collider.gameObject, false);
                StartCoroutine(addAndRemove(collider.gameObject));
            }
        }
    }

    /// <summary>
    /// Adds and removes the game object to ignore collisions then to not
    /// this can happen if the circle collider never enters or the player
    /// teleported etc..
    /// </summary>
    /// <returns>The and remove.</returns>
    /// <param name="removeObj">Remove object.</param>
    private IEnumerator addAndRemove(GameObject removeObj)
    {
        currentlyInside.Add(removeObj);
        yield return new WaitForSeconds(.5f);
        if(currentlyInside.Contains(removeObj))
        {
            changeCollisions(removeObj, true);
            currentlyInside.Remove(removeObj);
        }
    }

    private void OnTriggerExit2D(Collider2D  collider)
    {
        if(collider.tag == GameConstants.MAIN_PLAYER_TAG)
        {
            //Only care about the circle collider exiting, that is our ground check.
            if(collider.GetType() == typeof(CircleCollider2D) && currentlyInside.Contains(collider.gameObject))
            {
                changeCollisions(collider.gameObject, true);
                currentlyInside.Remove(collider.gameObject);
            }
        }
    }

    /// <summary>
    /// Helper method that sets all collidiers on a game object ignore this one.
    /// </summary>
    /// <param name="gameObject">Game object.</param>
    /// <param name="collide">If set to <c>true</c> collide.</param>
    private void changeCollisions(GameObject gameObject, bool collide)
    {
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        foreach(Collider2D otherCollider in colliders)
        {                    
            Physics2D.IgnoreCollision(myCollider, otherCollider, !collide);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D collider in colliders)
        {
            Gizmos.color = Color.red;
            if(collider.isTrigger)
            {
                Gizmos.color = Color.green;
            }
            BoxCollider2D bCollider = GetComponent<BoxCollider2D>();
            Gizmos.DrawWireCube(collider.bounds.center, new Vector3(collider.bounds.size.x, collider.bounds.size.y));
        }
    }
}
