using UnityEngine;
using System.Collections;

public class NPC 
{
    public GameObject objectHandle;
	public string name;
    public string musicPreference;
    public string gendersInterestedIn;

    public NPC()
    {
        name = "";
        musicPreference = "";
        gendersInterestedIn = "";
        // this.gameObject.tag = "NPC";
    }
}
