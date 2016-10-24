using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowGenerator : MonoBehaviour
{
    const float defaultSpawnTime = 0.5f;
    float spawnTimer;

    List<GameObject> leftArrows;
    List<GameObject> upArrows;
    List<GameObject> downArrows;
    List<GameObject> rightArrows;

    // Use this for initialization
    void Start()
    {
        spawnTimer = 0.1f;
        leftArrows = new List<GameObject>();
        upArrows = new List<GameObject>();
        downArrows = new List<GameObject>();
        rightArrows = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer > 0.0f)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0.0f)
            {
                var rand = Random.Range(0, 3);
                switch (rand)
                {
                    case 0:
                        var leftArrow = GameObject.Instantiate(Resources.Load("Prefabs/Dance/LeftArrow") as GameObject);
                        leftArrow.GetComponent<ArrowBehavior>().Initialize(this, "Left");
                        leftArrows.Add(leftArrow);
                        break;
                    case 1:
                        var upArrow = GameObject.Instantiate(Resources.Load("Prefabs/Dance/UpArrow") as GameObject);
                        upArrow.GetComponent<ArrowBehavior>().Initialize(this, "Up");
                        upArrows.Add(upArrow);
                        break;
                    case 2:
                        var downArrow = GameObject.Instantiate(Resources.Load("Prefabs/Dance/DownArrow") as GameObject);
                        downArrow.GetComponent<ArrowBehavior>().Initialize(this, "Down");
                        downArrows.Add(downArrow);
                        break;
                    case 3:
                        var rightArrow = GameObject.Instantiate(Resources.Load("Prefabs/Dance/RightArrow") as GameObject);
                        rightArrow.GetComponent<ArrowBehavior>().Initialize(this, "Right");
                        rightArrows.Add(rightArrow);
                        break;
                }

                spawnTimer = defaultSpawnTime;
            }
        }
    }

    public GameObject GetBottomArrow(string direction)
    {
        switch(direction)
        {
            case "Left":
                if (leftArrows.Count > 0)
                    return leftArrows[0];
                else
                    return null;
            case "Up":
                if (upArrows.Count > 0)
                    return upArrows[0];
                else
                    return null;
            case "Right":
                if (rightArrows.Count > 0)
                    return rightArrows[0];
                else
                    return null;
            case "Down":
                if (downArrows.Count > 0)
                    return downArrows[0];
                else
                    return null;
            default:
                Debug.Log("Incorrect direction");
                return null;
        }
    }

    public void RemoveArrow(GameObject arrow, string direction)
    {
        switch (direction)
        {
            case "Left":
                leftArrows.Remove(arrow);
                break;
            case "Up":
                upArrows.Remove(arrow);
                break;
            case "Right":
                rightArrows.Remove(arrow);
                break;
            case "Down":
                downArrows.Remove(arrow);
                break;
            default:
                Debug.Log("Incorrect direction");
                break;
        }
    }
}
