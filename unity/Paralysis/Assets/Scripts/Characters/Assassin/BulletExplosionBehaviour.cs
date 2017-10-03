using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosionBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("die", 0.4f);
	}
	
    void die()
    {
        Destroy(gameObject);
    }
}
