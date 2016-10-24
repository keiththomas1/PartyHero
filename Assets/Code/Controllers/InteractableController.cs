using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableController : MonoBehaviour 
{
	public List<Interactable> interactables;
	bool showGUI;
	GUIDetailsStruct GUIDetails;

	private struct GUIDetailsStruct
	{
		public Location loc; 
		public int options; 
		public List<string> commands;
	}

	public bool ShowGUI 
	{
		get 
		{ 
			return showGUI; 
		}
		set 
		{
			showGUI = value; 
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		interactables = new List<Interactable>();
		GUIDetails = new GUIDetailsStruct();
	}

	public void InitializeInteractables()
	{
		foreach( Interactable i in interactables )
		{
			i.Initialize();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
