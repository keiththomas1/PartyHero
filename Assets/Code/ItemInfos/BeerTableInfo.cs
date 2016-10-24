using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeerTableInfo : InteractableInfo 
{
	InterfaceController uiController;

	// Use this for initialization
	void Start ()
    {
        base.BaseInit();
        uiController = GameObject.Find("MainController").GetComponent<InterfaceController>();
		loc = new Location( transform.position.x, transform.position.y );
		commands = new List<string>();

        commands.Add("Drink");
        commandAmount = commands.Count;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    int DrinkBeer(bool success)
    {
        if (success)
        {
            hero.GetComponent<Animator>().Play("DrinkingBeer");
            uiController.DrinkBeer(10);
            uiController.AddDrinkingExperience(10);
        }
        return 0;
    }

    public override string GetName()
    {
        return "Beers";
    }

    public override void Selection1()
    {
        PerformCommandAfterPath(DrinkBeer);
	}
}
