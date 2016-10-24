using UnityEngine;

public class ScrollController : MonoBehaviour
{
    private GameObject scrollObject;
    private float scrollAreaTop;
    private float scrollAreaBottom;
    private bool scrollInitialized = false;

    private float scrollSpeed;
    private bool isScrolling;
    private float maxScrollTimer;
    private float scrollTimer;

    // For mouse position handling
    private float prevMouseY, mouseY;

	// Use this for initialization
	void Start () {
        scrollSpeed = 2.5f;
        maxScrollTimer = 0.2f;
        scrollTimer = maxScrollTimer;
	}

    public void UpdateScrollArea(GameObject scrollArea, float top, float bottom)
    {
        scrollObject = scrollArea;
        scrollAreaTop = top;
        scrollAreaBottom = bottom;

        scrollInitialized = true;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (scrollInitialized)
        {
            prevMouseY = mouseY;
            mouseY = Input.mousePosition.y;

            if (!isScrolling && Input.GetMouseButton(0))
            {
                scrollTimer -= Time.deltaTime;
                if (scrollTimer <= 0.0f)
                {
                    isScrolling = true;
                }
            }

            // Only for development use
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Scroll up
            {
                scrollObject.transform.Translate(new Vector2(0.0f, -12.0f * Time.deltaTime));
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scroll down
            {
                scrollObject.transform.Translate(new Vector2(0.0f, 12.0f * Time.deltaTime));
            }

            if (Input.GetMouseButtonUp(0))
            {
                scrollTimer = maxScrollTimer;
                isScrolling = false;
            }

            if (isScrolling)
            {
                float yDistance = prevMouseY - mouseY;
                if ((yDistance < 0.0f && transform.localPosition.y > scrollAreaBottom) ||
                    (yDistance > 0.0f && transform.localPosition.y < scrollAreaTop)) {
                    yDistance = 0.0f;
                }
                scrollObject.transform.Translate(0.0f, -1 * Time.deltaTime * (yDistance/5.0f), 0.0f);
            }
        }
	}
}
