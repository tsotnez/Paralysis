using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dummyController : MonoBehaviour {

	CharacterStats stats;
	Text textAboveHead;

	// Use this for initialization
	void Awake()
	{
		stats = GetComponent<CharacterStats>();
		textAboveHead = GetComponentInChildren<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		textAboveHead.text = stats.currentHealth + "/" + stats.maxHealth;
	}
}
