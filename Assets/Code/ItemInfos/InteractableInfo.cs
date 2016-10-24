using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InteractableInfo : MonoBehaviour
{
    /* Properties */
	public Location loc;
	public List<string> commands;
	public int commandAmount;

    protected HeroController hero;
    protected GridController grid;

    protected void BaseInit()
    {
        hero = GameObject.Find("Hero").GetComponent<HeroController>();
        grid = GameObject.Find("MainController").GetComponent<GridController>();
    }
	
	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    public virtual string GetName()
    {
        return "";
    }

	public virtual void Selection1()
	{
		Debug.Log("InteractableInfo Super selection1()");
	}
	public virtual void Selection2()
	{
		Debug.Log("InteractableInfo Super selection2()");
	}
	public virtual void Selection3()
	{
		Debug.Log("InteractableInfo Super selection3()");
	}
	public virtual void Selection4()
	{
		Debug.Log("InteractableInfo Super selection4()");
	}
    public virtual void Selected()
    {

    }
    public virtual void Unselected()
    {

    }
    protected void PerformCommandAfterPath(Func<bool, int> func)
    {
        var pointNode = grid.FindClosestAdjacentNode(
            transform.position.x, transform.position.y, hero.transform.position);
        if (pointNode != null)
        {
            hero.GetComponent<HeroController>().StartPath(pointNode, func);
        }
    }
}
