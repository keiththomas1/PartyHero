using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeakerInfo : InteractableInfo
{
    public GameObject[] highlightedSquares;
    public PersonInfo dummyPersonInfo;

	// Use this for initialization
	void Start ()
    {
        base.BaseInit();
		loc = new Location( transform.position.x, transform.position.y );
		commands = new List<string>();
		
		commands.Add( "Turn On" );
		commands.Add( "Turn Off" );

        commandAmount = commands.Count;

        UnhighlightSquares();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    int TurnOnSpeaker(bool success)
    {
        if( success )
        {
            this.GetComponent<Animator>().Play("SpeakerBouncing");

            float thisx = transform.position.x;
            float thisy = transform.position.y;
            List<GameObject> objects = grid.GetObjectsFromGrid(thisx - 2.0f, thisy, thisx + 2.0f, thisy - 4.0f);

            // Iterate through objects in speaker range and make them dance if it's an NPC
            foreach (GameObject o in objects)
            {
                InteractableInfo info = o.GetComponent<Interactable>().info;
                // Check if object is an NPC
                if (info.GetType() == dummyPersonInfo.GetType())
                {
                    PersonInfo newInfo = (PersonInfo)info;
                    // Tell NPC to dance
                    newInfo.Dance();
                }
            }
        }

        return 0;
    }

    int TurnOffSpeaker(bool success)
    {
        if( success )
        {
            this.GetComponent<Animator>().Play("Idle");

            float thisx = transform.position.x;
            float thisy = transform.position.y;
            List<GameObject> objects = grid.GetObjectsFromGrid(thisx - 2.0f, thisy, thisx + 2.0f, thisy - 4.0f);

            // Iterate through objects in speaker range and make them stand still if NPC
            foreach (GameObject o in objects)
            {
                InteractableInfo info = o.GetComponent<Interactable>().info;
                PersonInfo targetType = new PersonInfo();
                // Check if object is an NPC
                if (info.GetType() == targetType.GetType())
                {
                    PersonInfo newInfo = (PersonInfo)info;
                    // Tell NPC to stop dancing
                    newInfo.StopDancing();
                }
            }
        }

        return 0;
    }

    public override string GetName()
    {
        return "Speakers";
    }

    // Starts the speaker animation, and makes anybody on the dance floor start dancing
    public override void Selection1()
    {
        PerformCommandAfterPath(TurnOnSpeaker);
    }

    // Stops the speaker animation, and kills any dancing on the dance floor
	public override void Selection2()
    {
        PerformCommandAfterPath(TurnOffSpeaker);
    }

    public override void Selected()
    {
        foreach (GameObject square in highlightedSquares)
        {
            square.GetComponent<Renderer>().enabled = true;
        }
    }
    public override void Unselected()
    {
        UnhighlightSquares();
    }
    void UnhighlightSquares()
    {
        foreach (GameObject square in highlightedSquares)
        {
            square.GetComponent<Renderer>().enabled = false;
        }
    }
}
