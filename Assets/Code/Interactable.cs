using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interactable : MonoBehaviour 
{
	GridController grid;
    InteractableController interactableController;

	public InteractableInfo info;

	bool clicked;

	// Use this for initialization
	void Start () 
	{
		grid = GameObject.Find("MainController").GetComponent<GridController>();
        interactableController = GameObject.Find("MainController").GetComponent<InteractableController>();

		clicked = false;

		interactableController.interactables.Add( this );
        Initialize();
	}

	public void Initialize()
	{
		float[] xy = grid.SnapToGrid( this.gameObject );

        Vector3 moveVector = new Vector3();
		
		moveVector.x = xy[0] - transform.position.x;
		moveVector.y = xy[1] - transform.position.y;
        moveVector.z = xy[1] - transform.position.z;    // Same as Y
		
		transform.Translate( moveVector );
	}

	// Update is called once per frame
	void Update () 
	{
	}
}
