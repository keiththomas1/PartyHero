using UnityEngine;
using System;
using System.Collections.Generic;

public class PostScreenController : MonoBehaviour {
    public float startOfPostsX;
    public float lengthBetweenPosts;
    private GlobalVars globalVars;
    private DelayGramSerializer serializer;
    private SoundController soundController;
    private ScrollController scrollController;
    private NewPostController newPostController;
    private GameObject postPage;
    private GameObject notifyPage;
    private GameObject notificationWarning;
    private GameObject scrollArea;
    private GameObject dgNewPostPopup;
    private List<GameObject> postObjects;
    private float checkUpdateRemainingTimer;

	// Use this for initialization
    void Awake()
    {
        globalVars = GlobalVars.Instance;
        serializer = DelayGramSerializer.Instance;
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        newPostController = GetComponent<NewPostController>();
        postObjects = new List<GameObject>();

        checkUpdateRemainingTimer = -1.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (checkUpdateRemainingTimer >= 0.0f)
        {
            checkUpdateRemainingTimer -= Time.deltaTime;

            if (checkUpdateRemainingTimer <= 0.0f)
            {
                UpdateTimeRemaining();
            }
        }
	}

    public void EnterScreen()
    {
        serializer = DelayGramSerializer.Instance;

        postPage = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Posts/DGPostPage") as GameObject);
        postPage.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        scrollArea = postPage.transform.Find("ScrollArea").gameObject;

        if (serializer.NextPostTime > DateTime.Now) {
            ShowTimeRemaining();
        }

        GeneratePostObjects();
    }

    void OnDestroy()
    {
        DestroyPosts();
    }

    public void CheckUserInput(string colliderName)
    {
        if (!newPostController.PopupActive())
        {
            switch (colliderName)
            {
                case "NewPostButton":
                    newPostController.CreatePopup(NewPostCreated);
                    break;
            }
        }
        else
        {
            newPostController.CheckUserInput(colliderName);
        }
    }
    
    public void DestroyPage()
    {
        DestroyPosts();
        GameObject.Destroy(postPage);
    }
    
    private void DestroyPosts()
    {
        foreach (GameObject postObject in postObjects)
        {
            GameObject.Destroy(postObject);
        }
        postObjects.Clear();
    }

    private void GeneratePostObjects()
    {
        foreach (GameObject post in postObjects)
        {
            Destroy(post);
        }

        var pastPosts = serializer.GetPosts();
        pastPosts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));

        var currentY = scrollArea.transform.position.y + 1.5f;
        foreach (DelayGramPost post in pastPosts)
        {
            var newPost = SetupPostPrefab(post, currentY);

            currentY -= lengthBetweenPosts;
            postObjects.Add(newPost);
        }

        scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, -currentY);
    }

    private GameObject SetupPostPrefab(DelayGramPost post, float yPosition)
    {
        var postPrefab = Resources.Load("Prefabs/SocialMedia/Posts/DGPost") as GameObject;
        if (postPrefab)
        {
            var postPrefabInstance = GameObject.Instantiate(postPrefab);
            postPrefabInstance.transform.parent = scrollArea.transform;
            postPrefabInstance.transform.position = new Vector3(startOfPostsX, yPosition, 0.0f);

            var nameText = postPrefabInstance.transform.Find("NameText").GetComponent<TextMesh>();
            nameText.text = globalVars.PlayerName;
            var image = postPrefabInstance.transform.Find("PostImage").GetComponent<SpriteRenderer>();
            image.sprite = newPostController.GetSpriteFromName(post.imageName);
            var timeText = postPrefabInstance.transform.Find("TimeText").GetComponent<TextMesh>();
            var likesText = postPrefabInstance.transform.Find("LikesText").GetComponent<TextMesh>();
            
            var timeSincePost = DateTime.Now - post.dateTime;
            timeText.text = GetPostTimeFromDateTime(timeSincePost);
            likesText.text = post.likes.ToString() + " people liked this";

            return postPrefabInstance;
        }
        else
        {
            return null;
        }
    }

    private string GetPostTimeFromDateTime(TimeSpan timeSincePost)
    {
        string postTime = "";
        if (timeSincePost.Days > 0)
        {
            postTime = timeSincePost.Days.ToString() + " days ago";
        }
        else if (timeSincePost.Hours > 0)
        {
            postTime = timeSincePost.Hours.ToString() + " hours ago";
        }
        else
        {
            postTime = timeSincePost.Minutes.ToString() + " mins ago";
        }
        return postTime;
    }

    public void NewPostCreated()
    {
        GeneratePostObjects();
        ShowTimeRemaining();
    }

    public bool PopupActive()
    {
        return newPostController.PopupActive();
    }

    public void DestroyPopup()
    {
        newPostController.DestroyPopup();
    }

    private void ShowTimeRemaining()
    {
        var newPostButton = scrollArea.transform.Find("NewPostButton");
        if (newPostButton)
        {
            newPostButton.GetComponent<SpriteRenderer>().enabled = false;
            newPostButton.GetComponent<Collider>().enabled = true;
        }

        UpdateTimeRemaining();
    }

    private void UpdateTimeRemaining()
    {
        var timeRemaining = scrollArea.transform.Find("TimeRemaining");
        if (timeRemaining)
        {
            var timeTillCanPost = serializer.NextPostTime - DateTime.Now;
            if (timeTillCanPost > TimeSpan.FromDays(1))
            {
                var days = timeTillCanPost.Days;
                var daysText = "";
                if (days == 1) daysText = days + "day";
                else daysText = days + "days";
                timeRemaining.GetComponent<TextMesh>().text = daysText + " until next post";
            }
            else if (timeTillCanPost > TimeSpan.FromHours(1))
            {
                var hours = timeTillCanPost.Hours;
                var hoursText = "";
                if (hours == 1) hoursText = hours + "hour";
                else hoursText = hours + "hours";
                timeRemaining.GetComponent<TextMesh>().text = hoursText + " until next post";
                checkUpdateRemainingTimer = 600.0f;
            }
            else if (timeTillCanPost > TimeSpan.FromMinutes(1))
            {
                var minutes = timeTillCanPost.Minutes;
                var minutesText = "";
                if (minutes == 1) minutesText = minutes + "min";
                else minutesText = minutes + "mins";
                timeRemaining.GetComponent<TextMesh>().text = minutesText + " until next post";
                checkUpdateRemainingTimer = 60.0f;
            }
            else if (timeTillCanPost >= TimeSpan.FromSeconds(1))
            {
                var seconds = timeTillCanPost.Seconds;
                timeRemaining.GetComponent<TextMesh>().text = seconds + "secs until next post";
                checkUpdateRemainingTimer = 0.5f;
            }
            else
            {
                timeRemaining.GetComponent<TextMesh>().text = "";
                var newPostButton = postPage.transform.Find("NewPostButton");
                if (newPostButton)
                {
                    newPostButton.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }
}
