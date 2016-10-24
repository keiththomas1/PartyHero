using UnityEngine;
using System.Collections;

public class Location 
{
	public float x;
	public float y;

	// Perhaps variables for actual pixel or camera locations.
	
	public Location()
	{
		x = 0.0f;
		y = 0.0f;
	}

	public Location(float a, float b)
	{
		x = a;
		y = b;
	}
}
