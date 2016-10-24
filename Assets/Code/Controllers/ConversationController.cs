using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationController : MonoBehaviour {
    InterfaceController uiController;
    HeroController hero;

    Dictionary<string, ConversationContainer> conversations;
    GameObject currentConversationObject;
    NPC currentNPC;

    struct Conversation
    {
        public string text;
        public List<string> dialogChoices;
        public string action;
        // Todo: Add a lambda that gets called when a specific dialog choice is made
    };

    struct ConversationContainer
    {
        private string npcName;
        private int currentIndex;
        private List<Conversation> conversations;

        public ConversationContainer(int index)
        {
            npcName = "";
            currentIndex = index;
            conversations = new List<Conversation>();
        }

        public void AddConversation(Conversation conversation)
        {
            conversations.Add(conversation);
        }

        public Conversation GetCurrentConversation()
        {
            Debug.Log("GetCurrentConversation()" + currentIndex.ToString());
            if (currentIndex >= conversations.Count)
            {
                Debug.Log("Trying to find conversation past end of list");
                return new Conversation();
            }
            return conversations[currentIndex];
        }

        public void MakeDialogChoice(int index)
        {
            currentIndex += index;
        }
    };

    void Awake()
    {
        GenerateConversations();
    }

    // Use this for initialization
    void Start () {
        uiController = GetComponent<InterfaceController>();
        hero = GameObject.Find("Hero").GetComponent<HeroController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void GenerateConversations()
    {
        conversations = new Dictionary<string, ConversationContainer>();
        var beckyConvo = new ConversationContainer(0);

        var firstConvo = new Conversation();
        firstConvo.text = "Oh hi! It's really\n nice to meet you!";
        firstConvo.dialogChoices = new List<string>();
        firstConvo.dialogChoices.Add("Yeah you too!");
        firstConvo.action = "";
        var secondConvo = new Conversation();
        secondConvo.text = "Cool! Hope we will\nbecome fast friends!";
        secondConvo.dialogChoices = new List<string>();
        secondConvo.action = "";

        beckyConvo.AddConversation(firstConvo);
        beckyConvo.AddConversation(secondConvo);

        conversations["Becky"] = beckyConvo;

        var nickConvo = new ConversationContainer(0);

        firstConvo = new Conversation();
        firstConvo.text = "Sup bro. I'm Nick.";
        firstConvo.dialogChoices = new List<string>();
        firstConvo.dialogChoices.Add("Sup. I'm Keith");
        firstConvo.action = "";
        secondConvo = new Conversation();
        secondConvo.text = "Chill. Wanna chug a\nbeer with me?";
        secondConvo.dialogChoices = new List<string>();
        secondConvo.dialogChoices.Add("Yeah I'm down");
        secondConvo.dialogChoices.Add("Sorry dude not right now");
        secondConvo.action = "";
        var thirdConvo = new Conversation();
        thirdConvo.text = "Hell yeah. You're\ncool by me.";
        thirdConvo.dialogChoices = new List<string>();
        thirdConvo.action = "drinkBeer";
        var fourthConvo = new Conversation();
        fourthConvo.text = "Aight, all good.";
        fourthConvo.dialogChoices = new List<string>();
        fourthConvo.action = "";

        nickConvo.AddConversation(firstConvo);
        nickConvo.AddConversation(secondConvo);
        nickConvo.AddConversation(thirdConvo);
        nickConvo.AddConversation(fourthConvo);
        conversations["Nick"] = nickConvo;
    }

    public void BuildConversationDisplay(GameObject conversationObject, NPC npcinfo)
    {
        currentConversationObject = conversationObject;
        currentNPC = npcinfo;
        var portraitLeft = conversationObject.transform.Find("ConversationPortraitLeft").GetComponent<SpriteRenderer>();

        var npcHead = npcinfo.objectHandle.transform.Find("Head");
        var npcFace = npcinfo.objectHandle.transform.Find("Face");
        var portraitRight = conversationObject.transform.Find("ConversationPortraitRight");
        if (npcHead)
        {
            portraitRight.GetComponent<SpriteRenderer>().sprite = npcHead.GetComponent<SpriteRenderer>().sprite;
        }
        if (npcFace)
        {
            var portraitRightFace = portraitRight.transform.Find("Face");
            if (portraitRightFace)
            {
                portraitRightFace.GetComponent<SpriteRenderer>().sprite =
                    npcFace.GetComponent<SpriteRenderer>().sprite;
            }
        }

        var conversationText = conversationObject.transform.Find("ConversationText").GetComponent<TextMesh>();
        var dialogObject1 = conversationObject.transform.Find("DialogBox1");
        dialogObject1.GetComponent<SpriteRenderer>().enabled = false;
        dialogObject1.GetComponent<Collider>().enabled = false;
        var dialogText1 = conversationObject.transform.Find("DialogText1").GetComponent<TextMesh>();
        dialogText1.text = "";
        var dialogObject2 = conversationObject.transform.Find("DialogBox2");
        dialogObject2.GetComponent<SpriteRenderer>().enabled = false;
        dialogObject2.GetComponent<Collider>().enabled = false;
        var dialogText2 = conversationObject.transform.Find("DialogText2").GetComponent<TextMesh>();
        dialogText2.text = "";
        var dialogObject3 = conversationObject.transform.Find("DialogBox3");
        dialogObject3.GetComponent<SpriteRenderer>().enabled = false;
        dialogObject3.GetComponent<Collider>().enabled = false;
        var dialogText3 = conversationObject.transform.Find("DialogText3").GetComponent<TextMesh>();
        dialogText3.text = "";

        var conversation = conversations[name].GetCurrentConversation();
        conversationText.text = conversation.text;
        if (conversation.dialogChoices.Count > 3)
        {
            Debug.Log("More than 3 dialog choices not supported");
        }
        if (conversation.dialogChoices.Count == 3)
        {
            dialogObject3.GetComponent<SpriteRenderer>().enabled = true;
            dialogObject3.GetComponent<Collider>().enabled = true;
            dialogText3.text = conversation.dialogChoices[2];
        }
        if (conversation.dialogChoices.Count >= 2)
        {
            dialogObject2.GetComponent<SpriteRenderer>().enabled = true;
            dialogObject2.GetComponent<Collider>().enabled = true;
            dialogText2.text = conversation.dialogChoices[1];
        }
        if (conversation.dialogChoices.Count >= 1)
        {
            dialogObject1.GetComponent<SpriteRenderer>().enabled = true;
            dialogObject1.GetComponent<Collider>().enabled = true;
            dialogText1.text = conversation.dialogChoices[0];
        }
    }

    public void MakeConversationChoice(int index)
    {
        var currentName = currentNPC.name;
        if (conversations[currentName].GetCurrentConversation().dialogChoices.Count < index)
        {
            Debug.Log("Trying to make dialog choice that doesn't exist");
        }
        var currentConversation = conversations[currentName];
        currentConversation.MakeDialogChoice(index);
        conversations[currentName] = currentConversation;

        TakeAction(currentConversation.GetCurrentConversation().action);
        BuildConversationDisplay(currentConversationObject, currentNPC);
    }

    void TakeAction(string action)
    {
        switch (action)
        {
            case "drinkBeer":
                hero.GetComponent<Animator>().Play("DrinkingBeer");
                uiController.DrinkBeer(10);
                uiController.AddDrinkingExperience(10);
                break;
            default:
                break;
        }
    }
}
