using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaryJaneInfo : InteractableInfo
{
	public GameObject closestDoor;
    string currentActivity;
    bool inRoom;
    float fadeSpeed;
    Material material;
    Material normalMaterial;
    Material alphaMaterial;

	float inRoomTimer;
	float smokeAnimationTimer;

    // Use this for initialization
    void Start()
    {
        base.BaseInit();
        loc = new Location(transform.position.x, transform.position.y);
        commands = new List<string>();

        commands.Add("Hang Out");

        commandAmount = commands.Count;

        fadeSpeed = 2.0f;
        material = this.GetComponent<Renderer>().material;
        normalMaterial = new Material(material);
        alphaMaterial = new Material(material);
        var tempColor = material.color;
        tempColor.a = 0.0f;
        alphaMaterial.color = tempColor;
		inRoomTimer = 0.0f;
		smokeAnimationTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (inRoom)
        {
			if (smokeAnimationTimer > 0.0f) {
				smokeAnimationTimer -= Time.deltaTime;

				if (smokeAnimationTimer <= 0.0f) {
					closestDoor.GetComponent<Animator> ().Play ("Smoking");
				}
			}
            if (inRoomTimer > 0.0f)
            {
                inRoomTimer -= Time.deltaTime;

                if (null != material)
                {
                    material.Lerp(material, alphaMaterial, Time.deltaTime * fadeSpeed);
                }
            }
            else
            {
                if (null != material)
                {
                    material.Lerp(material, normalMaterial, Time.deltaTime * fadeSpeed);
                }

                if (material.color.a >= .99f)
                {
                    // ReadyToMove();
                }
            }
        }
    }

    int EnterRoom(bool success)
    {
        if (success)
        {
            hero.EnterTheRoom(currentActivity, 3.0f);

            inRoom = true;
            inRoomTimer = 3.0f;
			smokeAnimationTimer = 1.5f;
        }

        return 0;
    }

    public override string GetName()
    {
        return "Mary Jane";
    }

    public override void Selection1()
    {
        currentActivity = "Weed";
        NodeGrid pointNode = grid.FindClosestNode(transform.position.x - 2.0f, transform.position.y);
        if (pointNode != null && (pointNode.IsEmpty() || pointNode.occupyingObject == hero.gameObject))
        {
            hero.StartPath(pointNode, EnterRoom);
        }
    }
}
