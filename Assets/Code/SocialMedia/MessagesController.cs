using UnityEngine;
using System.Collections.Generic;

public class MessagesController : MonoBehaviour {
    private GlobalVars globalVars;
    private DelayGramSerializer dgSerializer;
    private ThumbnailsList thumbnailsList;
    private MessagesSerializer messagesSerializer;
    private int oldFollowerCount;

	// Use this for initialization
	void Start () {
        globalVars = GlobalVars.Instance;
        dgSerializer = DelayGramSerializer.Instance;
        thumbnailsList = GetComponent<ThumbnailsList>();
        oldFollowerCount = dgSerializer.Followers;
        dgSerializer.RegisterFollowersListener(this);
        messagesSerializer = MessagesSerializer.Instance;
        // oldFollowerCount = globalVars.
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        dgSerializer.UnregisterFollowersListener(this);
    }

    public List<Conversation> GetActiveConversations()
    {
        return messagesSerializer.ActiveConversations;
    }

    public void AddDialogToConversation(string response, string conversationName)
    {
        var newMessage = new Message();
        newMessage.thumbnail = thumbnailsList.mainCharacter.name;
        newMessage.personName = globalVars.PlayerName;
        newMessage.bodyText = response;
        newMessage.dialogOptions = new List<string>();

        messagesSerializer.RemoveDialogOptionsFromConversation(conversationName);
        messagesSerializer.AddMessageToConversation(newMessage, conversationName, true);
    }

    public void OnFollowersUpdated(int newFollowerCount)
    {
        CheckMarketingGuy(newFollowerCount);
        CheckFollowers(newFollowerCount);

        oldFollowerCount = newFollowerCount;
    }

    private void CheckFollowers(int newFollowerCount)
    {
        if (oldFollowerCount < 20 && newFollowerCount >= 20)
        {
            CheckFollower1();
        }
    }

    private void CheckFollower1()
    {
        var conversationName = "Follower1";

        // if (oldFollowerCount < 10 && newFollowerCount >= 10) {
        if (!messagesSerializer.ConversationExists(conversationName))
        {
            var newConversation = new Conversation();
            newConversation.name = conversationName;

            var newMessage = CreateFollower1Message(0);
            newConversation.messages = new List<Message>();
            newConversation.messages.Add(newMessage);

            messagesSerializer.AddConversation(newConversation);
        }
        // }
        // var newMessage = CreateFollower1Message(1);
        // messagesSerializer.AddMessageToConversation(newMessage, conversationName);
    }

    private void CheckMarketingGuy(int newFollowerCount)
    {
        var conversationName = "Promoter";

        if (oldFollowerCount < 10 && newFollowerCount >= 10)
        {
            if (!messagesSerializer.ConversationExists(conversationName))
            {
                var newConversation = new Conversation();
                newConversation.name = conversationName;

                var newMessage = CreateMarketingGuyMessage(0);
                newConversation.messages = new List<Message>();
                newConversation.messages.Add(newMessage);

                messagesSerializer.AddConversation(newConversation);
            }
        }
        else if (oldFollowerCount < 100 && newFollowerCount >= 100)
        {
            var newMessage = CreateMarketingGuyMessage(1);
            messagesSerializer.AddMessageToConversation(newMessage, conversationName, false);
        }
    }

    private Message CreateDefaultMessage()
    {
        var newMessage = new Message();
        newMessage.thumbnail = "";
        newMessage.personName = "Default";
        newMessage.bodyText = "Default.";
        newMessage.dialogOptions = new List<string>();

        return newMessage;
    }

    private Message CreateMarketingGuyMessage(int index)
    {
        var newMessage = CreateDefaultMessage();
        newMessage.thumbnail = "marketingGuy";
        newMessage.personName = "Jon Grow";

        switch (index)
        {
            case 0:
                newMessage.bodyText = "Hey! Me again. Congrats on getting those followers!"
                    + " I have a friend that"
                    + " works in the advertising agency and he's offering you"
                    + " cash for every like that you get on any picture that includes"
                    + " his new energy drink 'Nurjize'. Get it, like 'energize'? Some people"
                    + " mistake it for 'Nerd J' .. actually, nevermind. It will now be an option when"
                    + " you post a selfie. I'll touch base with you again once you gather 100 followers.";
                dgSerializer.AddEndorsement("EnergyDrink");
                break;
            case 1:
                newMessage.bodyText = "Wow, 100 followers already? That's great! I hope you've"
                    + " been able to collect some money from Nurjize. If not, I have some good"
                    + " news! I have another buddy that is trying to promote his new pre-workout"
                    + " that is supposed to give you a good pump. It isn't \"technically\" FDA-approved,"
                    + " and it doesn't \"technically\" have a name, but he will give you a good"
                    + " payout for including it in your posts. I'll shoot you another message"
                    + " once you get your thousandth follower. Peace.";
                dgSerializer.AddEndorsement("PreWorkout");
                break;
            default:
                newMessage.bodyText = "Error: index for marketing guy not found";
                break;
        }

        return newMessage;
    }

    private Message CreateFollower1Message(int index)
    {
        var newMessage = CreateDefaultMessage();
        newMessage.thumbnail = "blackGirlBlondeHair";
        newMessage.personName = "Trina Jordan";

        string option1a = "Great! Can't wait :)";
        string option1b = "Cool. Sorry to hear about the no friends thing :(";
        string option1c = "Uhm .. okay";

        switch (index)
        {
            case 0:
                newMessage.bodyText = "Thanks for liking my picture! I followed"
                    + " you because I don't get a whole lot of likes on my posts,"
                    + " on account of not having many friends."
                    + " And when I do it's usually from a bot or somebody trying"
                    + " to sell me something. So I was really happy to get yours!"
                    + " I'll be your biggest fan from now on! :)";
                newMessage.dialogOptions.Add(option1a);
                newMessage.dialogOptions.Add(option1b);
                newMessage.dialogOptions.Add(option1c);
                break;
            case 1: // Re: option1b
                newMessage.bodyText = "Oh, it's okay. Maybe we can be friends eventually!"
                    + " ";
                break;
            default:
                newMessage.bodyText = "Default: something went wrong for follower 1";
                break;
        }

        return newMessage;
    }

}
