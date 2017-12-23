﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTrapBehaviour : MonoBehaviour {

    Transform groundCheck;
    public LayerMask m_WhatIsGround;
    public LayerMask whatToHit;
    public GameObject explosionPrefab;
    public GameObject creator;

    bool grounded = false;
    float accelaration = 1;

    public bool ready = false;

    public int damage;

    // Use this for initialization
	void Start () {
        groundCheck = transform.Find("groundCheck").transform;
        StartCoroutine(die());
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (ready)
        {
            grounded = false;

            // The trap is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, .02f, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    grounded = true;
                }
            }

            if (!grounded)
            {
                //Fall, if not grounded
                transform.Translate(Vector3.down * Time.deltaTime * accelaration);
                accelaration *= 1.01f;
            }
            else
                accelaration = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When an Object collides, check if it is in a Layer which can be hit. If so, deal damage
        if (ready)
        {
            if (whatToHit == (whatToHit | (1 << collision.gameObject.layer)))
            {
                CharacterStats targetStats = collision.gameObject.GetComponent<CharacterStats>();
                targetStats.StartStunned(2f);
                creator.GetComponent<CharacterStats>().DealDamage(targetStats, damage, true);
                Camera.main.GetComponent<CameraBehaviour>().startShake();
                Explode();
            }
        }
    }

    void Explode()
    {
        //Plays explosion effect and destroys the bullet
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator die()
    {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }

}
