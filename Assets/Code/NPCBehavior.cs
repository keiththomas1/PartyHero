using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NPCBehavior : MonoBehaviour
{
    /* Walking */
    GridController grid;
    Animator animator;

    float defaultCheckWalkTime = 4.0f;
    float checkWalkTimer;

    bool currentlyWalking;
    List<Location> nodes;
    int nodeIndex;
    NodeGrid currentDestination;
    Func<bool, int> currentCallback;

    const float defaultSpeed = .07f;
    float currentSpeed;

    // Use this for initialization
    void Start ()
    {
        grid = GameObject.Find("MainController").GetComponent<GridController>();
        animator = GetComponent<Animator>();

        defaultCheckWalkTime = UnityEngine.Random.Range(defaultCheckWalkTime, defaultCheckWalkTime * 4.0f);
        checkWalkTimer = defaultCheckWalkTime;
        nodes = new List<Location>();
        nodeIndex = 0;
        currentlyWalking = false;
        currentSpeed = defaultSpeed;
    }
	
	// Update is called once per frame
	void Update () {
	    if (currentlyWalking)
        {
            ContinuePath();
        } else
        {
            if (checkWalkTimer > 0.0f)
            {
                checkWalkTimer -= Time.deltaTime;

                if (checkWalkTimer <= 0.0f)
                {
                    checkWalkTimer = defaultCheckWalkTime;
                    var destination = grid.FindRandomOpenNode();
                    StartPath(destination);
                }
            }
        }
	}

    public void StartPath(NodeGrid destination, Func<bool, int> callback = null)
    {
        currentDestination = destination;
        currentCallback = callback;
        NodeGrid currentNode = grid.FindClosestNode(transform.position.x, transform.position.y);

        nodes = grid.FindPath(
            new Location(currentNode.loc.x, currentNode.loc.y),
            new Location(destination.loc.x, destination.loc.y));
        if (nodes.Count > 1)
        {
            currentlyWalking = true;
            animator.Play("Walking");
            nodeIndex = 0;
            grid.EmptyOutNode(currentNode.loc.x, currentNode.loc.y);

            // For now, choose the end node for which direction to face
            // Eventually, would be better to find next horizontal node that
            // forces a change in direction
            FaceTheRightWay(nodes[nodes.Count - 1].x);
        }
        else // Assuming already at destination
        {
            if (null != currentCallback)
                currentCallback(true);
        }
    }

    void ContinuePath()
    {
        // If hero has reached end of the path
        if (nodeIndex >= nodes.Count)
        {
            currentlyWalking = false;
            animator.Play("Standing");
            grid.FillUpNode(transform.position.x, transform.position.y, this.gameObject);
            currentSpeed = defaultSpeed;

            if (null != currentCallback)
                currentCallback(true);
        }
        else // Still walking
        {
            // Calculate vector towards node, and reduce it
            // Could only calculate this once per node arrival, no need to do every step
            var moveVector = new Vector3(nodes[nodeIndex].x - transform.position.x,
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
        if (x < (transform.position.x - 0.5f))
        {
            temp = transform.localScale;
            transform.localScale = new Vector3(-1.0f, temp.y, temp.z);
        }
        else if (x > (transform.position.x + 0.5f))
        {
            temp = transform.localScale;
            transform.localScale = new Vector3(1.0f, temp.y, temp.z);
        } // else (do nothing if it's same x)
    }
}
