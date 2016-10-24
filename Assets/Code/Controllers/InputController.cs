using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour 
{
	bool isScrolling;
	bool canClick; 			// Specifies whether some sort of dialog/tutorial is up or not. May make this less general in future
    bool canScroll;         // Limits times at which you can move the screen, such as when blacked out
	float isScrollingTimer;
	float scrollTimerVal;
	bool supportsMultiTouch; // Whether device allows multi-touch
    bool textShowing;

    // Clicking
    const float defaultClickBuffer = 0.2f;
    float clickTimer;
    
	// For mouse position handling
	float prevMouseX, prevMouseY, mouseX, mouseY;

	// Script handlers
	GridController grid;
    MainCameraController cameraController;
	InteractableController itemController;
	InterfaceController uiController;
    ConversationController convoController;
    TextController textController;
    HeroController hero;

	Interactable tempInteractable;

	// Use this for initialization
	void Start () 
	{
		grid = GetComponent<GridController>();
        cameraController = GetComponent<MainCameraController>();
        itemController = GetComponent<InteractableController>();
		uiController = GetComponent<InterfaceController>();
        convoController = GetComponent<ConversationController>();
        textController = GetComponent<TextController>();
        hero = GameObject.Find("Hero").GetComponent<HeroController>();

		isScrolling = false;
		canClick = true;
        canScroll = true;
		scrollTimerVal = .2f;
		isScrollingTimer = scrollTimerVal;
		supportsMultiTouch = Input.multiTouchEnabled;
        textShowing = false;
        clickTimer = 0.0f;

		mouseX = 0.0f;
		mouseY = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		prevMouseX = mouseX;
		prevMouseY = mouseY;
		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y;

		// Count down scrollTimer to decide whether tapping or dragging
		if( !isScrolling && Input.GetMouseButton(0) )
		{
			isScrollingTimer -= Time.deltaTime;
			if( isScrollingTimer <= 0.0f )
			{
				isScrolling = true;
			}
		}

		// If a tap, find what was clicked on and act on it
		if( Input.GetMouseButtonUp(0) )
		{
            // If player wasn't scrolling and able to click
			if( !isScrolling && canClick)
			{
                if( textShowing )
                {
                    DestroyText();
                }
                else
                {
                    ClickAction();
                }
			}
			else
			{}
			
            // Reset scrolling
			isScrollingTimer = scrollTimerVal;
			isScrolling = false;
		}

        // Zooming
        if (canScroll && canClick)
        {
            // Windows zooming
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {  // Zoom in
                cameraController.Zoom(-1.0f);
                uiController.UpdateUI();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            { // Zoom out
                cameraController.Zoom(1.0f);
                uiController.UpdateUI();
            }

            // Mobile zooming
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                cameraController.Zoom(deltaMagnitudeDiff * 0.1f);
            }
        }

        // Controls camera scrolling
        if ( isScrolling && canScroll)
        {
            var direction = new Vector2(prevMouseX - mouseX, prevMouseY - mouseY);
            cameraController.Scroll(direction);
        }

        if (clickTimer > 0.0f)
        {
            clickTimer -= Time.deltaTime;

            if (clickTimer <= 0.0f)
            {
                EnableClicking();
            }
        }
	}

    public void RevertToNormal()
    {
        clickTimer = defaultClickBuffer;
        EnableScrolling();
        cameraController.SetOrthographicSize(6.0f);
    }

    // Here is the logic for either selecting a path to walk to or an
    // object to interact with (NPC or item)
    void ClickAction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            switch (hit.collider.name)
            {
                case "SelectBox1":
                    uiController.ChoiceClicked(1);
                    break;
                case "SelectBox2":
                    uiController.ChoiceClicked(2);
                    break;
                case "SelectBox3":
                    uiController.ChoiceClicked(3);
                    break;
                case "SelectBox4":
                    uiController.ChoiceClicked(4);
                    break;
                case "PhoneIcon":
                    uiController.CreatePhone();
                    break;
                case "DialogBox1":
                    convoController.MakeConversationChoice(1);
                    break;
                case "DialogBox2":
                    convoController.MakeConversationChoice(2);
                    break;
                case "DialogBox3":
                    convoController.MakeConversationChoice(3);
                    break;
                default:
                    Debug.Log(hit.collider.name + " clicked .. no event");
                    break;
            }
        }
        else // If no hitbox, then check the node clicked on
        {
            uiController.WipeUI();
            Vector3 point = ray.origin + (ray.direction * 4.5f);

            // Check the node clicked on to see if there is an action to take on it
            NodeGrid pointNode = grid.FindClosestNode(point.x, point.y);
            if (pointNode != null)
            {
                if (pointNode.IsEmpty())
                {
                    hero.StartPath(pointNode);
                }
                else
                {
                    tempInteractable = pointNode.occupyingObject.GetComponent<Interactable>();
                    InteractableInfo interactableInfo = tempInteractable.GetComponent<Interactable>().info;
                    if (interactableInfo != null)
                    {
                        uiController.CreateSelection(tempInteractable.transform.position.x,
                                                           tempInteractable.transform.position.y,
                                                           interactableInfo);
                    }
                }
            }
        }
    }

    public void DisableScrolling()
    {
        canScroll = false;
    }
    public void EnableScrolling()
    {
        canScroll = true;
    }
    public void DisableClicking()
    {
        canClick = false;
    }
    public void EnableClicking()
    {
        Debug.Log("Clicking enabled");
        canClick = true;
    }

    // Sends the text category ("Weed", "Girl", "Convo", etc) and lets the TextController handle it
    public void ShowText(string category)
    {
        textShowing = true;
        DisableScrolling();
        textController.ShowText(category);
    }
    void DestroyText()
    {
        textShowing = false;
        EnableScrolling();
        textController.DestroyText();
    }
}
