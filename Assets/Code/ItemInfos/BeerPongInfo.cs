using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BeerPongInfo : InteractableInfo
{
    public GameObject[] highlightedSquares;

    InputController ioController;
    InterfaceController uiController;
    Camera mainCamera;

    bool celebShotStarted;
    float celebShotTimer;
	
	// Use this for initialization
	void Start () 
	{
        base.BaseInit();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
        uiController = GameObject.Find("MainController").GetComponent<InterfaceController>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		loc = new Location( transform.position.x, transform.position.y );
		commands = new List<string>();

        commands.Add( "Play" );
		commands.Add( "Celeb Shot" );
        commandAmount = commands.Count;

        ResetVariables();

        UnhighlightSquares();
	}

    void ResetVariables()
    {
        celebShotStarted = false;
        celebShotTimer = 1.6f;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if( celebShotStarted )
        {
            celebShotTimer -= Time.deltaTime;
            if( celebShotTimer <= 0.0f )
            {
                ResetVariables();
                StartCelebShot();
            }
            Camera.main.orthographicSize -= Time.deltaTime * .2f;
        }
    }

    public override string GetName()
    {
        return "Beer Pong";
    }

    public override void Selected()
    {
        foreach (GameObject square in highlightedSquares)
        {
            square.GetComponent<Renderer>().enabled = true;
        }
    }
    public override void Unselected()
    {
        UnhighlightSquares();
    }
    void UnhighlightSquares()
    {
        foreach (GameObject square in highlightedSquares)
        {
            square.GetComponent<Renderer>().enabled = false;
        }
    }

    int StartBeerPongGame(bool success)
    {
        if (success)
        {
            uiController.StartBeerPongMiniGame();
        }
        return 0;
    }

    public override void Selection1()
    {
        PerformCommandAfterPath(StartBeerPongGame);
    }

    int PrepareCelebShot(bool success)
    {
        if (success)
        {
            uiController.BrownOut();
            celebShotStarted = true;
        }
        return 0;
    }

    void StartCelebShot()
    {
        Vector3 t = new Vector3(transform.position.x, transform.position.y, -10.0f);
        mainCamera.transform.position = t;
        Camera.main.orthographicSize = 6;

        ioController.DisableClicking();
        ioController.DisableScrolling();
    }
}
