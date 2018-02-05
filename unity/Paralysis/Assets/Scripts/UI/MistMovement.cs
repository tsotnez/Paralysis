using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the mist onject in loading screen.
/// </summary>
public class MistMovement : MonoBehaviour {

    public float Speed = 3f;
    public GameObject mistPrefab;

    private Rect screenRect;
    private RectTransform trans;
    private GameObject MainCanvas;
    private bool madeSuccessor = false;

    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        MainCanvas = GameObject.Find("MainCanvas");
        trans = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3[] corners = new Vector3[4];
        trans.GetWorldCorners(corners);

        trans.position = new Vector3(trans.position.x + Speed * Time.deltaTime, trans.position.y, trans.position.z); // move

        if (screenRect.Contains(Camera.main.WorldToScreenPoint(corners[0])) && !madeSuccessor) //Instantiate copy when left border reached screen rect
        {
            GameObject newMist = Instantiate(gameObject, MainCanvas.transform, false);
            trans.GetWorldCorners(corners);
            newMist.transform.position = new Vector3(corners[0].x, newMist.transform.position.y, newMist.transform.position.z);
            madeSuccessor = true;
        }

        if (madeSuccessor && !screenRect.Contains(Camera.main.WorldToScreenPoint(corners[0]))) //Destroy when put off screen
            Destroy(gameObject);
	}
}


