using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeroController : MonoBehaviour 
{
    Material m;
	Animator animator;
    InputController ioController;

	// Handle of the current grid
	GridController grid;
	MainCameraController mainCam;

	// Variables concerning the pathfinding aspect
	private bool pathfinding;
	List<Location> nodes;
	int nodeIndex;
	Vector3 moveVector;
	Vector3 camPos;
    Func<bool, int> currentCallback;
    GameObject destinationIndicator;
    Queue<GameObject> pathIndicators;

    const float defaultSpeed = .1f;
    float currentSpeed;
    bool drunk;
    bool high;
    bool inRoom;
    float inRoomTimer;
    float fadeSpeed;
    Material normalMaterial;
    Material alphaMaterial;
    Color tempColor;
    string currentActivity;
    public GameObject sexHeart;
	
	// Use this for initialization
	void Start () 
	{
        grid = GameObject.Find("MainController").GetComponent<GridController>();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
        mainCam = GameObject.Find ("MainController").GetComponent<MainCameraController>();
		animator = this.GetComponent<Animator>();
        m = this.GetComponent<Renderer>().material;
        pathIndicators = new Queue<GameObject>();

        drunk = false;
        high = false;
        pathfinding = false;

        transform.localScale = new Vector3(-1.0f, 1.0f, 0.0f);

        currentSpeed = defaultSpeed;
        fadeSpeed = 2.0f;
        normalMaterial = new Material(m);
        alphaMaterial = new Material(m);
        tempColor = m.color;
        tempColor.a = 0.0f;
        alphaMaterial.color = tempColor;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( pathfinding )
		{
            ContinuePath();
		}
        if( inRoom )
        {
            if( inRoomTimer > 0.0f )
            {
                inRoomTimer -= Time.deltaTime;

                if (null != m)
                {
                    m.Lerp(m, alphaMaterial, Time.deltaTime * fadeSpeed);
                }
            }
            else
            {
                if (null != m)
                {
                    m.Lerp(m, normalMaterial, Time.deltaTime * fadeSpeed);
                }

                if( m.color.a >= .99f )
                {
                    ReadyToMove();
                }
            }
        }
	}

    // Given a destination, finds a path to that node and starts the hero on it.
    // callback is for if another objects calls this in order to send the hero somewhere and needs to do something
    // once the hero gets there.
	public void StartPath( NodeGrid destination, Func<bool, int> callback = null)
	{
        Vector2 nodeToEmpty;
        if (pathfinding)
        {
            var reverseNodes = nodes;
            reverseNodes.Reverse();
            nodeToEmpty = new Vector2(reverseNodes[0].x, reverseNodes[0].y);

            if (destinationIndicator)
            {
                GameObject.Destroy(destinationIndicator);
            }

            foreach(var indicator in pathIndicators)
            {
                GameObject.Destroy(indicator);
            }
            pathIndicators.Clear();
        } else
        {
            nodeToEmpty = new Vector2(transform.position.x, transform.position.y);
        }

        currentCallback = callback;
		NodeGrid heroNode = grid.FindClosestNode( transform.position.x, transform.position.y );
			
		grid.EmptyOutNode( nodeToEmpty.x, nodeToEmpty.y );

        nodes = grid.FindPath( new Location(heroNode.loc.x, heroNode.loc.y), new Location(destination.loc.x, destination.loc.y) );
        if ( nodes.Count > 1 )
		{
			animator.Play("Walking");
			pathfinding = true;
            nodeIndex = 0;
            destinationIndicator = GameObject.Instantiate(Resources.Load("Prefabs/DestinationIndicator") as GameObject);
            destinationIndicator.transform.position = new Vector3(destination.loc.x, destination.loc.y, 20.0f);

            foreach(Location location in nodes)
            {
                if (location == destination.loc)
                {
                    break;
                }
                var indicator = GameObject.Instantiate(Resources.Load("Prefabs/PathIndicator") as GameObject);
                indicator.transform.position = new Vector3(location.x, location.y, 18.0f);
                pathIndicators.Enqueue(indicator);
            }

            // For now, choose the end node for which direction to face
            // Eventually, would be better to find next horizontal node that
            // forces a change in direction
            FaceTheRightWay(nodes[nodes.Count-1].x);
		}
        else // Assuming already at destination
        {
            if (null != currentCallback)
                currentCallback(true);
        }

        grid.FillUpNode(destination.loc.x, destination.loc.y, this.gameObject);
        mainCam.PanToLocation(new Vector2(destination.loc.x, destination.loc.y));
    }

    public void TemporarilyChangeSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }

    // Called every frame if hero is on a path
    void ContinuePath()
    {
        // If hero has reached end of the path
        if (nodeIndex >= nodes.Count)
        {
            // Stop walking
            pathfinding = false;
            // Reset speed
            currentSpeed = defaultSpeed;

            if (drunk)
                animator.Play("DrunkStanding");
            else if (high)
                animator.Play("HighStanding");
            else
                animator.Play("Standing");

            if ( null != currentCallback )
                currentCallback(true);

            if (destinationIndicator)
            {
                GameObject.Destroy(destinationIndicator);
            }
        }
        else // Still walking
        {
            // Calculate vector towards node, and reduce it
            // Could only calculate this once per node arrival, no need to do every step
            moveVector = new Vector3(nodes[nodeIndex].x - transform.position.x,
                                     nodes[nodeIndex].y - transform.position.y,
                                     nodes[nodeIndex].y - transform.position.z);
            moveVector.Normalize();
            moveVector = moveVector * currentSpeed;
            moveVector.x = moveVector.x * transform.localScale.x;

            // Make small movement towards node
            transform.Translate(moveVector);

            // If hero has reached node
            if (Math.Abs(transform.position.x - nodes[nodeIndex].x) < .1f &&
                Math.Abs(transform.position.y - nodes[nodeIndex].y) < .1f)
            {
                nodeIndex++;
                GameObject.Destroy(pathIndicators.Dequeue());

                if (nodeIndex < nodes.Count)
                {
                    FaceTheRightWay(nodes[nodeIndex].x);
                }
            }
        }
    }

	// Changes sprite so he is facing correct way while walking
	void FaceTheRightWay(float x)
	{
		// TODO: Fix so that if the next node is up or down, don't change direction
		Vector3 temp;
		if( x < (transform.position.x - 0.5f) )
		{
			temp = transform.localScale;
			transform.localScale = new Vector3(1.0f, temp.y, temp.z);
		}
		else if ( x > (transform.position.x + 0.5f) )
		{
			temp = transform.localScale;
			transform.localScale = new Vector3(-1.0f, temp.y, temp.z);
		} // else (do nothing if it's same x)
	}

    public void EnterTheDrunk()
    {
        drunk = true;
        animator.Play("DrunkStanding");
    }
    public void ExitTheDrunk()
    {
        drunk = false;
    }

    public void EnterTheHigh()
    {
        high = true;
        animator.Play("HighStanding");
    }
    public void ExitTheHigh()
    {
        high = false;
    }

    // Couple of methods that concern entering a room and leaving it
    public void EnterTheRoom(string activity, float activityTimer)
    {
        ioController.DisableClicking();
        inRoom = true;
        currentActivity = activity;
        inRoomTimer = activityTimer;

        if (activity == "Sex")
        {
            sexHeart.transform.position = new Vector3(transform.position.x, transform.position.y + 2.0f, -2.0f);
            sexHeart.GetComponent<Animator>().Play("CatchSequence");
        }
        else if (activity == "Weed")
        {
            // For when you first enter the room to get high
        }
    }
    void ReadyToMove()
    {
        ioController.EnableClicking();
        inRoom = false;
        if (currentActivity == "Weed")
        {
            EnterTheHigh();
            ioController.ShowText("Weed");
        }
        else if (currentActivity == "Sex")
        {
            sexHeart.transform.position = new Vector3(0.0f, -40.0f, 0.0f);
            sexHeart.GetComponent<Animator>().Play("Idle");
        }
    }
}
