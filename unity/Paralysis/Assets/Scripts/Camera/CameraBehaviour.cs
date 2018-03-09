using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    public Bounds CameraBounds;
    public List<Transform> targets = new List<Transform>();
    public Vector3 offset;
    private Vector3 velocity;
    public float smoothTime = 0.5f;

    public float addSize = .1f;
    public float minOrtoSize = 4f;
    public float maxOrtoSize = 8f;

    public bool gameRunning = true;

    [Header("Camera Shake")]
    public float magnitude = 0.05f;
    public float duration = 0.01f;

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
    private Vector2 startingPosition;

    private Bounds b = new Bounds();

    // Use this for initialization
    void Awake () {
        startingPosition = transform.position;
        cam = GetComponent<Camera>();

        //Get Border Objects
        //leftBorder = GameObject.FindGameObjectWithTag("LeftBorder").transform;
        //rightBorder = GameObject.FindGameObjectWithTag("RightBorder").transform;
        //topBorder = GameObject.FindGameObjectWithTag("TopBorder").transform;
        //bottomBorder = GameObject.FindGameObjectWithTag("BottomBorder").transform;

        CalculateCameraBounds();
        //CalculatePositioningValues();
    }


    // Update is called once per frame
    void LateUpdate ()
    {
        if (targets.Count == 0)
            return;

        //CalculatePositioningValues();
        CalculateCameraBounds();
        Move();
        Zoom();
    }

    private void Zoom()
    {
        //float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatesDistance()) / 2;
        //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);

        float factor = (float)Screen.height / (float)Screen.width;
        float newSize = (GetGreatesDistance() * factor) / 2 + addSize;

        cam.orthographicSize = Mathf.Clamp(newSize, minOrtoSize, maxOrtoSize);
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        if(newPosition.x > maxX)newPosition.x = maxX;
        if(newPosition.x < minX)newPosition.x = minX;
        if(newPosition.y > maxY)newPosition.y = maxY;
        if(newPosition.y < minY)newPosition.y = minY;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private float GetGreatesDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        foreach (Transform target in targets)
            bounds.Encapsulate(target.position);

        return bounds.size.x;
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
            return targets[0].position;
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        foreach (Transform target in targets)
            bounds.Encapsulate(target.position);

        return bounds.center;
    }

    //private void SinglePlayer()
    //{
    //    //clamp new position between min and max values and lerp to that position
    //    Vector3 desiredPos = new Vector3(Mathf.Clamp(target.position.x + offsetX, minX, maxX), Mathf.Clamp(target.position.y + offsetY, minY, maxY), transform.position.z);
    //    transform.position = Vector3.Lerp(transform.position, desiredPos, lerpSpeed * Time.deltaTime);
    //}

    //private void Multiplayer()
    //{
    //    Vector3 middleBetweenPlayers = target.position + ((target2.position- target.position) / 2); //Middle is target's position plus half of the vector leading to target2's position

    //    SetCameraSize();
    //    //Zoom correctly
    //    //float desiredSize =  Mathf.Clamp(Vector3.Distance(target.position, target2.position) * 0.3f, minSize, maxSize);
    //    //cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + ((desiredSize -  cam.orthographicSize) * Time.deltaTime * zoomSpeed), minSize, maxSize);

    //    //clamp new position between min and max values and lerp to that position
    //    Vector3 desiredPos = new Vector3(Mathf.Clamp(middleBetweenPlayers.x + offsetX, minX, maxX), Mathf.Clamp(middleBetweenPlayers.y + offsetY, minY, maxY), transform.position.z);
    //    transform.position = Vector3.Lerp(transform.position, desiredPos, lerpSpeed * Time.deltaTime);
    //}

    //void SetCameraSize()
    //{
    //    //horizontal size is based on actual screen ratio
    //    float minSizeX = minSizeY * Screen.width / Screen.height;
    //    //multiplying by 0.5, because the ortographicSize is actually half the height
    //    float width = Mathf.Abs(target.position.x - target2.position.x) * 0.5f;
    //    float height = Mathf.Abs(target.position.y - target2.position.y) * 0.5f;
    //    //computing the size
    //    float camSizeX = Mathf.Max(width, minSizeX);
    //    cam.orthographicSize += ((Mathf.Max(height, camSizeX * Screen.height / Screen.width, minSizeY)) - cam.orthographicSize) * Time.deltaTime * zoomSpeed;
    //}

    //public void switchToMultiplayer(Transform player2)
    //{
    //    target2 = player2;
    //    multiplayer = true;
    //    GetComponent<Camera>().orthographicSize = 6;

    //    //Re calculate stuff
    //    CalculatePositioningValues();
    //}

    public void AddTargetToCamera(Transform newPlayer)
    {
        targets.Add(newPlayer);
    }

    //public void switchToSingleplayer()
    //{
    //    target2 = null;
    //    multiplayer = false;
    //    cam.orthographicSize = 2.96f;

    //    //Re calculate stuff
    //    CalculatePositioningValues();
    //}

    //private void CalculatePositioningValues()
    //{
    //    //calculate screen size
    //    height = cam.orthographicSize * 2f;
    //    width = height * cam.aspect;
    //    minX = leftBorder.position.x + 0.5f * width;
    //    maxX = rightBorder.position.x - 0.5f * width;
    //    minY = bottomBorder.position.y + 0.5f * height;
    //    maxY = topBorder.position.y - 0.5f * height;
    //}

    //public void changeTarget(Transform newTarget)
    //{
    //    target = newTarget;
    //}

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


    private void CalculateCameraBounds()
    {
        float orthoSize = cam.orthographicSize - 0.2f;
        maxY = startingPosition.y + (CameraBounds.size.y / 2f) - orthoSize;
        minY = startingPosition.y - (CameraBounds.size.y / 2f) + orthoSize;
        maxX = startingPosition.x + (CameraBounds.size.x / 2f) - orthoSize;
        minX = startingPosition.x - (CameraBounds.size.x / 2f) + orthoSize;
    }

    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(CameraBounds.center, CameraBounds.size);
    }
}
