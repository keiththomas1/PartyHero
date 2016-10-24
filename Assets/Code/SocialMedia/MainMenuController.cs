using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    private bool gameLoaded = false;
    private InputController ioController;
    private GameObject titleScreen;
    private SoundController soundController;
    private GlobalVars globalVars;
    private DelayGramSerializer dgSerializer;
    private MessagesSerializer messagesSerializer;
    private MarketingGuyController marketingGuyController;
    private GameObject earningsChart;
    private bool loadMarketingGuy = false;

    // App controller settings
    private string currentApp;
    private GameObject currentAppObject;
    private GameObject delayGramApp;
    private GameObject hubApp;
    private GameObject shopApp;
    private GameObject settingsApp;

    // Use this for initialization
    void Start()
    {
        EnterMainMenu();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        marketingGuyController = GetComponent<MarketingGuyController>();

        currentApp = "";
        currentAppObject = null;

        delayGramApp = Resources.Load("Prefabs/SocialMedia/DGApp") as GameObject;
        hubApp = Resources.Load("Prefabs/SocialMedia/Hub/HubApp") as GameObject;
        shopApp = Resources.Load("Prefabs/SocialMedia/Shop/App") as GameObject;
        settingsApp = Resources.Load("Prefabs/SocialMedia/SettingsApp") as GameObject;

        LoadState();

        if (loadMarketingGuy)
        {
            marketingGuyController.StartIntroSequence();
        }
    }
	
	// Update is called once per frame
	void Update () {
        CheckUserInput();
    }

    // Consider OnApplicationFocus also (when keyboard is brought up on android for instance)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // Paused game
        {
            SaveState();
        }
        else // Resumed game
        {
            gameLoaded = false;
            LoadState();
            GenerateUpdates();
        }
    }

    void SaveState()
    {
        dgSerializer.SaveGame();
        globalVars.SaveGame();
    }

    private void GenerateUpdates()
    {
        var totalLikes = dgSerializer.GetLikes();
        var cashEarned = dgSerializer.GetCashGenerated();
        dgSerializer.ClearInfo();

        if (totalLikes > 0)
        {
            ShowEarningsChart(totalLikes, cashEarned);

            globalVars.AddCash(cashEarned);
        }
    }

    private void ShowEarningsChart(int notifications, float cashEarned)
    {
        if (!earningsChart)
        {
            earningsChart = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/EarningsChart") as GameObject);

            var likesText = earningsChart.transform.Find("LikesText");
            likesText.GetComponent<TextMesh>().text = notifications.ToString();

            var cashText = earningsChart.transform.Find("CashText");
            cashText.GetComponent<TextMesh>().text = cashEarned.ToString("C2");

            var followersText = earningsChart.transform.Find("FollowersText");
            followersText.GetComponent<TextMesh>().text = "0";
        }
    }

    void LoadState()
    {
        if (!gameLoaded)
        {
            globalVars = GlobalVars.Instance;
            dgSerializer = DelayGramSerializer.Instance;
            messagesSerializer = MessagesSerializer.Instance;
            var globalSaveFound = globalVars.LoadGame();
            var dgsSaveFound = dgSerializer.LoadGame();
            var messagesSaveFound = messagesSerializer.LoadGame();
            if (!globalSaveFound || !dgsSaveFound || !messagesSaveFound)
            {
                loadMarketingGuy = true;
            }

            gameLoaded = true;
        }
    }

    private void CheckUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (marketingGuyController && marketingGuyController.EventInPlay())
                {
                    marketingGuyController.CheckUserInput(hit.collider.name);
                }
                else if (earningsChart)
                {
                    switch (hit.collider.name)
                    {
                        case "OkayButton":
                            GameObject.Destroy(earningsChart);
                            break;
                    }
                }
                else
                {
                    switch (hit.collider.name)
                    {
                        case "ExitButton":
                            ioController.RevertToNormal();
                            GameObject.Destroy(transform.parent.gameObject);
                            break;
                        case "DGIcon":
                            soundController.PlayClickSound(2);
                            LeaveMainMenu();
                            CreateDelayGram();
                            break;
                        case "HubIcon":
                            soundController.PlayClickSound(2);
                            LeaveMainMenu();
                            CreateHub();
                            break;
                        case "ShopIcon":
                            soundController.PlayClickSound(2);
                            LeaveMainMenu();
                            CreateShop();
                            break;
                        case "SettingsIcon":
                            soundController.PlayClickSound(2);
                            LeaveMainMenu();
                            CreateSettings();
                            break;
                        case "BackButton":
                            BackOut();
                            break;
                    }
                }
            }
        }
    }


    public void CreateDelayGram()
    {
        currentAppObject = GameObject.Instantiate(delayGramApp);
        currentAppObject.transform.position = new Vector3(
            transform.position.x + 0.0f, transform.position.y + 1.02f, currentAppObject.transform.position.z);
        currentAppObject.transform.parent = transform.parent;
        currentApp = "DelayGram";
    }

    public void CreateHub()
    {
        currentAppObject = GameObject.Instantiate(hubApp);
        currentAppObject.transform.position = new Vector3(
            transform.position.x, transform.position.y, currentAppObject.transform.position.z);
        currentAppObject.transform.parent = transform.parent;
        currentApp = "Hub";
    }

    public void CreateShop()
    {
        currentAppObject = GameObject.Instantiate(shopApp);
        currentAppObject.transform.position = new Vector3(
            transform.position.x - 0.16f, transform.position.y - 0.56f, currentAppObject.transform.position.z);
        currentAppObject.transform.parent = transform.parent;
        currentApp = "Shop";
    }

    public void CreateSettings()
    {
        currentAppObject = GameObject.Instantiate(settingsApp);
        currentAppObject.transform.position = new Vector3(
            transform.position.x, transform.position.y, currentAppObject.transform.position.z);
        currentAppObject.transform.parent = transform.parent;
        currentApp = "Settings";
    }

    public bool BackOutCurrentApp()
    {
        switch (currentApp)
        {
            case "DelayGram":
                if (currentAppObject.GetComponent<DelayGramController>().BackOut())
                {
                    DeleteCurrentApp();
                    return true;
                }
                break;
            case "Hub":
                if (currentAppObject.GetComponent<HubController>().BackOut())
                {
                    DeleteCurrentApp();
                    return true;
                }
                break;
            case "Shop":
                if (currentAppObject.GetComponent<ShopController>().BackOut())
                {
                    DeleteCurrentApp();
                    return true;
                }
                break;
            case "Settings":
                if (currentAppObject.GetComponent<SettingsController>().BackOut())
                {
                    DeleteCurrentApp();
                    return true;
                }
                break;
        }

        return false;
    }

    public void DeleteCurrentApp()
    {
        switch (currentApp)
        {
            case "DelayGram":
                currentAppObject.GetComponent<DelayGramController>().ExitDelayGram();
                break;
            case "Hub":
                currentAppObject.GetComponent<HubController>().LeaveApp();
                break;
            case "Shop":
                currentAppObject.GetComponent<ShopController>().LeaveApp();
                break;
            case "Settings":
                currentAppObject.GetComponent<SettingsController>().LeaveApp();
                break;
        }

        GameObject.Destroy(currentAppObject);
        currentApp = "";
    }

    private void BackOut()
    {
        soundController.PlayBackSound();
        if (BackOutCurrentApp())
        {
            EnterMainMenu();
        }
    }

    private void LeaveMainMenu()
    {
        if (titleScreen)
        {
            GameObject.Destroy(titleScreen);
        }
    }

    private void EnterMainMenu()
    {
        if (titleScreen) {
            Application.Quit();
        } else {
            titleScreen = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/TitleScreen")) as GameObject;
            var newPosition = transform.parent.position;
            newPosition.y += 0.3f;
            titleScreen.transform.position = newPosition;
            titleScreen.transform.parent = transform.parent;

            var batteryLevel = titleScreen.transform.Find("BatteryLevel").gameObject;
            if (batteryLevel)
            {
                batteryLevel.GetComponent<TextMesh>().text = GetBatteryLevel().ToString() + "%";
            }
            var timeText = titleScreen.transform.Find("TimeText").gameObject;
            if (timeText)
            {
                string formattedTime = System.DateTime.Now.ToString("h:mm");
                timeText.GetComponent<TextMesh>().text = formattedTime;
            }
        }
    }

    public static float GetBatteryLevel()
    {
#if UNITY_IOS
         UIDevice device = UIDevice.CurrentDevice();
         device.batteryMonitoringEnabled = true; // need to enable this first
         Debug.Log("Battery state: " + device.batteryState);
         Debug.Log("Battery level: " + device.batteryLevel);
         return device.batteryLevel*100;
#elif UNITY_ANDROID

        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    if (null != unityPlayer)
                    {
                        using (AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            if (null != currActivity)
                            {
                                using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[] { "android.intent.action.BATTERY_CHANGED" }))
                                {
                                    using (AndroidJavaObject batteryIntent = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, intentFilter }))
                                    {
                                        int level = batteryIntent.Call<int>("getIntExtra", new object[] { "level", -1 });
                                        int scale = batteryIntent.Call<int>("getIntExtra", new object[] { "scale", -1 });

                                        // Error checking that probably isn't needed but I added just in case.
                                        if (level == -1 || scale == -1)
                                        {
                                            return 50f;
                                        }
                                        return ((float)level / (float)scale) * 100.0f;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error " + ex.ToString() + " while getting battery life.");
            }
        }

        return 100;
#else
        return 0;
#endif
    }
}
