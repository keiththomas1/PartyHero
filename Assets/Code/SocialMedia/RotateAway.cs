using UnityEngine;

public class RotateAway : MonoBehaviour {
    private bool started;
    private float incrementTimer;
    private Vector2 directionVector;
    private float rotationSpeed;

	void Start () {
        started = false;
        directionVector = new Vector2(Random.value - 0.5f, Random.value);
        directionVector.Normalize();
        directionVector /= 10;

        if (directionVector.x < 0.0f)
        {
            rotationSpeed = 0.5f;
        }
        else
        {
            rotationSpeed = -0.5f;
        }
	}
	
	void Update () {
        if (started)
        {
            transform.Translate(directionVector);
            transform.Rotate(0.0f, 0.0f, rotationSpeed);
        }
	}

    public void StartAnimation() {
        started = true;
    }
}
