using UnityEngine;
using System.Collections.Generic;

public class MessagesScreenController : MonoBehaviour {
    private MessagesController messagesController;
    private ThumbnailsList thumbnailsList;
    private GameObject page;
    private Transform pageScrollArea;
    private Transform popupScrollArea;
    private ScrollController scrollController;
    private GameObject popupContainer;
    private List<Conversation> activeConversations;
    private List<GameObject> createdStubs;
    private Conversation currentConversationWithDialog;
    private float stubStartingX;
    private float stubStartingY;

	// Use this for initialization
	void Start () {
        messagesController = GetComponent<MessagesController>();
        thumbnailsList = GetComponent<ThumbnailsList>();
        activeConversations = new List<Conversation>();

        createdStubs = new List<GameObject>();
        stubStartingX = -0.8f;
        stubStartingY = -0.7f;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CheckUserInput(string colliderName)
    {
        if (popupContainer)
        {
            switch (colliderName)
            {
                case "Option0":
                    ConversationOptionSelected(0);
                    break;
                case "Option1":
                    ConversationOptionSelected(1);
                    break;
                case "Option2":
                    ConversationOptionSelected(2);
                    break;
                case "CloseMessageButton":
                    GameObject.Destroy(popupContainer);
                    break;
            }
        }
        else
        {
            switch (colliderName)
            {
                default:
                    foreach (Conversation conversation in activeConversations)
                    {
                        if (colliderName == conversation.name)
                        {
                            GenerateMessagePopup(conversation);
                        }
                    }
                    break;
            }
        }
    }

    public void EnterScreen()
    {
        page = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Messages/MessagesPage") as GameObject);
        var newPosition = transform.parent.position;
        newPosition.x -= 0.4f;
        newPosition.y += 3.0f;
        page.transform.position = newPosition;
        page.transform.parent = transform.parent;
        pageScrollArea = page.transform.Find("ScrollArea");

        activeConversations = messagesController.GetActiveConversations();

        GenerateMessageStubs();
    }

    public void DestroyPage()
    {
        if (page)
        {
            GameObject.Destroy(page);
        }
    }

    public bool PopupActive()
    {
        return popupContainer;
    }

    private void GenerateMessageStubs()
    {
        float currentYPosition = stubStartingY;
        foreach (Conversation conversation in activeConversations)
        {
            var messages = conversation.messages;
            if (messages.Count != 0)
            {
                var firstMessage = messages[0];

                CreateMessageStub(conversation.name, firstMessage, stubStartingX, currentYPosition);
                currentYPosition -= 1.0f;
            }
        }
    }

    private void CreateMessageStub(string name, Message message, float xPosition, float yPosition)
    {
        var stub = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Messages/MessageStub") as GameObject);
        stub.name = name;
        stub.transform.parent = pageScrollArea;
        stub.transform.localPosition = new Vector3(xPosition, yPosition, 0.0f);

        var thumbnail = stub.transform.Find("Thumbnail");
        if (thumbnail)
        {
            var thumbnailSprite = thumbnailsList.GetThumbnail(message.thumbnail);
            thumbnail.GetComponent<SpriteRenderer>().sprite = thumbnailSprite;
        }
        var previewText = stub.transform.Find("PreviewText");
        if (previewText)
        {
            var shortenedText = TurnBodyTextIntoPreviewText(message.bodyText);
            previewText.GetComponent<TextMesh>().text = shortenedText;
        }
        createdStubs.Add(stub);
    }

    private string TurnBodyTextIntoPreviewText(string bodyText)
    {
        const int lineWidth = 22;
        var newText = WrapText(bodyText, lineWidth);
        var shortenedText = newText.Substring(0, lineWidth*2 - 3);
        return shortenedText + "...";
    }

    private void GenerateMessagePopup(Conversation conversation)
    {
        popupContainer = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Messages/PopupContainer") as GameObject);
        popupContainer.transform.localPosition = new Vector3(-.18f, 1.1F, -1.0f);
        popupScrollArea = popupContainer.transform.Find("ScrollArea");
        if (!popupScrollArea)
        {
            return;
        }

        var scrollHeight = 2;
        scrollController = popupScrollArea.gameObject.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(
            popupScrollArea.gameObject, popupScrollArea.transform.localPosition.y, scrollHeight);

        const float xPosition = 0.4f;
        float yPosition = 0.0f;
        foreach (Message message in conversation.messages)
        {
            yPosition = AddMessageToPopup(conversation, message, xPosition, yPosition);
        }
    }

    private float AddMessageToPopup(Conversation conversation, Message message, float xPosition, float yPosition)
    {
        if (!popupScrollArea)
        {
            return 0;
        }
        var popupMessage = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Messages/PopupMessage") as GameObject);
        popupMessage.transform.parent = popupScrollArea;
        popupMessage.transform.localPosition = new Vector3(xPosition, yPosition, 0.0f);

        var thumbnail = popupMessage.transform.Find("Thumbnail");
        if (thumbnail)
        {
            var thumbnailSprite = thumbnailsList.GetThumbnail(message.thumbnail);
            thumbnail.GetComponent<SpriteRenderer>().sprite = thumbnailSprite;
        }
        var personName = popupMessage.transform.Find("PersonName");
        if (personName)
        {
            var formattedName = WrapText(message.personName, 15);
            personName.GetComponent<TextMesh>().text = formattedName;
        }
        int bodyLineCount = 0;
        var bodyText = popupMessage.transform.Find("BodyText");
        if (bodyText)
        {
            var formattedBodyText = WrapText(message.bodyText, 32, out bodyLineCount);
            bodyText.GetComponent<TextMesh>().text = formattedBodyText;
        }

        yPosition -= CalculateMessageHeight(bodyLineCount);

        if (message.dialogOptions.Count > 0)
        {
            currentConversationWithDialog = conversation;
        }
        int optionCount = 0;
        foreach (string option in message.dialogOptions)
        {
            var dialogOption = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Messages/DialogOption") as GameObject);
            dialogOption.transform.parent = popupScrollArea;
            dialogOption.transform.localPosition = new Vector3(xPosition - 0.3f, yPosition + 2.2f, -1.0f);
            dialogOption.name = "Option" + optionCount.ToString();

            int optionLineCount = 0;
            var newText = WrapText(option, 25, out optionLineCount);
            if (optionLineCount > 1)
            {
                var dialogOptionBack = dialogOption.transform.Find("DialogOptionBack");
                if (dialogOptionBack)
                {
                    dialogOptionBack.localScale = new Vector3(1.0f, optionLineCount, 1.0f);
                }
            }

            var dialogOptionText = dialogOption.transform.Find("DialogText");
            if (dialogOptionText)
            {
                dialogOptionText.GetComponent<TextMesh>().text = newText;
            }

            yPosition -= CalculateDialogHeight(optionLineCount);
            optionCount++;
        }

        return yPosition;
    }

    private void ConversationOptionSelected(int index)
    {
        string response = "";
        foreach (Message message in currentConversationWithDialog.messages)
        {
            if (message.dialogOptions.Count > index)
            {
                response = message.dialogOptions[index];

                break;
            }
        }

        messagesController.AddDialogToConversation(response, currentConversationWithDialog.name);
    }

    public void DestroyPopup()
    {
        GameObject.Destroy(popupContainer);
    }

    private string WrapText(string text, int lineLength)
    {
        int temp;
        var result = WrapText(text, lineLength, out temp);

        return result;
    }

    private string WrapText(string text, int lineLength, out int lineCount)
    {
        lineCount = 1;
        string newText = "";
        int currentLength = 0;
        var words = text.Split(' ');
        foreach (string word in words)
        {
            var wordLength = word.Length;
            if (currentLength + wordLength > lineLength)
            {
                newText += System.Environment.NewLine;
                lineCount++;
                currentLength = 0;
            } else {
                newText += ' ';
                currentLength++;
            }

            newText += word;
            currentLength += wordLength;
        }

        return newText;
    }

    private float CalculateMessageHeight(int lineCount)
    {
        return 0.1f + (lineCount * 0.25f);
    }

    private float CalculateDialogHeight(int lineCount)
    {
        return 0.1f + (lineCount * 0.3f);
    }
}
