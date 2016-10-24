using UnityEngine;
using System.Collections.Generic;

public class MarketingGuyController : MonoBehaviour {
    public List<Sprite> introScreens;
    private GlobalVars globalVars;
    private GameObject marketingGuyObject;
    private SpriteRenderer marketingGuySpriteRenderer;
    private List<Sprite> currentScreens;
    private string currentState = null; // Intro, FirstOffer, SecondOffer, etc.
    private int currentIndex = 0;

	// Use this for initialization
	void Awake () {
        globalVars = GlobalVars.Instance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool EventInPlay()
    {
        return currentState != null;
    }

    public void CheckUserInput(string colliderName)
    {
        switch (colliderName)
        {
            case "OkayButton":
                GoToNextScreen();
                break;
        }
    }

    public void StartIntroSequence()
    {
        marketingGuyObject = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/MarketingGuy") as GameObject);
        var marketingGuySprite = marketingGuyObject.transform.Find("MarketingGuySprite");
        marketingGuySpriteRenderer = marketingGuySprite.GetComponent<SpriteRenderer>();
        marketingGuySpriteRenderer.sprite = introScreens[0];

        currentScreens = introScreens;
        currentState = "Intro";
        currentIndex = 0;
    }

    private void GoToNextScreen()
    {
        if (currentIndex + 1 == currentScreens.Count)
        {
            currentState = null;
            GameObject.Destroy(marketingGuyObject);
        }
        else
        {
            currentIndex++;
            marketingGuySpriteRenderer.sprite = currentScreens[currentIndex];
        }
    }
}
