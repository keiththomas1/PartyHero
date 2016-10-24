using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChipsInfo : InteractableInfo 
{
	
	
	
	// Use this for initialization
	void Start () 
	{
		loc = new Location( transform.position.x, transform.position.y );
		commands = new List<string>();
		
		commands.Add( "Refill" );
        commandAmount = commands.Count;
    }
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
