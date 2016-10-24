using UnityEngine;

public class HubController : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckUserInput();
    }

    public void LeaveApp()
    {
        DestroyCurrentScreen();
    }

    public bool BackOut()
    {
        return true;
    }

    public void CheckUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                }
            }
        }
    }

    private void DestroyCurrentScreen()
    {
    }
}
