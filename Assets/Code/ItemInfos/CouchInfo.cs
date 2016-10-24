using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CouchInfo : InteractableInfo 
{

	// Use this for initialization
	void Start () 
	{
		loc = new Location( transform.position.x, transform.position.y );
		commands = new List<string>();
		commands.Add( "Sit Down" );
        commandAmount = commands.Count;
    }

    public override string GetName()
    {
        return "Couch";
    }

    // Update is called once per frame
    void Update () {
	
	}
}
