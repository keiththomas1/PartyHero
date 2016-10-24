using UnityEngine;
using System.Collections;

public class NodeGrid
{
	// Handle to parent grid
	public GridController parentGrid;
	// Coordinates
	public Location loc;
	// Whether this node is walkable
	public bool empty;
    public bool containsPerson; // If an NPC is here, you can walk through it
	// If not empty, handle to object occupying it
	public GameObject occupyingObject;


	// Use this for initialization
	public NodeGrid() 
	{
		loc = new Location();
		empty = true;
        containsPerson = false;
	}

	public bool IsEmpty()
	{
		return empty;
	}

    public bool HasPerson()
    {
        return containsPerson;
    }

    public bool IsTraversable()
    {
        return empty || containsPerson;
    }
}
