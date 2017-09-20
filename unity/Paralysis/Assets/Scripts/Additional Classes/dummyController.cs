using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dummyController : MonoBehaviour {

    public int maxHealth = 100;
    public int currentHealth;
    Rigidbody2D rigid;
    Text textAboveHead;

    // Use this for initialization
    void Awake () {
        currentHealth = maxHealth;
        rigid = GetComponent<Rigidbody2D>();
        textAboveHead = GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        textAboveHead.text = currentHealth + "/" + maxHealth;
	}

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) die();
    }

    private void die()
    {
        Destroy(gameObject);
    }
}
