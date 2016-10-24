using UnityEngine;

public class SettingsController : MonoBehaviour {
    private GlobalVars globalVars;

	// Use this for initialization
	void Start () {
        globalVars = GlobalVars.Instance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LeaveApp()
    {

    }

    public bool BackOut()
    {
        return true;
    }

}
