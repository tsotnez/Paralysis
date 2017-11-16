using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    bool multiplayer = false;
    public bool gameRunning = true;

    public float magnitude = 0.05f;
    public float duration = 0.01f;

    [SerializeField]
    float offsetX = 0;
    [SerializeField]
    float offsetY = 0;
    [SerializeField]
    float lerpSpeed = 15f;
    [SerializeField]
    float minSize = 5f;
    [SerializeField]
    float maxSize = 6f;
    [SerializeField]
    float zoomSpeed = 2f;

    Transform target;
    Transform target2;

    private Camera cam;

    //Border objects, used to calculate if space outside of map is visible
    private Transform leftBorder;                                  
    private Transform rightBorder;                                 
    private Transform topBorder;                                   
    private Transform bottomBorder;

    //Minimum and maximum coordinates
    private float minY;
    private float minX;
    private float maxY;
    private float maxX;

    //Screen size
    private float height;
    private float width;                      

    private Coroutine shakingRoutine;


    // Use this for initialization
    void Awake () {
        cam = GetComponent<Camera>();

        //Get Border Objects
        leftBorder = GameObject.FindGameObjectWithTag("LeftBorder").transform;
        rightBorder = GameObject.FindGameObjectWithTag("RightBorder").transform;
        topBorder = GameObject.FindGameObjectWithTag("TopBorder").transform;
        bottomBorder = GameObject.FindGameObjectWithTag("BottomBorder").transform;

        CalculatePositioningValues();
    }


    // Update is called once per frame
    void LateUpdate ()
    {
        if (gameRunning)
        {
            if (multiplayer)
                Multiplayer();
            else
                SinglePlayer();
        }

        CalculatePositioningValues();
    }

    private void SinglePlayer()
    {
        //clamp new position between min and max values and lerp to that position
        Vector3 desiredPos = new Vector3(Mathf.Clamp(target.position.x + offsetX, minX, maxX), Mathf.Clamp(target.position.y + offsetY, minY, maxY), transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, lerpSpeed * Time.deltaTime);
    }

    private void Multiplayer()
    {
        Vector3 middleBetweenPlayers = target.position + ((target2.position- target.position) / 2); //Middle is target's position plus half of the vector leading to target2's position

        //Zoom correctly
        float desiredSize =  Mathf.Clamp(Vector3.Distance(target.position, target2.position) * 0.3f, minSize, maxSize);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + ((desiredSize -  cam.orthographicSize) * Time.deltaTime * zoomSpeed), minSize, maxSize);

        //clamp new position between min and max values and lerp to that position
        Vector3 desiredPos = new Vector3(Mathf.Clamp(middleBetweenPlayers.x + offsetX, minX, maxX), Mathf.Clamp(middleBetweenPlayers.y + offsetY, minY, maxY), transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, lerpSpeed * Time.deltaTime);
    }
    
    public void switchToMultiplayer(Transform player2)
    {
        target2 = player2;
        multiplayer = true;
        GetComponent<Camera>().orthographicSize = 6;

        //Re calculate stuff
        CalculatePositioningValues();
    }

    public void switchToSingleplayer()
    {
        target2 = null;
        multiplayer = false;
        cam.orthographicSize = 2.96f;

        //Re calculate stuff
        CalculatePositioningValues();
    }

    private void CalculatePositioningValues()
    {
        //calculate screen size
        height = cam.orthographicSize * 2f;
        width = height * cam.aspect;
        minX = leftBorder.position.x + 0.5f * width;
        maxX = rightBorder.position.x - 0.5f * width;
        minY = bottomBorder.position.y + 0.5f * height;
        maxY = topBorder.position.y - 0.5f * height;
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

            cam.transform.position += new Vector3(x, y, 0);

            yield return null;
        }
    }

    public void startShake()
    {
        if (shakingRoutine != null) StopCoroutine(shakingRoutine);
        shakingRoutine = StartCoroutine(Shake());
    }
}
