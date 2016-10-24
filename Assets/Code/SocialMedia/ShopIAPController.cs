using UnityEngine;
using System.Collections;

public class ShopIAPController : MonoBehaviour
{
    private GameObject screenObject;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EnterScreen()
    {
        screenObject = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Shop/IAPScreen") as GameObject);
    }

    public void ExitScreen()
    {
        if (screenObject)
        {
            GameObject.Destroy(screenObject);
        }
    }
}
