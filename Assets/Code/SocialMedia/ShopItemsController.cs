using UnityEngine;
using System.Collections;

public class ShopItemsController : MonoBehaviour
{
    public Sprite[] itemPopups;
    public Sprite[] soldOutSprites;
    private GlobalVars globalVars;
    private DelayGramSerializer dgSerializer;
    private GameObject screenObject;
    private GameObject popupObject;

    // Use this for initialization
    void Awake()
    {
        globalVars = GlobalVars.Instance;
        dgSerializer = DelayGramSerializer.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
    }

    public void CheckUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (popupObject) {
                    if (hit.collider.name == "OkayButton")
                    {
                        GameObject.Destroy(popupObject);
                    }
                } else {
                    switch (hit.collider.name)
                    {
                        case "FollowersForHireButton":
                            if (globalVars.TotalCash >= 10.0f)
                            {
                                globalVars.AddCash(-10.0f);
                                dgSerializer.AddFollowers(10);
                                UpdateText();
                                GeneratePopup("FollowersForHire");
                            }
                            break;
                        case "DoubleClickButton":
                            if (globalVars.TotalCash >= 30.0f)
                            {
                                globalVars.AddCash(-30.0f);
                                dgSerializer.SetDoubleClickAbility();
                                UpdateText();
                                GeneratePopup("DoubleClick");
                            }
                            break;
                        case "MessengerBotButton":
                            if (globalVars.TotalCash >= 40.0f)
                            {
                                globalVars.AddCash(-40.0f);
                                dgSerializer.SetMessageBotAbility();
                                GeneratePopup("MessengerBot");
                            }
                            break;
                    }
                }
            }
        }
    }

    public bool PopupActive()
    {
        return popupObject;
    }

    public void EnterScreen()
    {
        screenObject = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Shop/ItemsScreen") as GameObject);

        var doubleClickIcon = screenObject.transform.Find("DoubleClickButton");
        if (dgSerializer.IsDoubleClickEnabled() && doubleClickIcon)
        {
            doubleClickIcon.GetComponent<SpriteRenderer>().sprite = soldOutSprites[0];
            doubleClickIcon.GetComponent<Collider>().enabled = false;
        }
        var messengerBotIcon = screenObject.transform.Find("MessengerBotButton");
        if (dgSerializer.IsMessageBotEnabled() && messengerBotIcon)
        {
            messengerBotIcon.GetComponent<SpriteRenderer>().sprite = soldOutSprites[1];
            messengerBotIcon.GetComponent<Collider>().enabled = false;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        if (screenObject)
        {
            var cashText = screenObject.transform.Find("CashText");
            var cash = globalVars.TotalCash;
            if (cash > 0.0f)
            {
                var formattedCash = cash.ToString("C2");
                cashText.gameObject.GetComponent<TextMesh>().text = "Cash: " + formattedCash;
            }
            else
            {
                cashText.gameObject.GetComponent<TextMesh>().text = "Cash: $0.00";
            }
        }
    }

    private void GeneratePopup(string itemName)
    {
        Sprite popupSprite;
        switch (itemName)
        {
            case "FollowersForHire":
                popupSprite = itemPopups[0];
                break;
            case "DoubleClick":
                popupSprite = itemPopups[1];
                break;
            case "MessengerBot":
                popupSprite = itemPopups[2];
                break;
            default:
                popupSprite = itemPopups[0];
                break;
        }

        popupObject = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Shop/ItemsPopup") as GameObject);
        var popupSpriteObject = popupObject.transform.Find("PopupSprite");
        popupSpriteObject.GetComponent<SpriteRenderer>().sprite = popupSprite;
    }

    public void ExitScreen()
    {
        if (screenObject)
        {
            GameObject.Destroy(screenObject);
        }
        if (popupObject)
        {
            GameObject.Destroy(popupObject);
        }
    }
}
