using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {
    GameObject mainCameraObject;
    Camera mainCamera;
    Transform secondaryCameraObject;
    Camera secondaryCamera;

    // Boundaries of scroll space
    float scrollAreaLeft;
    float scrollAreaTop;
    float scrollAreaRight;
    float scrollAreaBottom;
    float scrollSpeed;		// How quickly can we scroll

    // For zooming
    const float maxZoomedIn = 4.0f;
    const float maxZoomedOut = 10.0f;

    // Drunk stuff
    float drunkMultiplier;
    bool swayingRight;

    // For panning
    const float panSpeed = 0.08f;
    Vector2 panVector;
    Vector3 panDestination;
    bool currentlyPanning;
    const float defaultPanAdjust = 1.0f;
    float panAdjustTimer;

    // Use this for initialization
    void Start() {
        mainCameraObject = GameObject.Find("Main Camera");
        if (!mainCameraObject)
        {
            Debug.Log("Error: can't find camera");
        }
        mainCamera = mainCameraObject.GetComponent<Camera>();
        secondaryCameraObject = mainCameraObject.transform.Find("SecondaryCamera");
        if (!secondaryCameraObject)
        {
            Debug.Log("Warning: can't find secondary camera");
        }
        secondaryCamera = secondaryCameraObject.GetComponent<Camera>();

        scrollAreaLeft = 0.0f;
        scrollAreaTop = 20.0f;
        scrollAreaRight = 20.0f;
        scrollAreaBottom = 0.0f;
        scrollSpeed = 4.5f;

        drunkMultiplier = 0.0f;
        swayingRight = true;

        panAdjustTimer = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        if (drunkMultiplier > 0.0f)
        {
            DrunkCam();
        }

        if (currentlyPanning)
        {
            mainCamera.transform.Translate(panVector);

            if (Vector2.Distance(mainCamera.transform.position, panDestination) < 0.2f)
            {
                currentlyPanning = false;
            }

            if (panAdjustTimer > 0.0f)
            {
                panAdjustTimer -= Time.deltaTime;

                if (panAdjustTimer <= 0.0f)
                {
                    panVector = panDestination - mainCamera.transform.position;
                    panVector.Normalize();
                    panVector *= panSpeed;
                    panAdjustTimer = defaultPanAdjust;
                }
            }
        }
    }

    public void Zoom(float amount)
    {
        if (amount > 0.0f)  // Zoom out
        {
            mainCamera.orthographicSize = Mathf.Min(
                mainCamera.orthographicSize + amount, maxZoomedOut);
        }
        else                // Zoom in
        {
            mainCamera.orthographicSize = Mathf.Max(
                mainCamera.orthographicSize + amount, maxZoomedIn);
        }
    }

    public void Scroll(Vector2 direction)
    {
        currentlyPanning = false;
        var normalizedDirection = direction.normalized;

        // If at far left or far right side of screen
        if ((normalizedDirection.x < 0 && mainCamera.transform.position.x <= scrollAreaLeft)
            || (normalizedDirection.x > 0 && mainCamera.transform.position.x >= scrollAreaRight))
        {
            normalizedDirection.x = 0.0f;
        }
        if ((normalizedDirection.y < 0 && mainCamera.transform.position.y <= 0.0f)
            || (normalizedDirection.y > 0 && mainCamera.transform.position.y >= scrollAreaTop))
        {
            normalizedDirection.y = 0.0f;
        }

        mainCamera.transform.Translate(normalizedDirection * scrollSpeed * Time.deltaTime);
    }

    public float GetOrthographicSize()
    {
        return mainCamera.orthographicSize;
    }

    public void SetOrthographicSize(float size)
    {
        mainCamera.orthographicSize = size;
    }

    public Vector3 GetCameraPosition()
    {
        return mainCamera.transform.position;
    }

    public Vector3 CenterObjectInView(GameObject obj)
    {
        // Get middle of obj based on width and height, and then place it
        // at the middle of camera

        return new Vector3();
    }

    public void RevertToDefaultRotation()
    {
        mainCamera.transform.Rotate(0.0f, 0.0f, -mainCamera.transform.eulerAngles.z);
    }

    public void StartDrunkCam(float drunkLevel)
    {
        // If hero just got drunk, multiplier will be 1
        // If about to black out, in this example, multiplier will be 4
        drunkMultiplier = drunkLevel / 10;
    }

    public void StopDrunkCam()
    {
        drunkMultiplier = 0.0f;
    }

    // Moves the camera to reflect the character being drunk
    void DrunkCam()
    {
        if (swayingRight)
        {
            mainCamera.transform.Rotate(0.0f, 0.0f, 0.01f * drunkMultiplier);

            if ((mainCamera.transform.eulerAngles.z >= (2.0f * drunkMultiplier)) && mainCamera.transform.eulerAngles.z <= 180.0f)
            {
                swayingRight = false;
            }
        }
        else
        {
            mainCamera.transform.Rotate(0.0f, 0.0f, -0.01f * drunkMultiplier);

            if ((mainCamera.transform.eulerAngles.z <= (360.0f - (2.0f * drunkMultiplier))) && mainCamera.transform.eulerAngles.z >= 180.0f)
            {
                swayingRight = true;
            }
        }
    }

    public void PanToLocation(Vector3 location)
    {
        currentlyPanning = true;
        panDestination = location;
        panDestination.z = mainCamera.transform.position.z;
        panVector = location - mainCamera.transform.position;
        panVector.Normalize();
        panVector *= panSpeed;

        if (drunkMultiplier > 0.0f)
        {
            panAdjustTimer = defaultPanAdjust;
        }
    }

    public void SnapToLocation(Vector3 location)
    {
        var newLocation = location;
        newLocation.z = mainCamera.transform.position.z;
        mainCamera.transform.position = newLocation;
        currentlyPanning = false;
    }

    public Vector2 WorldToScreen(Vector2 position)
    {
        return mainCamera.WorldToScreenPoint(position);
    }

    public void SetTargetTexture(RenderTexture texture)
    {
        secondaryCamera.enabled = true;
        var newLocation = mainCamera.transform.position;
        newLocation.z = mainCamera.transform.position.z;
        secondaryCamera.transform.position = newLocation;
        secondaryCamera.orthographicSize = mainCamera.orthographicSize;

        mainCamera.targetTexture = texture;
    }

    public Vector2 SecondaryWorldToScreen(Vector2 position)
    {
        return secondaryCamera.WorldToScreenPoint(position);
    }

    public void SecondaryRender()
    {
        mainCamera.Render();
    }
}
