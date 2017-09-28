using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    //Kommentar

    [SerializeField]
    Transform target;
    [SerializeField]
    float offsetX = 0;
    [SerializeField]
    float offsetY = 0;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(target.position.x + offsetX, target.position.y + offsetY, transform.position.z);
	}

    public void changeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
