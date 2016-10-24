using UnityEngine;
using System.Collections;

public class ShopVisualsController : MonoBehaviour
{
    private GameObject screenObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckUserInput()
    {
    }

    public void EnterScreen()
    {
        screenObject = GameObject.Instantiate(Resources.Load("Prefabs/SocialMedia/Shop/VisualsScreen") as GameObject);
    }

    public void ExitScreen()
    {
        if (screenObject)
        {
            GameObject.Destroy(screenObject);
        }
    }
}
