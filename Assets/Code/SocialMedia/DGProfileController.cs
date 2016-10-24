using UnityEngine;

public class DGProfileController : MonoBehaviour {
    private GlobalVars globalVars;
    private DelayGramSerializer dgSerializer;
    private ScrollController scrollController;
    private GameObject page;
    private GameObject scrollArea;

    private GameObject postsInfo;
    private GameObject followersInfo;
    private GameObject moneyInfo;

    void Awake () {
        globalVars = GlobalVars.Instance;
        dgSerializer = DelayGramSerializer.Instance;
    }

    void Start()
    {
    }

    public void OnTotalCashUpdated(float newCash)
    {
        UpdateText();
    }

    public void OnFollowersUpdated(int newFollowers)
    {
        UpdateText();
    }

    void Update ()
    {
        CheckHover();
    }

    private void CheckHover()
    {
        if (Input.GetMouseButtonUp(0))
        {
            DisableInfoSprites();
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "PostsIcon":
                        DisableInfoSprites();
                        if (postsInfo)
                        {
                            postsInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    case "FollowersIcon":
                        DisableInfoSprites();
                        if (followersInfo)
                        {
                            followersInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    case "MoneyIcon":
                        DisableInfoSprites();
                        if (moneyInfo)
                        {
                            moneyInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    default:
                        DisableInfoSprites();
                        break;
                }
            } else {
                DisableInfoSprites();
            }
        }
    }

    private void DisableInfoSprites()
    {
        if (postsInfo)
        {
            postsInfo.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (followersInfo)
        {
            followersInfo.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (moneyInfo)
        {
            moneyInfo.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void CheckUserInput(string colliderName)
    {
    }

    public void EnterScreen()
    {
        page = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Profile/DGProfilePage") as GameObject);

        var pagePosition = transform.parent.position;
        pagePosition.y += 1.1f;
        pagePosition.x += 0.25f;
        page.transform.position = pagePosition;
        page.transform.parent = transform.parent;

        scrollArea = page.transform.Find("ScrollArea").gameObject;
        scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, 4.0f);

        postsInfo = scrollArea.transform.Find("PostsInfo").gameObject;
        followersInfo = scrollArea.transform.Find("FollowersInfo").gameObject;
        moneyInfo = scrollArea.transform.Find("MoneyInfo").gameObject;

        UpdateText();

        globalVars.RegisterCashListener(this);
        dgSerializer.RegisterFollowersListener(this);
    }

    private void UpdateText()
    {
        if (scrollArea)
        {
            var nameText = scrollArea.transform.Find("NameText");
            if (nameText)
            {
                nameText.gameObject.GetComponent<TextMesh>().text = globalVars.PlayerName;
            }
            var postsText = scrollArea.transform.Find("PostsText");
            if (postsText)
            {
                var postCount = dgSerializer.GetPosts().Count;
                postsText.gameObject.GetComponent<TextMesh>().text = postCount.ToString();
            }
            var followersText = scrollArea.transform.Find("FollowersText");
            if (followersText)
            {
                var followers = dgSerializer.Followers;
                followersText.gameObject.GetComponent<TextMesh>().text = followers.ToString();
            }
            var moneyText = scrollArea.transform.Find("MoneyText");
            if (moneyText)
            {
                var cash = globalVars.TotalCash;
                if (cash > 0.0f)
                {
                    var formattedCash = cash.ToString("C2");
                    moneyText.gameObject.GetComponent<TextMesh>().text = formattedCash;
                }
                else
                {
                    moneyText.gameObject.GetComponent<TextMesh>().text = "$0.00";
                }
            }
        }
    }

    public void DestroyPage()
    {
        globalVars.UnregisterCashListener(this);
        dgSerializer.UnregisterFollowersListener(this);
        GameObject.Destroy(page);
    }
}
