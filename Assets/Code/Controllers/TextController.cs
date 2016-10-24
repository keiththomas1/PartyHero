using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    GameObject currentText;
    RawImage outcomeTextImage;
    Text outcomeTextText;
    Camera mainCamera;

	// Use this for initialization
	void Start () 
    {
        var outcomeImage = GameObject.Find("OutcomeText");
        if (!outcomeImage)
        {
            Debug.Log("Error finding outcome image");
        }
        outcomeTextImage = outcomeImage.GetComponent<RawImage>();
        var outcomeText = outcomeImage.transform.Find("Text");
        if (!outcomeText)
        {
            Debug.Log("Error finding outcome text");
        }
        outcomeTextText = outcomeText.GetComponent<Text>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void ShowText(string category)
    {
        ChooseText(category);
    }
    public void DestroyText()
    {
        outcomeTextImage.enabled = false;
        outcomeTextText.enabled = false;
    }

    void ChooseText(string category)
    {
        switch( category )
        {
            case "Weed":
                outcomeTextImage.enabled = true;
                outcomeTextText.enabled = true;

                var rand = Random.Range(1, 4);
                switch (rand)
                {
                    case 1:
                        outcomeTextText.text = "You exit in a smoky haze,\n" +
                            "wondering simultaneously why it is that\n" +
                            "the universe is so large and why\n" +
                            "it is that pizza rolls are so delicious.";
                        break;
                    case 2:
                        outcomeTextText.text = "You exit in a smoky haze,\n" +
                            "wondering simultaneously why it is that\n" +
                            "the universe is so large and why\n" +
                            "it is that pizza rolls are so delicious.";
                        break;
                    case 3:
                        outcomeTextText.text = "You exit in a smoky haze,\n" +
                            "wondering simultaneously why it is that\n" +
                            "the universe is so large and why\n" +
                            "it is that pizza rolls are so delicious.";
                        break;
                    case 4:
                        outcomeTextText.text = "You exit in a smoky haze,\n" +
                            "wondering simultaneously why it is that\n" +
                            "the universe is so large and why\n" +
                            "it is that pizza rolls are so delicious.";
                        break;
                    default:
                        break;
                }

                break;
            default:
                break;
        }
    }
}
