using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    //Kommentar

    public float magnitude = 0.05f;
    public float duration = 0.01f;
    [SerializeField]
    Transform target;
    [SerializeField]
    float offsetX = 0;
    [SerializeField]
    float offsetY = 0;

    private Coroutine shakingRoutine;


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

    IEnumerator Shake()
    {

        float elapsed = 0.0f;

        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            Camera.main.transform.position += new Vector3(x, y, 0);

            yield return null;
        }
    }

    public void startShake()
    {
        if (shakingRoutine != null) StopCoroutine(shakingRoutine);
        shakingRoutine = StartCoroutine(Shake());

    }
}
