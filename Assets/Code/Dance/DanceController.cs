using UnityEngine;
using System.Collections;

public class DanceController : MonoBehaviour
{
    public GameObject hero;
    ArrowGenerator arrowGenerator;
    SpriteRenderer heroSprite;

    // For now static sprite switches
    // In the future - switch the hero to a new animation
    public Sprite heroIdleSprite;
    public Sprite heroLeftDance;
    public Sprite heroDownDance;
    public Sprite heroUpDance;
    public Sprite heroRightDance;

    public GameObject leftDanceArrow;
    public GameObject downDanceArrow;
    public GameObject upDanceArrow;
    public GameObject rightDanceArrow;

    public GameObject comboText;
    public GameObject descriptionText;

    const float defaultDanceAnimationLength = 1.5f;
    float danceAnimationTimer;

    int currentCombo;

    // Use this for initialization
    void Start () {
        arrowGenerator = GetComponent<ArrowGenerator>();
        heroSprite = hero.GetComponent<SpriteRenderer>();
        danceAnimationTimer = 0.0f;
        currentCombo = 0;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckHit("Left");
            heroSprite.sprite = heroLeftDance;
            danceAnimationTimer = defaultDanceAnimationLength;
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckHit("Right");
            rightDanceArrow.GetComponent<Animator>().Play("Hit");
            heroSprite.sprite = heroRightDance;
            danceAnimationTimer = defaultDanceAnimationLength;
        } else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CheckHit("Up");
            upDanceArrow.GetComponent<Animator>().Play("Hit");
            heroSprite.sprite = heroUpDance;
            danceAnimationTimer = defaultDanceAnimationLength;
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CheckHit("Down");
            downDanceArrow.GetComponent<Animator>().Play("Hit");
            heroSprite.sprite = heroDownDance;
            danceAnimationTimer = defaultDanceAnimationLength;
        }

        if (danceAnimationTimer > 0.0f)
        {
            danceAnimationTimer -= Time.deltaTime;

            if (danceAnimationTimer <= 0.0f)
            {
                heroSprite.sprite = heroIdleSprite;
            }
        }
    }

    void CheckHit(string direction)
    {
        float arrowY = 0.0f;
        switch (direction)
        {
            case "Left":
                var leftArrow = arrowGenerator.GetBottomArrow("Left");
                if (leftArrow)
                {
                    arrowY = leftArrow.transform.position.y;
                }
                break;
            case "Up":
                var upArrow = arrowGenerator.GetBottomArrow("Up");
                if (upArrow)
                {
                    arrowY = upArrow.transform.position.y;
                }
                break;
            case "Down":
                var downArrow = arrowGenerator.GetBottomArrow("Down");
                if (downArrow)
                {
                    arrowY = downArrow.transform.position.y;
                }
                break;
            case "Right":
                var rightArrow = arrowGenerator.GetBottomArrow("Right");
                if (rightArrow)
                {
                    arrowY = rightArrow.transform.position.y;
                }
                break;
            default:
                Debug.Log("Incorrect direction");
                break;
        }

        if (arrowY < -2.5f && arrowY > -3.4f)
        {
            currentCombo++;
        }
        else
        {
            currentCombo = 0;
        }

        comboText.GetComponent<TextMesh>().text = currentCombo.ToString() + " Combo!";
        leftDanceArrow.GetComponent<Animator>().Play("Hit");
    }
}
