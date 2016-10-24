using UnityEngine;
using System;
using System.Collections.Generic;

public delegate void CreatePostCallBack();

public class NewPostController : MonoBehaviour
{
    public List<Sprite> quoteImages;
    public List<Sprite> selfieImages;
    public List<Sprite> endorsementSelectionBox;
    public List<Sprite> buttonSprites;
    public List<Sprite> greyButtonSprites;
    private SoundController soundController;
    private DelayGramSerializer serializer;

    private GameObject postPopupWindow;
    private Transform scrollArea;
    private CreatePostCallBack postCallBack;
    private GameObject fancyInfo;
    private string currentPostType;
    private float currentCashPerLike;
    private bool preWorkoutSelected;
    private bool energyDrinkSelected;

    void Awake()
    {
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        serializer = DelayGramSerializer.Instance;
    }

    void Update()
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
                    case "FancyButton":
                        if (fancyInfo)
                        {
                            fancyInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    default:
                        DisableInfoSprites();
                        break;
                }
            }
            else
            {
                DisableInfoSprites();
            }
        }
    }

    public bool PopupActive()
    {
        return postPopupWindow;
    }

    public void CreatePopup(CreatePostCallBack callBack)
    {
        postCallBack = callBack;
        var postPopupWindowPrefab = Resources.Load("Prefabs/SocialMedia/Posts/NewPostScreen") as GameObject;
        if (postPopupWindowPrefab)
        {
            postPopupWindow = GameObject.Instantiate(postPopupWindowPrefab);
            postPopupWindow.transform.position = new Vector3(0.0f, 0.0f, -1.0f);
            preWorkoutSelected = false;
            energyDrinkSelected = false;

            scrollArea = postPopupWindow.transform.Find("ScrollArea");
            var fancyButton = scrollArea.transform.Find("FancyButton").gameObject;
            if (fancyButton)
            {
                fancyInfo = fancyButton.transform.Find("FancyInfo").gameObject;
            }

            DisableEndorsements();
            GreyOutTypeButtons();

            currentPostType = "Quote";
            currentCashPerLike = 0.0f;
            SetPostType(currentPostType);
        }
    }

    public void CheckUserInput(string colliderName)
    {
        switch (colliderName)
        {
            case "CreatePostButton":
                soundController.PlayLikeSound();
                CreateNewPost();
                DestroyPopup();
                break;
            case "ExitPopupButton":
                soundController.PlayLikeSound();
                DestroyPopup();
                break;
            case "QuoteButton":
                if (currentPostType != "Quote")
                {
                    GreyOutTypeButtons();
                    SetPostType("Quote");
                    soundController.PlayLikeSound();
                }
                break;
            case "SelfieButton":
                if (currentPostType != "Selfie")
                {
                    GreyOutTypeButtons();
                    SetPostType("Selfie");
                    soundController.PlayLikeSound();
                }
                break; 
            case "PreWorkoutSelected":
                ToggleEndorsement("PreWorkout");
                break;
            case "EnergyDrinkSelected":
                ToggleEndorsement("EnergyDrink");
                break;
        }
    }

    public void DestroyPopup()
    {
        GameObject.Destroy(postPopupWindow);
    }

    public Sprite GetSpriteFromName(string name)
    {
        Sprite sprite = new Sprite();
        foreach (var image in quoteImages)
        {
            if (image.name == name)
            {
                sprite = image;
            }
        }
        foreach (var image in selfieImages)
        {
            if (image.name == name)
            {
                sprite = image;
            }
        }

        return sprite;
    }

    private void DisableInfoSprites()
    {
        if (fancyInfo)
        {
            fancyInfo.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void DisableEndorsements()
    {
        var endorsements = serializer.Endorsements;
        if (!endorsements.Contains("EnergyDrink"))
        {
            var energyDrink = scrollArea.transform.Find("EnergyDrink");
            if (energyDrink)
            {
                GameObject.Destroy(energyDrink.gameObject);
            }
        }
        if (!endorsements.Contains("PreWorkout"))
        {
            var preWorkout = scrollArea.transform.Find("PreWorkout");
            if (preWorkout)
            {
                GameObject.Destroy(preWorkout.gameObject);
            }
        }
    }

    private void ToggleEndorsement(string name)
    {
        switch (name)
        {
            case "EnergyDrink":
                var energyDrink = scrollArea.transform.Find("EnergyDrink");
                if (energyDrink)
                {
                    var selectionBox = energyDrink.transform.Find("EnergyDrinkSelected");
                    if (selectionBox)
                    {
                        if (energyDrinkSelected)
                        {
                            currentCashPerLike -= 0.25f;
                            energyDrinkSelected = false;
                            selectionBox.GetComponent<SpriteRenderer>().sprite = endorsementSelectionBox[1];
                        }
                        else
                        {
                            currentCashPerLike += 0.25f;
                            energyDrinkSelected = true;
                            selectionBox.GetComponent<SpriteRenderer>().sprite = endorsementSelectionBox[0];
                        }
                    }
                }
                break;
            case "PreWorkout":
                var preWorkout = scrollArea.transform.Find("PreWorkout");
                if (preWorkout)
                {
                    var selectionBox = preWorkout.transform.Find("PreWorkoutSelected");
                    if (selectionBox)
                    {
                        if (preWorkoutSelected)
                        {
                            currentCashPerLike -= 0.50f;
                            preWorkoutSelected = false;
                            selectionBox.GetComponent<SpriteRenderer>().sprite = endorsementSelectionBox[1];
                        }
                        else
                        {
                            currentCashPerLike += 0.50f;
                            preWorkoutSelected = true;
                            selectionBox.GetComponent<SpriteRenderer>().sprite = endorsementSelectionBox[0];
                        }
                    }
                }
                break;
        }
    }

    private void CreateNewPost()
    {
        switch (currentPostType)
        {
            case "Quote":
                serializer.NextPostTime = DateTime.Now.AddMinutes(20.0f);
                break;
            case "Selfie":
                serializer.NextPostTime = DateTime.Now.AddHours(1.0f);
                break;
        }

        var imageName = GetImage(currentPostType);
        CreateNewPostDataStructure(imageName, currentCashPerLike);

        postCallBack();
    }

    // For now, random is fine, but I want to eventually have so many
    // different ones (even if just slightly different) so that there
    // will never be a repeat. Or it would at least take the person
    // playing for a long time to cyle through. Can always add new
    // ones with new patches too.
    private string GetImage(string imageType)
    {
        string selectedImage = "";
        switch (imageType)
        {
            case "Quote":
                var randomQuoteIndex = UnityEngine.Random.Range(0, quoteImages.Count);
                selectedImage = quoteImages[randomQuoteIndex].name;
                break;
            case "Selfie":
                var randomSelfieIndex = UnityEngine.Random.Range(0, selfieImages.Count);
                selectedImage = selfieImages[randomSelfieIndex].name;
                break;
        }

        return selectedImage;
    }

    private void CreateNewPostDataStructure(string imageName, float cashPerLike)
    {
        var newPost = new DelayGramPost();
        newPost.imageName = imageName;
        newPost.cashPerLike = cashPerLike;
        newPost.likes = 0;
        newPost.dateTime = DateTime.Now;

        serializer.SerializePost(newPost);
    }

    private void GreyOutTypeButtons()
    {
        if (!postPopupWindow) return;

        var quoteButton = scrollArea.transform.Find("QuoteButton");
        if (quoteButton)
        {
            quoteButton.GetComponent<SpriteRenderer>().sprite = greyButtonSprites[0];
        }
        var selfieButton = scrollArea.transform.Find("SelfieButton");
        if (selfieButton)
        {
            selfieButton.GetComponent<SpriteRenderer>().sprite = greyButtonSprites[1];
        }
        var fancyButton = scrollArea.transform.Find("FancyButton");
        if (fancyButton)
        {
            fancyButton.GetComponent<SpriteRenderer>().sprite = greyButtonSprites[2];
        }
    }

    private void SetPostType(string type)
    {
        if (!postPopupWindow) return;
        
        switch (type)
        {
            case "Quote":
                var quoteButton = scrollArea.transform.Find("QuoteButton");
                if (quoteButton)
                {
                    quoteButton.GetComponent<SpriteRenderer>().sprite = buttonSprites[0];
                }
                break;
            case "Selfie":
                var selfieButton = scrollArea.transform.Find("SelfieButton");
                if (selfieButton)
                {
                    selfieButton.GetComponent<SpriteRenderer>().sprite = buttonSprites[1];
                }
                break;
            case "Fancy":
                var fancyButton = scrollArea.transform.Find("FancyButton");
                if (fancyButton)
                {
                    fancyButton.GetComponent<SpriteRenderer>().sprite = buttonSprites[2];
                }
                break;
        }

        currentPostType = type;
    }
}
