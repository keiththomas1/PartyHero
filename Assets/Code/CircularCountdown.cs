/*using UnityEngine;
using System.Collections;

public class CircularCountdown : MonoBehaviour {
    float rate = 0.3f;
    float i = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        i += Time.deltaTime * rate;
        this.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", i);
    }
}*/

using UnityEngine;
using System.Collections;
 
public class CircularCountdown : MonoBehaviour
{

    float timeToComplete = 3;
 
// Use this for initialization
void Start()
    {
        //Use this to Start progress
        StartCoroutine(RadialProgress(timeToComplete));
    }

IEnumerator RadialProgress(float time)
    {
        float rate = 1 / time;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * rate;
            this.GetComponent<SpriteRenderer>().material.SetFloat("_Cutoff", i);
            yield return 0;
        }
    }
}