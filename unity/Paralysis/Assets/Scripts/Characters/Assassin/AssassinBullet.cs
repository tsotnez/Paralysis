using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinBullet : MonoBehaviour {

    public float speed = 7; //Max Speed of the bullet
    public int direction = 0; //Direction to travel in, -1 if left, 1 if right
    public GameObject explosionPrefab;
    public float range = 5; //Distance to travel before dying
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position; //Save starting position
    }

    // Use this for initialization
    void Update () {
        if (direction == -1) GetComponent<SpriteRenderer>().flipX = true; //Flip sprite if necessary
        if (Vector2.Distance(startPos, transform.position) >= range) explode(); //Explode if max range is reached
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed * direction, 0);
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        //On collision, check if collider is enemy.
        if (collision.gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<CharacterStats>().startKnockBack(transform.position);
            collision.gameObject.GetComponent<CharacterStats>().takeDamage(20, false);
        }
        explode();
    }

    void explode()
    {
        //Plays explosion effect and destroys the bullet
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
