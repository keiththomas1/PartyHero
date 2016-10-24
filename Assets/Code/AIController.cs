using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct PersonLocation
{
    string name;
    Location location;

    public PersonLocation(string name_, Location location_)
    {
        name = name_;
        location = location_;
    }
}

public class AIController : MonoBehaviour 
{
    private List<PersonLocation> people;

	// Use this for initialization
	void Start () 
    {
        people = new List<PersonLocation>();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject o in objects) {
            people.Add(new PersonLocation(o.name,
                new Location(o.transform.localPosition.x, o.transform.localPosition.y)));
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
