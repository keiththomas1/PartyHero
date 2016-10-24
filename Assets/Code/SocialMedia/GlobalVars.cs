using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GlobalVars
{
    private List<MonoBehaviour> cashListeners = new List<MonoBehaviour>();
    private static GlobalVars instance;
    private GlobalSaveVariables currentSave;
    private string savePath;

    private GlobalVars() {
        savePath = Application.persistentDataPath + "/globalPlayerInfo.dat";
    }

    public static GlobalVars Instance
    {
        get {
            if (instance == null)
            {
                instance = new GlobalVars();
            }
            return instance;
        }
    }

    public void RegisterCashListener(MonoBehaviour listener)
    {
        cashListeners.Add(listener);
    }

    public void UnregisterCashListener(MonoBehaviour listener)
    {
        cashListeners.Remove(listener);
    }

    public float MusicLevel
    {
        get { return currentSave.musicLevel; }
        set
        {
            currentSave.musicLevel = value;
            SaveGame();
        }
    }

    public float SoundEffectsLevel
    {
        get { return currentSave.soundEffectsLevel; }
        set
        {
            currentSave.soundEffectsLevel = value;
            SaveGame();
        }
    }

    public void AddCash(float cash)
    {
        if (currentSave.totalCash + cash < 0.0f)
        {
            currentSave.totalCash = 0.0f;
        }
        else
        {
            currentSave.totalCash += cash;
        }

        foreach (MonoBehaviour listener in cashListeners)
        {
            if (listener)
            {
                listener.BroadcastMessage("OnTotalCashUpdated", currentSave.totalCash);
            }
            else
            {
                cashListeners.Remove(listener);
            }
        }

        SaveGame();
    }

    public string PlayerName
    {
        get { return currentSave.playerName; }
    }

    public float TotalCash
    {
        get { return currentSave.totalCash; }
    }

    public void AddStyle(int style)
    {
        currentSave.styleLevel += style;
        SaveGame();
    }

    public int StyleLevel
    {
        get { return currentSave.styleLevel; }
        set
        {
            currentSave.styleLevel = value;
            SaveGame();
        }
    }

    public void AddHotness(int hotness)
    {
        currentSave.hotnessLevel += hotness;
        SaveGame();
    }

    public int HotnessLevel
    {
        get { return currentSave.hotnessLevel; }
        set
        {
            currentSave.hotnessLevel = value;
            SaveGame();
        }
    }

    public TimeSpan GetTotalTimePlayed()
    {
        var timePlayedThisSession = DateTime.Now - currentSave.lastUpdate;
        return currentSave.totalTimePlayed + timePlayedThisSession;
    }

    public void SaveGame()
    {
        Thread oThread = new Thread(new ThreadStart(SaveGameThread));
        oThread.Start();
    }

    private void SaveGameThread()
    {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var timePlayedThisSession = DateTime.Now - currentSave.lastUpdate;
            currentSave.totalTimePlayed += timePlayedThisSession;
            currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, currentSave);
            Debug.Log("Saved global vars file");
        }
        else
        {
            Debug.Log("Problem opening " + file.Name + " for writing");
        }

        file.Close();
    }

    public bool LoadGame()
    {
        bool fileLoaded = false;
        if (File.Exists(savePath))
        {
            FileStream file = File.Open(savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                currentSave = (GlobalSaveVariables)bf.Deserialize(file);
                Debug.Log("Global vars loaded from " + savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            currentSave = new GlobalSaveVariables();
            currentSave.totalTimePlayed = new TimeSpan(0);
            currentSave.playerName = GenerateRandomName();
            currentSave.musicLevel = 1.0f;
            currentSave.soundEffectsLevel = 1.0f;
            currentSave.totalCash = 0.0f;
            currentSave.hotnessLevel = 1;
            currentSave.styleLevel = 1;
            SaveGame();
        }

        currentSave.lastUpdate = DateTime.Now;
        return fileLoaded;
    }

    string GenerateRandomName()
    {
        string[] names =
        {
            "Alaina Less",
            "Nan Forgettem",
            "Lindsey Mcuvachkscov",
            "Cassie Goode",
            "Aubrey Catania",
            "Maya Lawsuit",
            "Monica Sosa",
            "Emily Muffintrucks",
            "Amy Thotman",
            "Jessica Enigma"
        };

        var selection = UnityEngine.Random.Range(0, names.Length);
        return names[selection];
    }
}

[Serializable]
class GlobalSaveVariables
{
    public DateTime lastUpdate;
    public TimeSpan totalTimePlayed;
    public string playerName;
    public float musicLevel;
    public float soundEffectsLevel;
    public float totalCash;
    public int hotnessLevel;
    public int styleLevel;
}