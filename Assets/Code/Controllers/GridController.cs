using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour
{
    InteractableController iController;

    // 2D array of NodeGrid's that represents the entirety of the grid
    private NodeGrid[,] nodeGrid;
    public int nodeSize;

    public int gridWidth;
    public int gridHeight;
    public int gridActualWidth;
    public int gridActualHeight;

    private List<NodeGrid> currentPath;

    // Allows breaking out of recursive pathfinding if path found.
    private bool doneSearching;

    private List<GameObject> currentHighlightSquares;
    private float currentHighlightTimer;

    bool gridInitialized = false;

    // Use this for initialization
    void Awake()
    {
        iController = this.GetComponent<InteractableController>();
        nodeGrid = new NodeGrid[gridWidth, gridHeight];

        gridActualWidth = gridWidth * nodeSize;
        gridActualHeight = gridHeight * nodeSize;

        // Generates nodeGrid array based on the height and width of the grid
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                nodeGrid[i, j] = new NodeGrid();
                nodeGrid[i, j].parentGrid = this;
                // Puts the node at the center of the square.
                nodeGrid[i, j].loc.x = (i * nodeSize) + (nodeSize / 2);
                nodeGrid[i, j].loc.y = (j * nodeSize) + (nodeSize / 2);
            }
        }

        currentPath = new List<NodeGrid>();
        doneSearching = false;
        currentHighlightSquares = new List<GameObject>();
        currentHighlightTimer = 0.0f;

        // iController.InitializeInteractables();

        gridInitialized = true;
        DebugDrawGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHighlightTimer > 0.0f)
        {
            currentHighlightTimer -= Time.deltaTime;

            if (currentHighlightTimer <= 0.0f)
            {
                foreach (GameObject square in currentHighlightSquares)
                {
                    GameObject.Destroy(square);
                }
                currentHighlightSquares.Clear();
            }
        }
    }

    // Snap object to grid so that converting is precise.
    public float[] SnapToGrid(GameObject obj)
    {
        if (!gridInitialized)
        {
            Debug.Log("Trying to snap to grid before grid is created.");
        }
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;

        int[] ij = FindClosestCoords(x, y);

        FillUpNode(x, y, obj);

        float[] xy = { nodeGrid[ij[0], ij[1]].loc.x, nodeGrid[ij[0], ij[1]].loc.y };

        return xy;
    }

    // NOTE: This could be made more efficient by using math to find the node
    // instead of iterating through the whole grid till you find it.
    public int[] FindClosestCoords(float x, float y)
    {
        int nodeI;
        int nodeJ;
        int counter;

        if (x > (gridWidth * nodeSize))
        {
            nodeI = gridWidth - 1;
        }
        else if (x < 0)
        {
            nodeI = 0;
        }
        else
        {
            // Iterate through nodeGrid until x is not greater than node
            counter = 0;
            while (counter < gridHeight && (x > (nodeGrid[counter, 0].loc.x - nodeSize / 2)))
            {
                counter++;
            }
            nodeI = counter - 1;
        }


        if (y > (gridHeight * nodeSize))
        {
            nodeJ = gridHeight - 1;
        }
        else if (y < 0)
        {
            nodeJ = 0;
        }
        else
        {
            // Iterate through nodeGrid until y is not greater than node
            counter = 0;
            while (counter < gridWidth && (y > (nodeGrid[0, counter].loc.y - nodeSize / 2)))
            {
                counter++;
            }
            nodeJ = counter - 1;
        }

        int[] ij = { nodeI, nodeJ };

        return ij;
    }

    // Return the node that contains the given x and y coordinate, return
    // null if that is outside the grid.
    public NodeGrid FindClosestNode(float x, float y)
    {
        int[] coords = FindClosestCoords(x, y);

        if (null == coords)
        {
            return null;
        }

        return nodeGrid[coords[0], coords[1]];
    }

    public NodeGrid FindClosestAdjacentNode(float x, float y, Vector2 heroLocation)
    {
        int[] heroCoords = FindClosestCoords(heroLocation.x, heroLocation.y);
        int[] coords = FindClosestCoords(x, y);

        if (null == coords)
        {
            return null;
        }

        // Check if hero is already adjacent
        if (((heroCoords[0] == coords[0] + 1) && (heroCoords[1] == coords[1]))
            || ((heroCoords[0] == coords[0] - 1) && (heroCoords[1] == coords[1]))
            || ((heroCoords[0] == coords[0]) && (heroCoords[1] == coords[1] + 1))
            || ((heroCoords[0] == coords[0]) && (heroCoords[1] == coords[1] - 1)))
        {
            Debug.Log("adjacent");
            return nodeGrid[heroCoords[0], heroCoords[1]];
        }

        if (heroCoords[0] > coords[0])
        {
            if (coords[0] < (gridWidth - 1))
            {
                var rightSpot = nodeGrid[coords[0] + 1, coords[1]];
                if (rightSpot.IsEmpty() || (heroCoords[0] == coords[0]+1 && heroCoords[1] == coords[1]))
                {
                    return rightSpot;
                }
            }
        }
        else
        {
            if (coords[0] > 0)
            {
                var leftSpot = nodeGrid[coords[0] - 1, coords[1]];
                if (leftSpot.IsEmpty())
                {
                    return leftSpot;
                }
            }
        }

        if (heroCoords[1] > coords[1])
        {
            if (coords[1] < (gridHeight - 1))
            {
                var topSpot = nodeGrid[coords[0], coords[1] + 1];
                if (topSpot.IsEmpty())
                {
                    return topSpot;
                }
            }
        }
        else
        {
            if (coords[1] > 0)
            {
                var bottomSpot = nodeGrid[coords[0], coords[1] - 1];
                if (bottomSpot.IsEmpty())
                {
                    return bottomSpot;
                }
            }
        }

        return null;
    }

    public NodeGrid FindRandomOpenNode()
    {
        bool notFound = true;

        while (notFound)
        {
            var randX = UnityEngine.Random.Range(0, gridWidth);
            var randY = UnityEngine.Random.Range(0, gridHeight);

            if (nodeGrid[randX, randY].empty)
            {
                return nodeGrid[randX, randY];
            }
        }

        return null;
    }

    // Convert world coords to grid coords
	private int[] ConvertXYtoIJ(int x, int y)
	{
		int i = (x - (nodeSize/2)) / nodeSize;
		int j = (y - (nodeSize/2)) / nodeSize;

		int[] a = {i, j};
		return a;
	}

    // Convert world coords to grid coords
    private float[] ConvertIJtoXY(int i, int j)
    {
        int x = (i * 2) + 1;
        int y = (j * 2) + 1;

        float[] a = { x, y };
        return a;
    }

    // Use SearchForPath to find path and return as list of Location's. For public use.
    public List<Location> FindPath(Location start, Location end)
	{
		int[] tuple = ConvertXYtoIJ((int)start.x, (int)start.y);
		doneSearching = false;
        SearchForPath(tuple[0], tuple[1], end, new List<NodeGrid>());

		List<Location> locations = new List<Location>();

		foreach( NodeGrid node in currentPath )
		{
			locations.Add(new Location(node.loc.x, node.loc.y));
		}

		return locations;
	}

	// Recursively search the grid using A* pathfinding to fill up
	// currentPath. i and j are index into nodeGrid.
	private void SearchForPath(int i, int j, Location end, List<NodeGrid> L)
	{
		// Add current node onto list of travelled.
		L.Add( nodeGrid[i, j] );

        if ( doneSearching )
		{
			return;
		}
		if( nodeGrid[i, j].loc.x == end.x && nodeGrid[i, j].loc.y == end.y )
		{
			currentPath = new List<NodeGrid>(L);
			doneSearching = true;
			return;
		}

		// First, check whether node is within grid.
		// Second, check if said node is empty or not.
		// Third, check if node has already been visited.
		if( end.y > nodeGrid[i, j].loc.y ) // Then first search upwards.
		{
            // If it is not off the grid, is empty, and is not already in the list
            if ((j + 1) < gridHeight && nodeGrid[i, j + 1].IsTraversable() && !L.Contains(nodeGrid[i, j + 1]))
			{
				SearchForPath(i, j+1, end, L);
			}
			// Now check which horizontal direction is most likely best.
			if( end.x > nodeGrid[i, j].loc.x )
			{
                if ((i + 1) < gridWidth && nodeGrid[i + 1, j].IsTraversable() && !L.Contains(nodeGrid[i + 1, j]))
					SearchForPath(i+1, j, end, L);
                if ((i - 1) >= 0 && nodeGrid[i - 1, j].IsTraversable() && !L.Contains(nodeGrid[i - 1, j]))
					SearchForPath(i-1, j, end, L);
			}
			else
			{
                if ((i - 1) >= 0 && nodeGrid[i - 1, j].IsTraversable() && !L.Contains(nodeGrid[i - 1, j]))
					SearchForPath(i-1, j, end, L);
                if ((i + 1) < gridWidth && nodeGrid[i + 1, j].IsTraversable() && !L.Contains(nodeGrid[i + 1, j]))
					SearchForPath(i+1, j, end, L);
			}
            if ((j - 1) >= 0 && nodeGrid[i, j - 1].IsTraversable() && !L.Contains(nodeGrid[i, j - 1]))
				SearchForPath(i, j-1, end, L);
		}
		else if( end.y < nodeGrid[i, j].loc.y ) // Prioritize downwards first.
		{
            if ((j - 1) >= 0 && nodeGrid[i, j - 1].IsTraversable() && !L.Contains(nodeGrid[i, j - 1]))
				SearchForPath(i, j-1, end, L);
			// Now check which horizontal direction is most likely best.
			if( end.x > nodeGrid[i, j].loc.x )
			{
                if ((i + 1) < gridWidth && nodeGrid[i + 1, j].IsTraversable() && !L.Contains(nodeGrid[i + 1, j]))
					SearchForPath(i+1, j, end, L);
                if ((i - 1) >= 0 && nodeGrid[i - 1, j].IsTraversable() && !L.Contains(nodeGrid[i - 1, j]))
					SearchForPath(i-1, j, end, L);
			}
			else
			{
                if ((i - 1) >= 0 && nodeGrid[i - 1, j].IsTraversable() && !L.Contains(nodeGrid[i - 1, j]))
					SearchForPath(i-1, j, end, L);
                if ((i + 1) < gridWidth && nodeGrid[i + 1, j].IsTraversable() && !L.Contains(nodeGrid[i + 1, j]))
					SearchForPath(i+1, j, end, L);
			}
            if ((j + 1) < gridHeight && nodeGrid[i, j + 1].IsTraversable() && !L.Contains(nodeGrid[i, j + 1]))
				SearchForPath(i, j+1, end, L);
		}
		else // Already at x, only check 
		{
			// Now check which horizontal direction is most likely best.
			if( end.x > nodeGrid[i, j].loc.x )
			{
                if ((i + 1) < gridWidth && nodeGrid[i + 1, j].IsTraversable() && !L.Contains(nodeGrid[i + 1, j]))
					SearchForPath(i+1, j, end, L);
				
				// In case it's a situation where you can only move up or down (blocked by obstacles), but on
				// same x-plane as target, go up and then check down. I arbitrarily chose up first.
                if ((j + 1) < gridHeight && nodeGrid[i, j + 1].IsTraversable() && !L.Contains(nodeGrid[i, j + 1]))
					SearchForPath(i, j+1, end, L);
                if ((j - 1) >= 0 && nodeGrid[i, j - 1].IsTraversable() && !L.Contains(nodeGrid[i, j - 1]))
					SearchForPath(i, j-1, end, L);
                if ((i - 1) >= 0 && nodeGrid[i - 1, j].IsTraversable() && !L.Contains(nodeGrid[i - 1, j]))
					SearchForPath(i-1, j, end, L);
			}
			else
			{
                if ((i - 1) >= 0 && nodeGrid[i - 1, j].IsTraversable() && !L.Contains(nodeGrid[i - 1, j]))
					SearchForPath(i-1, j, end, L);
				
				// In case it's a situation where you can only move up or down (blocked by obstacles), but on
				// same x-plane as target, go up and then check down. I arbitrarily chose up first.
                if ((j + 1) < gridHeight && nodeGrid[i, j + 1].IsTraversable() && !L.Contains(nodeGrid[i, j + 1]))
					SearchForPath(i, j+1, end, L);
                if ((j - 1) >= 0 && nodeGrid[i, j - 1].IsTraversable() && !L.Contains(nodeGrid[i, j - 1]))
					SearchForPath(i, j-1, end, L);
                if ((i + 1) < gridWidth && nodeGrid[i + 1, j].IsTraversable() && !L.Contains(nodeGrid[i + 1, j]))
					SearchForPath(i+1, j, end, L);
			}
		}
	}

    // Clear out the node for other objects to occupy
	public void EmptyOutNode( float x, float y )
	{
		int[] ij = FindClosestCoords( x, y );

		nodeGrid[ ij[0], ij[1] ].empty = true;
		nodeGrid[ ij[0], ij[1] ].containsPerson = false;
		nodeGrid[ ij[0], ij[1] ].occupyingObject = null;
	}
	
    // Fill up node with object so others can't occupy
	public void FillUpNode( float x, float y, GameObject obj )
	{
		int[] ij = FindClosestCoords( x, y );

        nodeGrid[ ij[0], ij[1] ].empty = false;
		nodeGrid[ ij[0], ij[1] ].occupyingObject = obj;
        if(null != obj.GetComponent<PersonInfo>())
        {
            nodeGrid[ij[0], ij[1]].containsPerson = true;
        }
    }

	// Draw squares around each node, showing the entire grid.
	void DebugDrawGrid()
	{
		Vector3 start, end;

		for( int i = 0; i < gridWidth; i++ )
		{
			for( int j = 0; j < gridHeight; j++ )
			{
				// Draw left line
				start = new Vector3(nodeGrid[i,j].loc.x - nodeSize/2, nodeGrid[i,j].loc.y - nodeSize/2, 0.0f);
				end = new Vector3(nodeGrid[i,j].loc.x - nodeSize/2, nodeGrid[i,j].loc.y + nodeSize/2, 0.0f);
				Debug.DrawLine(start, end, Color.green, 300.0f);
				
				// Draw top line
				start = new Vector3(nodeGrid[i,j].loc.x - nodeSize/2, nodeGrid[i,j].loc.y + nodeSize/2, 0.0f);
				end = new Vector3(nodeGrid[i,j].loc.x + nodeSize/2, nodeGrid[i,j].loc.y + nodeSize/2, 0.0f);
				Debug.DrawLine(start, end, Color.green, 300.0f);
				
				// Draw right line
				start = new Vector3(nodeGrid[i,j].loc.x + nodeSize/2, nodeGrid[i,j].loc.y + nodeSize/2, 0.0f);
				end = new Vector3(nodeGrid[i,j].loc.x + nodeSize/2, nodeGrid[i,j].loc.y - nodeSize/2, 0.0f);
				Debug.DrawLine(start, end, Color.green, 300.0f);
				
				// Draw bottom line
				start = new Vector3(nodeGrid[i,j].loc.x + nodeSize/2, nodeGrid[i,j].loc.y - nodeSize/2, 0.0f);
				end = new Vector3(nodeGrid[i,j].loc.x - nodeSize/2, nodeGrid[i,j].loc.y - nodeSize/2, 0.0f);
				Debug.DrawLine(start, end, Color.green, 300.0f);
			}
		}
	}

    // Get gameobjects in square from a,b to x,y and return them
    public List<GameObject> GetObjectsFromGrid(float a, float b, float x, float y)
    {
        int[] ab = FindClosestCoords(a, b);
        int[] xy = FindClosestCoords(x, y);

        return GetObjectsWithinCoords(ab[0], ab[1], xy[0], xy[1]);
    }

    // Get objects in region:
    //         x x x
    //         x O x
    //         x x x
    public List<GameObject> GetObjectsSurroundingCoords(float x, float y)
    {
        int[] ij = FindClosestCoords(x, y);

        // Check if these nodes are accessible or not
        int leftMost = ij[0] > 0 ? ij[0] - 1 : 0;
        int rightMost = ij[0] < gridWidth - 1 ? ij[0] + 1 : gridWidth - 1;
        int bottomMost = ij[1] > 0 ? ij[1] - 1 : 0;
        int topMost = ij[1] < gridHeight - 1 ? ij[1] + 1 : gridHeight - 1;

        return GetObjectsWithinCoords(leftMost, topMost, rightMost, bottomMost);
    }

    public List<GameObject> GetObjectsWithinCoords(int a1, int b1, int a2, int b2)
    {
        List<GameObject> objects = new List<GameObject>();

        // This goes up because x increases to the right
        for (int i = a1; i <= a2; i++)
        {
            // This goes down because y increases going up
            for (int j = b1; j >= b2; j--)
            {
                NodeGrid pointNode = nodeGrid[i, j];
                GameObject tempObject = pointNode.occupyingObject;
                if (null != tempObject)
                {
                    objects.Add(tempObject);
                }
            }
        }

        return objects;
    }

    public void ShowOrangeSquaresSurroundingLocation(float x, float y, float duration)
    {
        currentHighlightTimer = duration;
        GameObject leftSquare = null, topSquare = null, rightSquare = null, bottomSquare = null;
        float zPosition = 20.0f;
        int[] ij = FindClosestCoords(x, y);

        var middleSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
        middleSquare.transform.position = new Vector3(x, y, zPosition);
        currentHighlightSquares.Add(middleSquare);

        if (ij[0] > 0)  // Left
        {
            leftSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0] - 1, ij[1]);
            leftSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(leftSquare);
        }

        if (ij[1] < (gridHeight-1))  // Top
        {
            topSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0], ij[1] + 1);
            topSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(topSquare);
        }

        if (ij[0] < (gridWidth - 1))  // Right
        {
            rightSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0] + 1, ij[1]);
            rightSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(rightSquare);
        }

        if (ij[1] > 0)  // Bottom
        {
            bottomSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0], ij[1] - 1);
            bottomSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(bottomSquare);
        }

        if (leftSquare && topSquare)
        {
            var topLeftSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0] - 1, ij[1] + 1);
            topLeftSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(topLeftSquare);
        }

        if (rightSquare && topSquare)
        {
            var topRightSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0] + 1, ij[1] + 1);
            topRightSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(topRightSquare);
        }

        if (rightSquare && bottomSquare)
        {
            var bottomLeftSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0] + 1, ij[1] - 1);
            bottomLeftSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(bottomLeftSquare);
        }

        if (leftSquare && bottomSquare)
        {
            var bottomLeftSquare = GameObject.Instantiate(Resources.Load("Prefabs/OrangeSquare") as GameObject);
            var coords = ConvertIJtoXY(ij[0] - 1, ij[1] - 1);
            bottomLeftSquare.transform.position = new Vector3(coords[0], coords[1], zPosition);
            currentHighlightSquares.Add(bottomLeftSquare);
        }
    }
}
