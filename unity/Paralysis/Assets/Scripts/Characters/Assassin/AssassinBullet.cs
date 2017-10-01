using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinBullet : MonoBehaviour {

    public float speed = 7;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
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
