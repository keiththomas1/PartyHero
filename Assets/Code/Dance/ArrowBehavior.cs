using UnityEngine;
using System.Collections;

public class ArrowBehavior : MonoBehaviour {
    string direction = "";
    ArrowGenerator arrowGenerator;
    Vector2 movement = new Vector2(0.0f, -0.07f);

    bool active;
    float deathTimer;
    const float defaultDeathTime = 5.0f;

	// Use this for initialization
	void Start () {
        active = true;
        deathTimer = 0.0f;
    }
	
    public void Initialize(ArrowGenerator generator, string direction_)
    {
        arrowGenerator = generator;
        direction = direction_;
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(movement);

        if (active && transform.position.y < -4.5f)
        {
            active = false;
            deathTimer = defaultDeathTime;
            if (arrowGenerator)
            {
                arrowGenerator.RemoveArrow(this.gameObject, direction);
            }
        }

        if (deathTimer > 0.0f)
        {
            deathTimer -= Time.deltaTime;

            if (deathTimer <= 0.0f)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
	}
}
