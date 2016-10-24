using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MessagesSerializer
{
    private static MessagesSerializer instance;
    private MessageSaveVariables currentSave;
    private string savePath;
    private bool hasBeenLoaded = false;

    public static MessagesSerializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MessagesSerializer();
            }
            return instance;
        }
    }

    private MessagesSerializer()
    {
        savePath = Application.persistentDataPath + "/MessageInfo.dat";
    }

    public List<Conversation> ActiveConversations
    {
        get { return currentSave.activeConversations; }
    }

    public bool ConversationExists(string name)
    {
        bool conversationExists = false;
        foreach(var conversation in currentSave.activeConversations)
        {
            if (conversation.name == name)
            {
                conversationExists = true;
            }
        }

        return conversationExists;
    }

    public void AddConversation(Conversation conversation)
    {
        currentSave.activeConversations.Insert(0, conversation);
        SaveGame();
    }

    public void AddMessageToConversation(Message message, string conversationName, bool viewedMessage)
    {
        foreach(Conversation conversation in currentSave.activeConversations) {
            if (conversation.name == conversationName)
            {
                conversation.messages.Add(message);
                SaveGame();
                return;
            }
        }
        Debug.Log("Warning: Could not find named conversation to add message to");
    }

    public void RemoveDialogOptionsFromConversation(string conversationName)
    {
        int conversationIndex = 0;
        Conversation modifiedConversation = new Conversation();
        foreach (Conversation conversation in currentSave.activeConversations)
        {
            if (conversation.name == conversationName)
            {
                modifiedConversation = conversation;
                break;
            }
            conversationIndex++;
        }
        if (conversationIndex == currentSave.activeConversations.Count)
        {
            Debug.Log("Error finding the conversation to drop dialog from");
            return;
        }

        for (int i=0; i < modifiedConversation.messages.Count; i++)
        {
            var newMessage = modifiedConversation.messages[i];
            newMessage.dialogOptions = new List<string>();
            modifiedConversation.messages[i] = newMessage;
        }
        currentSave.activeConversations.RemoveAt(conversationIndex);
        currentSave.activeConversations.Insert(conversationIndex, modifiedConversation);
    }

    public void SaveGame()
    {
        Thread oThread = new Thread(new ThreadStart(SaveGameThread));
        oThread.Start();
    }

    public void SaveGameThread()
    {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, currentSave);
            Debug.Log("Saved messages file");
        }
        else
        {
            Debug.Log("Problem opening " + file.Name + " for writing");
        }

        file.Close();
    }

    public bool LoadGame()
    {
        if (hasBeenLoaded)
        {
            return true;
        }

        bool fileLoaded = false;
        if (File.Exists(savePath))
        {
            FileStream file = File.Open(savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                currentSave = (MessageSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            currentSave = new MessageSaveVariables();
            currentSave.lastUpdate = DateTime.Now;
            currentSave.activeConversations = new List<Conversation>();
            SaveGame();
        }

        hasBeenLoaded = true;
        return fileLoaded;
    }
}

[Serializable]
public struct MessageSaveVariables
{
    public DateTime lastUpdate;
    public List<Conversation> activeConversations;
}

[Serializable]
public struct Conversation
{
    public bool viewed;
    public string name;
    public List<Message> messages;
}

[Serializable]
public struct Message
{
    public string thumbnail;
    public string personName;
    public string bodyText;
    public List<string> dialogOptions;
}