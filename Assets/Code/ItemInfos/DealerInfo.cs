using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealerInfo : InteractableInfo
{
    NPC info;
    InterfaceController uiController;

    // Use this for initialization
    void Start()
    {
        base.BaseInit();
        uiController = GameObject.Find("MainController").GetComponent<InterfaceController>();
        loc = new Location(transform.position.x, transform.position.y);
        commands = new List<string>();

        commands.Add("Buy Water");
        commands.Add("Talk");

        commandAmount = commands.Count;
    }

    // Update is called once per frame
    void Update()
    {

    }

    int DrinkWater(bool success)
    {
        if( success )
        {
            hero.GetComponent<Animator>().Play("DrinkingWater");
            uiController.DrinkWater(10);
        }
        return 0;
    }

    public override void Selection1()
    {
        PerformCommandAfterPath(DrinkWater);
    }
    public override void Selection2()
    {
        uiController.CreateConversation(info);
    }
}
