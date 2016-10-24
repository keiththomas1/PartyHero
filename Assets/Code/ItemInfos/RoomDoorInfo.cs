using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomDoorInfo : InteractableInfo
{
    string currentActivity;

    // Use this for initialization
    void Start()
    {
        base.BaseInit();
        loc = new Location(transform.position.x, transform.position.y);
        commands = new List<string>();

        commands.Add("Hook Up");

        commandAmount = commands.Count;
    }

    // Update is called once per frame
    void Update()
    {

    }

    int EnterRoom(bool success)
    {
        if( success )
        {
            hero.EnterTheRoom(currentActivity, 8.5f);
        }

        return 0;
    }

    public override string GetName()
    {
        return "Bedroom";
    }

    public override void Selection1()
    {
        currentActivity = "Sex";
        NodeGrid pointNode = grid.FindClosestNode(transform.position.x, transform.position.y - 2.0f);
        if (pointNode != null && (pointNode.IsEmpty() || pointNode.occupyingObject == hero.gameObject))
        {
            hero.GetComponent<HeroController>().StartPath(pointNode, EnterRoom);
        }
    }
}
