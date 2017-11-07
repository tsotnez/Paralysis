using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(die());
	}

    private IEnumerator die()
    {
        yield return new WaitForSeconds(0.832f);
        Destroy(gameObject);
    }
}
