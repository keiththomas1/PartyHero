using UnityEngine;
using System.Collections;

public class RandomUpwardsDrift : MonoBehaviour {
    private Vector2 moveVector;

	// Use this for initialization
	void Start () {
        moveVector = new Vector2();
        var randomX = Random.Range(0, 2);
        if (randomX == 0)
        {
            moveVector.x = -0.01f;
        }
        else
        {
            moveVector.x = 0.01f;
        }

        moveVector.y = 0.09f;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(moveVector);
        moveVector.x = moveVector.x * 1.01f;
        moveVector.y = moveVector.y * .95f;
	}
}
