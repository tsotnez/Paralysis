using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinBullet : MonoBehaviour {

    public float speed = 7; //Max Speed of the bullet
    public int direction = 0; //Direction to travel in, -1 if left, 1 if right
    
    // Use this for initialization
	void Update () {
        if (direction == -1) GetComponent<SpriteRenderer>().flipX = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed * direction, 0);
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<CharacterStats>().takeDamage(20, false);
            Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
