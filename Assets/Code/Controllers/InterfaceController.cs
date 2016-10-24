using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InterfaceController : MonoBehaviour
{
    MainCameraController cameraController;
    InputController ioController;
    ConversationController convoController;
    HeroController hero;

	/* Handles to UI elements */
	public GameObject drunkBar;
    private Vector3 drunkBarStartingScale;
    public GameObject pointsTextObject;
    private Text pointsText;

	/* Selection Box */
	public GameObject selectionGroup;
    public GameObject selectionName;
	public GameObject selection1;
	public GameObject selection1text;
	public GameObject selection2;
	public GameObject selection2text;
	public GameObject selection3;
	public GameObject selection3text;
	public GameObject selection4;
	public GameObject selection4text;
    public Sprite defaultSelectionBox;
    public Sprite eventSelectionBox;
	InteractableInfo currentItem;
	int currentSelectionCount;
    /* Conversation */
    public GameObject conversation;
    /* Effects */
    public GameObject fadingBox;
    /* Phone */
    GameObject phoneObject;
    /* Minigames */
    GameObject beerPongInstance;

	/* Properties */
	int drunkLevel;
    int startingDrunkLevel;
    int currentPoints;
    int currentBeerPongLevel = 1;
    int currentBeerPongExperience = 0;
    int neededBeerPongExperience = 400;
    int currentDrinkingLevel = 1;
    int currentDrinkingExperience = 0;
    int neededDrinkingExperience = 400;
    float selectionGroupDepth;
    bool blackedOut;
    float blackOutTimer;
    bool heroDrunk;
    float brownOutTimer;

	// Use this for initialization
	void Start () 
	{
        cameraController = GetComponent<MainCameraController>();
        ioController = GetComponent<InputController>();
        convoController = GetComponent<ConversationController>();
        hero = GameObject.Find("Hero").GetComponent<HeroController>();
        drunkBarStartingScale = drunkBar.transform.localScale;
        pointsText = pointsTextObject.GetComponent<Text>();
        drunkLevel = 0;
        startingDrunkLevel = 50;
        currentPoints = 0;
        selectionGroupDepth = -4.0f;
        blackedOut = false;
        blackOutTimer = 3.0f;
        heroDrunk = false;
        brownOutTimer = 10.0f;

        WipeUI();
		UpdateUI();
	}
	
	void Update () 
	{
        if( blackedOut )
        {
            blackOutTimer -= Time.deltaTime;
            if( blackOutTimer <= 0.0f ) // Transition out of black out, for now to home screen
            {
                SceneManager.LoadScene("HomeScreen");
            }
        }
        if( drunkLevel >= 90 )
        {
            brownOutTimer -= Time.deltaTime;
            if( brownOutTimer <= 0.0f )
            {
                brownOutTimer = 10.0f;
                BrownOut();
            }
        }
	}

	// Use this as a catch-all update UI
	public void UpdateUI()
    {
		drunkBar.transform.localScale = new Vector3(
            drunkBarStartingScale.x, (float)drunkLevel/100.0f * drunkBarStartingScale.y, drunkBarStartingScale.z);
        pointsText.text = "Total points: " + currentPoints.ToString();

    }

	// Use to reset the UI and destroy anything that is up
	public void WipeUI()
	{
		DisableSelection();
		DisableConversation();
	}

	// Returns false if already at max drunk
	public bool DrinkBeer(int drunk_level)
	{
        if ( drunkLevel >= 100 )   // If somehow already at max drunk
			return false;
		drunkLevel += drunk_level;
        if (drunkLevel >= startingDrunkLevel)   // Point at which to show a new drunken state
        {
            hero.EnterTheDrunk();
            StartDrunkCam();
        }
        if (drunkLevel >= 100)   // If hero just went over max drunk
        {
            StopDrunkCam();
            cameraController.RevertToDefaultRotation();
            BlackOut();
        }
		UpdateUI();
		return true;
	}

    public void DrinkWater(int water_level)
    {
        if (drunkLevel <= 0)   // If somehow already at max drunk
            return;
        drunkLevel -= water_level;
        if (drunkLevel < startingDrunkLevel)   // Point at which to show a new drunken state
        {
            hero.ExitTheDrunk();
            StopDrunkCam();
        }
        UpdateUI();
    }

    public void AddPartyPoints(int newPoints, Vector3 location)
    {
        currentPoints += newPoints;
        UpdateUI();

        var pointsText = GameObject.Instantiate(Resources.Load("Prefabs/PointsText") as GameObject);
        pointsText.transform.position = location;
        pointsText.GetComponent<TextMesh>().text = "+" + newPoints.ToString();
    }

    public void AddBeerPongExperience(int experiencePoints)
    {
        var experienceObject = GameObject.Instantiate(Resources.Load("Prefabs/Popups/ExperienceBar") as GameObject);
        var experiencePosition = hero.transform.position;
        experiencePosition.x -= 0.5f;
        experiencePosition.y += 4.5f;
        experienceObject.transform.position = experiencePosition;

        var descriptionText = experienceObject.transform.Find("ExperienceDescription");
        if (descriptionText)
        {
            descriptionText.GetComponent<TextMesh>().text = "Beer Pong - Lvl " + currentBeerPongLevel.ToString();
        }

        var newBeerPongExperience = currentBeerPongExperience + experiencePoints;
        experienceObject.GetComponent<ExperienceBarAnimation>().StartAnimation(
            currentBeerPongExperience, newBeerPongExperience, neededBeerPongExperience);
        currentBeerPongExperience = newBeerPongExperience;
    }

    public void AddDrinkingExperience(int experiencePoints)
    {
        var experienceObject = GameObject.Instantiate(Resources.Load("Prefabs/Popups/ExperienceBar") as GameObject);
        var experiencePosition = hero.transform.position;
        experiencePosition.x -= 0.5f;
        experiencePosition.y += 4.5f;
        experienceObject.transform.position = experiencePosition;

        var descriptionText = experienceObject.transform.Find("ExperienceDescription");
        if (descriptionText)
        {
            descriptionText.GetComponent<TextMesh>().text = "Drinking - Lvl " + currentBeerPongLevel.ToString();
        }

        var newDrinkingExperience = currentDrinkingExperience + experiencePoints;
        experienceObject.GetComponent<ExperienceBarAnimation>().StartAnimation(
            currentDrinkingExperience, newDrinkingExperience, neededDrinkingExperience);
        currentDrinkingExperience = newDrinkingExperience;
    }

    public void StartBeerPongMiniGame()
    {
        ioController.DisableClicking();
        ioController.DisableScrolling();
        beerPongInstance = GameObject.Instantiate(Resources.Load("Prefabs/BeerPongInstance") as GameObject);
        var newPosition = cameraController.GetCameraPosition();
        newPosition.y += 1.0f;
        newPosition.z = 0.0f;
        beerPongInstance.transform.position = newPosition;
        cameraController.SetOrthographicSize(5.5f);
    }

    public void EndBeerPongMiniGame()
    {
        if (beerPongInstance)
        {
            GameObject.Destroy(beerPongInstance);
            ioController.RevertToNormal();
            AddBeerPongExperience(130);
        }
    }

    // Render all the selection boxes/texts invisible
    void DisableSelection()
	{
        if( null != currentItem )
            currentItem.Unselected();
        selectionName.GetComponent<TextMesh>().text = "";
        selection1.GetComponent<Renderer>().enabled = false;
		selection2.GetComponent<Renderer>().enabled = false;
		selection3.GetComponent<Renderer>().enabled = false;
		selection4.GetComponent<Renderer>().enabled = false;
		selection1text.GetComponent<Renderer>().enabled = false;
		selection2text.GetComponent<Renderer>().enabled = false;
		selection3text.GetComponent<Renderer>().enabled = false;
		selection4text.GetComponent<Renderer>().enabled = false;
		selection1.GetComponent<BoxCollider>().enabled = false;
		selection2.GetComponent<BoxCollider>().enabled = false;
		selection3.GetComponent<BoxCollider>().enabled = false;
		selection4.GetComponent<BoxCollider>().enabled = false;
	}

	// Remove the conversation
	void DisableConversation()
	{
        if (conversation)
        {
            GameObject.Destroy(conversation);
        }
	}
	
	// From selection box choice, choose correct method on interactable
	public void ChoiceClicked(int choice)
	{
		DisableSelection();
		
		switch( choice )
		{
		case 1:
			currentItem.Selection1();
			break;
		case 2:
			currentItem.Selection2();
			break;
		case 3:
			currentItem.Selection3();
			break;
		case 4:
			currentItem.Selection4();
			break;
		default:
			Debug.Log("Passed choice outside of range");
			break;
		}
	}

	// Pops up a selection dialog at coords x,y with 1-4 options
	public void CreateSelection(float x, float y, InteractableInfo item)
	{
        DisableSelection();
        item.Selected();
		selectionGroup.transform.position = new Vector3(x, y, selectionGroupDepth);
		currentItem = item;
		currentSelectionCount = item.commandAmount;

		// Render appropriate boxes/texts
        if (currentSelectionCount > 0)
        {
            selectionName.GetComponent<TextMesh>().text = item.GetName();

            selection1.GetComponent<Renderer>().enabled = true;
            selection1.GetComponent<BoxCollider>().enabled = true;
            selection1text.GetComponent<Renderer>().enabled = true;
            selection1text.GetComponent<TextMesh>().text = item.commands[0];
            if (item.commands[0] == "Take Snap")
            {
                selection1.GetComponent<SpriteRenderer>().sprite = eventSelectionBox;
            }
            else
            {
                selection1.GetComponent<SpriteRenderer>().sprite = defaultSelectionBox;
            }
        }
		if( currentSelectionCount > 1 ) 
		{
			selection2.GetComponent<Renderer>().enabled = true;
			selection2.GetComponent<BoxCollider>().enabled = true;
			selection2text.GetComponent<Renderer>().enabled = true;
			selection2text.GetComponent<TextMesh>().text = item.commands[1];
		}
		if( currentSelectionCount > 2 )
		{
			selection3.GetComponent<Renderer>().enabled = true;
			selection3.GetComponent<BoxCollider>().enabled = true;
			selection3text.GetComponent<Renderer>().enabled = true;
			selection3text.GetComponent<TextMesh>().text = item.commands[2];
		}
		if( currentSelectionCount > 3 )
		{
			selection4.GetComponent<Renderer>().enabled = true;
			selection4.GetComponent<BoxCollider>().enabled = true;
			selection4text.GetComponent<Renderer>().enabled = true;
			selection4text.GetComponent<TextMesh>().text = item.commands[3];
		}
	}
	
	public void CreateConversation(NPC npcinfo)
	{
        conversation = GameObject.Instantiate(Resources.Load("Prefabs/Popups/Conversation") as GameObject);
        var conversationPosition = conversation.transform.position;

        var camPos = cameraController.GetCameraPosition();
        conversationPosition.x = camPos.x - 3.0f;
        conversationPosition.y = camPos.y + 11.0f;
        conversation.transform.position = conversationPosition;

        convoController.BuildConversationDisplay(conversation, npcinfo);

        cameraController.SetOrthographicSize(9.0f);
	}

    public void CreatePhone()
    {
        if (!phoneObject)
        {
            cameraController.SetOrthographicSize(3.1f);
            ioController.DisableClicking();
            ioController.DisableScrolling();
            phoneObject = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/PhoneInstance") as GameObject);
            var newPosition = cameraController.GetCameraPosition();
            newPosition.z = 0.0f;
            newPosition.y -= 1.3f;
            phoneObject.transform.position = newPosition;
        }
    }

    // Transitions camera to the "Blacked Out" screen
    public void BlackOut()
    {
        ioController.DisableScrolling();
        ioController.DisableClicking();
        var blackOutScreen = GameObject.Instantiate(Resources.Load("Prefabs/BlackedOutScreen") as GameObject);
        // Can replace eventually with cameraController.CenterObjectInView()
        var camPosition = cameraController.GetCameraPosition();
        camPosition.z = -8.0f;
        blackOutScreen.transform.position = camPosition;
        blackedOut = true;
    }

    public void StartDrunkCam()
    {
        heroDrunk = true;
        cameraController.StartDrunkCam(drunkLevel - startingDrunkLevel + 10);
        
    }
    public void StopDrunkCam()
    {
        heroDrunk = false;
        cameraController.StopDrunkCam();
    }

    // First fades to black, and then back in
    public void BrownOut()
    {
        fadingBox.GetComponent<FadeToBlack>().FadeOutAndIn();
    }
}
