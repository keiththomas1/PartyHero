using UnityEngine;

public class ShopController : MonoBehaviour
{
    private ShopVisualsController visualsController;
    private ShopItemsController itemsController;
    private ShopIAPController iapController;
    private SoundController soundController;
    private string currentScreen;

    // Use this for initialization
    void Start()
    {
        visualsController = GetComponent<ShopVisualsController>();
        itemsController = GetComponent<ShopItemsController>();
        iapController = GetComponent<ShopIAPController>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        currentScreen = "";

        GoToItemsScreen();
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
    }

    public void LeaveApp()
    {
        DestroyCurrentScreen();
    }

    public bool BackOut()
    {
        return true;
    }

    public void CheckUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (currentScreen == "Items" && itemsController.PopupActive())
                {
                    return;
                }
                else
                {
                    switch (hit.collider.name)
                    {
                        case "VisualsButton":
                            soundController.PlayClickSound(1);
                            GoToVisualsScreen();
                            break;
                        case "ItemsButton":
                            soundController.PlayClickSound(1);
                            GoToItemsScreen();
                            break;
                        case "IAPButton":
                            soundController.PlayClickSound(1);
                            GoToIAPScreen();
                            break;
                    }
                }
            }
        }
    }

    private void GoToVisualsScreen()
    {
        if (currentScreen != "Visuals")
        {
            DestroyCurrentScreen();
            currentScreen = "Visuals";
            visualsController.EnterScreen();
        }
    }

    private void GoToItemsScreen()
    {
        if (currentScreen != "Items")
        {
            DestroyCurrentScreen();
            currentScreen = "Items";
            itemsController.EnterScreen();
        }
    }

    private void GoToIAPScreen()
    {
        if (currentScreen != "IAP")
        {
            DestroyCurrentScreen();
            currentScreen = "IAP";
            iapController.EnterScreen();
        }
    }

    private void DestroyCurrentScreen()
    {
        switch (currentScreen)
        {
            case "Visuals":
                visualsController.ExitScreen();
                break;
            case "Items":
                itemsController.ExitScreen();
                break;
            case "IAP":
                iapController.ExitScreen();
                break;
        }
    }
}
