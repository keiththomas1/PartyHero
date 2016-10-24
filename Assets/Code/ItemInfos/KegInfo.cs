using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KegInfo : InteractableInfo
{
    InputController ioController;
    InterfaceController uiController;
    Camera mainCamera;
    public Sprite kegStand;
    public GameObject countdown;
    public GameObject stopButton;

    // Timers for kegstand
    bool preparingKegStand;
    bool settingUpKegStand;
    float prepareTimer;
    float startTimer;
    bool drinking;
    float drinkingTimer;
    float pointsTimer;
    int pointsCounter;
    Vector3 heroLocation;

	// Use this for initialization
	void Start ()
    {
        base.BaseInit();
        ioController = GameObject.Find("MainController").GetComponent<InputController>();
        uiController = GameObject.Find("MainController").GetComponent<InterfaceController>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        loc = new Location(transform.position.x, transform.position.y);
        commands = new List<string>();

        commands.Add("Keg Stand!");
        commands.Add("Drink a Beer");
        commandAmount = commands.Count;

        ResetVariables();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (preparingKegStand)
        {
            prepareTimer -= Time.deltaTime;

            if (prepareTimer <= 0.0f)
            {
                preparingKegStand = false;
                settingUpKegStand = true;
                SetUpKegStand();
            }
        }
        if (settingUpKegStand)
        {
            startTimer -= Time.deltaTime;

            if (startTimer <= 0.0f)
            {
                settingUpKegStand = false;
                StartKegStand();
            }
        }
        if( drinking )
        {
            drinkingTimer -= Time.deltaTime;

            if( drinkingTimer <= 0.0f )
            {
                drinkingTimer = .2f;
                uiController.DrinkBeer(2);
            }

            pointsTimer -= Time.deltaTime;

            if (pointsTimer <= 0.0f)
            {
                pointsTimer = 1.0f;
                pointsCounter++;

                var newPoints = 10 + (pointsCounter * 5);
                uiController.AddPartyPoints(
                    newPoints, new Vector3(transform.position.x, transform.position.y + 1.0f, -2.0f));
            }


            // If a tap, find what was clicked on and act on it
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    switch (hit.collider.name)
                    {
                        case "StopKegStand":
                            StopKegStand();
                            break;
                    }
                }
            }
        }
	}

    void ResetVariables()
    {
        preparingKegStand = false;
        settingUpKegStand = false;
        prepareTimer = 1.6f;
        startTimer = 1.0f;
        drinking = false;
        drinkingTimer = .2f;
        pointsTimer = 1.0f;
        pointsCounter = 0;
    }

    int PrepareKegStand(bool successful)
    {
        if( successful )
        {
            uiController.BrownOut();

            ioController.DisableClicking();
            ioController.DisableScrolling();
            preparingKegStand = true;
        }
        return 0;
    }

    void SetUpKegStand()
    {
        // Change keg to keg stand sprite (maybe animation in future)
        this.GetComponent<Animator>().Play("Drinking");
        this.GetComponent<Animator>().speed = 0.0f;
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        heroLocation = hero.transform.position;
        hero.transform.position = new Vector3(0.0f, -40.0f, 0.0f);
    }

    void StartKegStand()
    {
        this.GetComponent<Animator>().Play("Drinking");
        this.GetComponent<Animator>().speed = 1.0f;
        stopButton.transform.position = new Vector3(transform.position.x - .9f,
                                            transform.position.y - 2.3f,
                                            -8.0f);
        countdown.transform.position = new Vector3(transform.position.x - .9f,
                                            transform.position.y + 5.4f,
                                            -5.0f);
        countdown.GetComponent<TextMesh>().text = "1";
        countdown.GetComponent<Animator>().Play("Countdown");

        drinking = true;
    }

    void StopKegStand()
    {
        this.GetComponent<Animator>().Play("Idle");
        hero.transform.position = heroLocation;
        countdown.transform.position = new Vector3(0.0f, -40.0f, 0.0f);
        stopButton.transform.position = new Vector3(0.0f, -40.0f, 0.0f);
        ioController.EnableClicking();
        ioController.EnableScrolling();
        ResetVariables();

        uiController.AddDrinkingExperience(50);
    }

    public override string GetName()
    {
        return "Keg";
    }

    public override void Selection1()
    {
        PerformCommandAfterPath(PrepareKegStand);
    }

    public override void Selection2()
    {
        PerformCommandAfterPath(DrinkBeer);
    }

    

    int DrinkBeer(bool success)
    {
        if( success )
        {
            hero.GetComponent<Animator>().Play("DrinkingBeer");
            uiController.DrinkBeer(10);
            uiController.AddDrinkingExperience(10);
        }
        return 0;
    }
}
