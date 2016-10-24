using UnityEngine;

public class DelayGramController : MonoBehaviour {
    public Sprite[] navigationButtonSprites;
    private GlobalVars globalVars;
    private DelayGramSerializer serializer;
    private HomeScreenController homeController;
    private DGProfileController profileController;
    private PostScreenController postController;
    private MessagesScreenController messagesController;
    private EventController eventController;
    private SoundController soundController;
    private GameObject navigationButtons;
    private string currentPage;

    void Awake()
    {
        globalVars = GlobalVars.Instance;
        serializer = DelayGramSerializer.Instance;
        homeController = GetComponent<HomeScreenController>();
        profileController = GetComponent<DGProfileController>();
        postController = GetComponent<PostScreenController>();
        messagesController = GetComponent<MessagesScreenController>();
        eventController = GetComponent<EventController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        navigationButtons = transform.Find("DGNavigation").gameObject;
        currentPage = "";

        var newMailText = transform.Find("DGNavigation").Find("NewMailText");
        if (newMailText)
        {
            newMailText.GetComponent<TextMesh>().text = "";
        }
    }

    void Start()
    {
        GenerateProfilePage();
    }

    void Update()
    {
        UserInputCheck();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // Paused game
        {
        }
        else // Resumed game
        {
        }
    }

    private void UserInputCheck()
    {
        // For testing only
        if (Input.GetKeyDown(KeyCode.M))
        {
            globalVars.AddCash(50.0f);
            serializer.AddFollowers(50);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (eventController && eventController.EventInPlay())
                {
                    eventController.CheckUserInput(hit.collider.name);
                }
                else if (messagesController && messagesController.PopupActive())
                {
                    CheckPageInput(hit.collider.name);
                }
                else if (postController && postController.PopupActive())
                {
                    CheckPageInput(hit.collider.name);
                }
                else
                {
                    switch (hit.collider.name)
                    {
                        case "HomePageButton":
                            soundController.PlayClickSound(1);
                            GenerateHomePage();
                            break;
                        case "ProfilePageButton":
                            soundController.PlayClickSound(1);
                            GenerateProfilePage();
                            break;
                        case "PostPageButton":
                            soundController.PlayClickSound(1);
                            GeneratePostPage();
                            break;
                        case "MessagesPageButton":
                            soundController.PlayClickSound(1);
                            GenerateMessagesPage();
                            break;
                        default:
                            CheckPageInput(hit.collider.name);
                            break;
                    }
                }
            }
        }
    }

    private void CheckPageInput(string colliderName)
    {
        switch (currentPage)
        {
            case "Home":
                homeController.CheckUserInput(colliderName);
                break;
            case "Profile":
                profileController.CheckUserInput(colliderName);
                break;
            case "Post":
                postController.CheckUserInput(colliderName);
                break;
            case "Messages":
                messagesController.CheckUserInput(colliderName);
                break;
        }
    }

    public bool BackOut()
    {
        if (eventController && eventController.EventInPlay())
        {
            // Can exit out if not a dialog choice
        }
        else if (messagesController && messagesController.PopupActive())
        {
            messagesController.DestroyPopup();
        }
        else if (postController && postController.PopupActive())
        {
            postController.DestroyPopup();
        }
        else
        {
            return true;
        }

        return false;
    }

    private void GenerateHomePage()
    {
        if (currentPage != "Home")
        {
            if (navigationButtons && navigationButtonSprites.Length == 4)
            {
                navigationButtons.GetComponent<SpriteRenderer>().sprite =
                    navigationButtonSprites[0];
            }

            DestroyPage(currentPage);
            homeController.EnterScreen();
            currentPage = "Home";
        }
    }

    private void GenerateProfilePage()
    {
        if (currentPage != "Profile")
        {
            if (navigationButtons && navigationButtonSprites.Length == 4)
            {
                navigationButtons.GetComponent<SpriteRenderer>().sprite =
                    navigationButtonSprites[1];
            }

            DestroyPage(currentPage);
            profileController.EnterScreen();
            currentPage = "Profile";
        }
    }

    private void GeneratePostPage()
    {
        if (currentPage != "Post")
        {
            if (navigationButtons && navigationButtonSprites.Length == 4)
            {
                navigationButtons.GetComponent<SpriteRenderer>().sprite =
                    navigationButtonSprites[2];
            }

            DestroyPage(currentPage);
            postController.EnterScreen();
            currentPage = "Post";
        }
    }

    private void GenerateMessagesPage()
    {
        if (currentPage != "Messages")
        {
            if (navigationButtons && navigationButtonSprites.Length == 4)
            {
                navigationButtons.GetComponent<SpriteRenderer>().sprite =
                    navigationButtonSprites[3];
            }

            DestroyPage(currentPage);
            messagesController.EnterScreen();
            currentPage = "Messages";
        }
    }

    public void ExitDelayGram()
    {
        DestroyPage(currentPage);
    }

    private void DestroyPage(string pageName)
    {
        switch (pageName)
        {
            case "Home":
                homeController.DestroyPage();
                break;
            case "Profile":
                profileController.DestroyPage();
                break;
            case "Post":
                postController.DestroyPage();
                break;
            case "Messages":
                messagesController.DestroyPage();
                break;
        }
    }
}
