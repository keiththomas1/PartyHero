using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PersonInfo : InteractableInfo 
{
    MainCameraController cameraController;
	InterfaceController uiController;
	InputController ioController;
    Animator animator;
    NPC info;

    const float defaultThrowUpTime = 10.0f;
    float throwUpTimer;
    const float defaultThrowUpRecoverTime = 5.0f;
    float throwUpRecoverTimer;

	// Use this for initialization
	void Start () 
	{
        base.BaseInit();
        this.gameObject.tag = "NPC";
        cameraController = GameObject.Find("MainController").GetComponent<MainCameraController>();
        uiController = GameObject.Find("MainController").GetComponent<InterfaceController>();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
        loc = new Location( transform.position.x, transform.position.y );
        if (commands.Count == 0)
        {
            commands = new List<string>();
        }

        commands.Insert(0, "Info");
        commands.Insert(1, "Talk");
        commandAmount = commands.Count;

        info = new NPC();
        info.objectHandle = this.gameObject;
        info.name = name;

        animator = GetComponent<Animator>();
        if( null != animator )
        {
            animator.Play("Standing");
        }


        throwUpTimer = 0.4f;
        throwUpRecoverTimer = 0.0f;
    }
	
	// Update is called once per frame
	void Update () 
	{
	    if (throwUpTimer > 0.0f)
        {
            throwUpTimer -= Time.deltaTime;

            if (throwUpTimer <= 0.0f)
            {
                var rand = Random.Range(0, 1);
                if (rand == 0)
                {
                    if (animator)
                    {
                        if (animator.HasState(0, Animator.StringToHash("ThrowingUp")))
                        {
                            commands.Insert(0, "Take Snap");
                            commandAmount++;
                            animator.Play("ThrowingUp");
                            throwUpRecoverTimer = defaultThrowUpRecoverTime;
                            var eventCount = GameObject.Instantiate(Resources.Load("Prefabs/PukeEvent") as GameObject);
                            eventCount.transform.position = new Vector3(transform.position.x, transform.position.y + 5.2f, transform.position.z);
                        }
                    }
                }
                throwUpTimer = defaultThrowUpTime;
            }
        }
        if (throwUpRecoverTimer > 0.0f)
        {
            throwUpRecoverTimer -= Time.deltaTime;

            if (throwUpRecoverTimer <= 0.0f)
            {
                commands.Remove("Take Snap");
                commandAmount--;
            }
        }
	}

    void OpenInfo()
    {
        var personInfo = GameObject.Instantiate(Resources.Load("Prefabs/Popups/PersonInfo") as GameObject);
        personInfo.transform.position = new Vector3(
            transform.position.x - 3.0f, transform.position.y + 3.0f, -2.0f);

        var personSprite = personInfo.transform.Find("PersonInfoSprite");
        // Need to show the whole prefab somewhow in the info
        // personSprite.GetComponent<SpriteRenderer>().sprite = thisSprite;

        var personName = personInfo.transform.Find("PersonInfoName");
        if (personName)
        {
            personName.GetComponent<TextMesh>().text = info.name;
        }
    }

    int OpenSnapApp(bool success)
    {
        if (success)
        {
            var snapApp = GameObject.Instantiate(Resources.Load("Prefabs/SnapApp") as GameObject);
            snapApp.transform.position = new Vector3(
                transform.position.x + 1.0f, transform.position.y + 0.4f, -5.0f);
            cameraController.PanToLocation(transform.position);
            cameraController.SetOrthographicSize(5.0f);
            ioController.DisableClicking();
            ioController.DisableScrolling();
        }
        return 0;
    }

    int OpenTalkDialog(bool success)
    {
        if( success )
            uiController.CreateConversation(info);
        return 0;
    }

    void MakeSelection(int index)
    {
        if (commands[index] == "Take Snap")
        {
            // Open up snap app
            hero.TemporarilyChangeSpeed(0.2f);
            PerformCommandAfterPath(OpenSnapApp);
        }
        else if (commands[index] == "Info")
        {
            OpenInfo();
        }
        else if (commands[index] == "Talk")
        {
            PerformCommandAfterPath(OpenTalkDialog);
        }
    }

    public override string GetName()
    {
        return info.name;
    }

    public override void Selection1()
	{
        MakeSelection(0);
    }

    public override void Selection2()
    {
        MakeSelection(1);
    }

    public override void Selection3()
    {
        MakeSelection(2);
    }

    public void Dance()
    {
        if (null != animator)
        {
            animator.Play("Dancing");
        }
    }

    public void StopDancing()
    {
        if (null != animator)
        {
            animator.Play("Standing");
        }
    }

    public void React(string action)
    {
        GameObject reaction;
        var rand = Random.Range(0, 2);
        if (rand == 0)
        {
            reaction = GameObject.Instantiate(Resources.Load("Prefabs/UpsetReaction") as GameObject);
        }
        else
        {
            reaction = GameObject.Instantiate(Resources.Load("Prefabs/HappyReaction") as GameObject);
        }

        switch (action)
        {
            case "Dancing":
                break;
            case "TurnOnMusic":
                break;
            default:
                break;
        }

        reaction.transform.position = new Vector3(
            transform.position.x, transform.position.y + 4.0f,transform.position.z);
    }

    string GetRandomName()
    {
        string[] names = { "Sydney", "Eric", "Stephen", "Monica", "Freddy", "Steph", "Harry" };
        return names[Random.Range(0, names.Length)];
    }
}
