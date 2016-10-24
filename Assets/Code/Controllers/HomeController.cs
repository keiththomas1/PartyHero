using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour 
{
	Camera mainCamera;
	GameObject currentPartyBox;

	// Use this for initialization
	void Start () 
	{
		mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( Input.GetMouseButtonDown(0) )
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
		
			if( Physics.Raycast(ray,out hit) )
			{
				switch( hit.collider.name )
				{
                case "GoToPartyButton":
                    StartParty();
                    break;
                case "House1":
                    ResetPartyText();
                    ShowPartyText(1);
                    break;
                default:
                    ResetPartyText();
                    break;
                }
			}
            else
            {
                ResetPartyText();
            }
		}
	}

    void ShowPartyText(int index)
    {
		currentPartyBox = GameObject.Instantiate (Resources.Load ("Prefabs/Popups/PartyBox") as GameObject);
		currentPartyBox.transform.position = new Vector3 (0.4f, 0.1f, -4.0f);
    }
    void ResetPartyText()
    {
		if (currentPartyBox) {
			GameObject.Destroy (currentPartyBox);
		}
    }

    void StartParty()
	{
		SceneManager.LoadScene("LoadingParty");
    }
}
