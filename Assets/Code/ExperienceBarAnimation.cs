using UnityEngine;
using System.Collections;

public class ExperienceBarAnimation : MonoBehaviour {
    float startingAnimationTimer = 0.0f;
    const float defaultStartTime = 0.4f;

    float experienceTickTimer = 0.0f;
    const float defaultTickTime = 0.01f;

    float deathTimer = 0.0f;
    const float defaultDeathTime = 0.4f;

    TextMesh experienceText;
    Transform experienceBar;
    float experienceBarWidth;

    float currentExperiencePoints;
    float targetExperiencePoints;
    float totalExperiencePoints;

    // Use this for initialization
    void Awake () {
        experienceText = transform.Find("ExperienceText").GetComponent<TextMesh>();
        experienceBar = transform.Find("ExperienceBar");
        experienceBarWidth = experienceBar.localScale.x;
    }
	
	// Update is called once per frame
	void Update () {
        if (startingAnimationTimer > 0.0f)
        {
            startingAnimationTimer -= Time.deltaTime;

            if (startingAnimationTimer <= 0.0f)
            {
                experienceTickTimer = defaultTickTime;
            }
        }

        if (experienceTickTimer > 0.0f)
        {
            experienceTickTimer -= Time.deltaTime;

            if (experienceTickTimer <= 0.0f)
            {
                experienceTickTimer = defaultTickTime;

                currentExperiencePoints++;
                UpdateExperienceObject();

                if (currentExperiencePoints >= targetExperiencePoints)
                {
                    experienceTickTimer = 0.0f;
                    deathTimer = defaultDeathTime;
                }
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

    public void StartAnimation(float oldExperiencePoints, float newExperiencePoints, float neededExperiencePoints)
    {
        currentExperiencePoints = oldExperiencePoints;
        targetExperiencePoints = newExperiencePoints;
        totalExperiencePoints = neededExperiencePoints;
        startingAnimationTimer = defaultStartTime;

        var newExperienceBar = transform.Find("NewExperienceBar");
        if (newExperienceBar)
        {
            newExperienceBar.localScale = new Vector3(
                (float)newExperiencePoints / neededExperiencePoints * newExperienceBar.localScale.x,
                newExperienceBar.localScale.y,
                1.0f);
        }

        UpdateExperienceObject();
    }

    void UpdateExperienceObject()
    {
        experienceBar.localScale = new Vector3(
            (float)currentExperiencePoints / totalExperiencePoints * experienceBarWidth,
            experienceBar.localScale.y,
            1.0f);

        experienceText.GetComponent<TextMesh>().text =
            currentExperiencePoints.ToString() + " / " + totalExperiencePoints.ToString();
    }
}
