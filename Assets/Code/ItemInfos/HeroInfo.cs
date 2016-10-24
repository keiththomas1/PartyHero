using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HeroInfo : InteractableInfo
{
    Camera mainCamera;
    InputController ioController;
    InterfaceController uiController;

    // Use this for initialization
    void Start()
    {
        base.BaseInit();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
        uiController = GameObject.Find("MainController").GetComponent<InterfaceController>();
        loc = new Location(transform.position.x, transform.position.y);
        commands = new List<string>();
        commands.Add("Info");
        commands.Add("Check Phone");
        commands.Add("Dance");
        commandAmount = commands.Count;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override string GetName()
    {
        return "Hero";
    }

    public override void Selection1()
    {
        OpenInfo();
    }

    public override void Selection2()
    {
        uiController.CreatePhone();
    }

    public override void Selection3()
    {
        GetComponent<Animator>().Play("Dancing");
        HighlightSquares();
        var thisx = transform.position.x;
        var thisy = transform.position.y;
        var objects = grid.GetObjectsSurroundingCoords(thisx, thisy);
        
        foreach (GameObject obj in objects) {
            InteractableInfo info = obj.GetComponent<Interactable>().info;
            PersonInfo targetType = new PersonInfo();
            // Check if object is an NPC
            if (info && info.GetType() == targetType.GetType())
            {
                PersonInfo newInfo = (PersonInfo)info;
                newInfo.React("Dancing");
            }
        }
    }

    void OpenInfo()
    {
        var personInfo = GameObject.Instantiate(Resources.Load("Prefabs/Popups/PersonInfo") as GameObject);
        personInfo.transform.position = new Vector3(
            transform.position.x - 3.0f, transform.position.y + 3.0f, -2.0f);

        var personSprite = personInfo.transform.Find("PersonInfoSprite");
        if (personSprite)
        {
            var thisSprite = GetComponent<SpriteRenderer>().sprite;
            if (thisSprite)
            {
                personSprite.GetComponent<SpriteRenderer>().sprite = thisSprite;
            }
        }

        var personName = personInfo.transform.Find("PersonInfoName");
        if (personName)
        {
            personName.GetComponent<TextMesh>().text = "Hero";
        }
    }

    void HighlightSquares()
    {
        grid.ShowOrangeSquaresSurroundingLocation(transform.position.x, transform.position.y, 2.0f);
    }
}
