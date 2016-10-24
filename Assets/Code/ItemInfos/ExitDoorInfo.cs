using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ExitDoorInfo : InteractableInfo
{

	// Use this for initialization
	void Start () 
	{
        base.BaseInit();
		loc = new Location( transform.position.x, transform.position.y );
		commands = new List<string>();
		
		commands.Add( "Leave Party" );

        commandAmount = commands.Count;
    }
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    int ExitToHomeScreen(bool success)
    {
        if (success)
        {
            SceneManager.LoadScene("HomeScreen");
        }
        return 0;
    }

    public override string GetName()
    {
        return "Front Door";
    }

    public override void Selection1()
	{
        PerformCommandAfterPath(ExitToHomeScreen);
	}
}
