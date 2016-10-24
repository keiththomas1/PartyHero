using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BeerPongController : MonoBehaviour
{
    InputController ioController;
    InterfaceController uiController;
    private bool[] cupsPlaced;   // For beer pong.
    private bool[] yourCupsPlaced;

    bool playersTurn;
    bool isShootingHorizontal;
	float shootVerticalTimer;	// Gives a brief pause between shooting to avoid double taps
	bool canShootVertical;
	bool isShootingVertical;

	// Concerning bouncing when cup missed.
	bool isBouncing;
    bool opponentIsBouncing;

    public GameObject ballParent;
    public GameObject opponentBallParent;
    Vector3 originalBallPosition;
    Vector3 originalBallScale;
    Vector3 opponentOriginalBallPosition;
    float opponentDestinationY;
    Vector2 ballMovement;
	Vector2 downBallMovement;
	float ballGrowRate = 1.03f;
	float ballShrinkRate = .92f;
	
	public GameObject sliderHorizontal;
    Vector3 sliderHorizontalOriginalPosition;
	public GameObject sliderVertical;
	public GameObject slideBarHorizontal;
	public GameObject slideBarVertical;
	float slideBarHorizontalActualLength;
	float slideBarVerticalActualLength;
	string sliderDirection;
	Vector2 rightSlide;
	Vector2 leftSlide;
	float sliderMultiplier;
	
	public GameObject[] cups;
    public GameObject[] playerCups;
    const float defaultCupAnimTime = 1.0f;
	float cupAnimTimer;
	float opponentCupAnimTimer;
    int cupIndex;
    int opponentCupIndex;
    float cupLeftMost;
    float cupRightMost;
    float cupTopMost;
    float cupBottomMost;

    public GameObject descriptionText;
	bool descriptionTextGrowing;
	float textGrowthTimer;
	float growthTimerRate; // A constantly decreasing number to make the growth exponential

	public GameObject ballCursor;

	public GameObject instructionText;
	
	// Sound Effects
	public GameObject inCupSFX, thrownSFX, rimJobSFX, bounceSFX;
	
	bool gameStarted;
    int cupsRemoved;

    // Variables for fading out the instructions
    float fadeTimer;
	Color colorStart;
	Color colorEnd;
	float fadeValue;

	// Variables for heating up and fire
	public GameObject heatingUpText;
	public GameObject fireText;

    float opponentShotTimer = 0.0f;
	
	// Use this for initialization
	void Start ()
    {
        var mainController = GameObject.Find("MainController");
        if (mainController)
        {
            ioController = mainController.GetComponent<InputController>();
            uiController = mainController.GetComponent<InterfaceController>();
        }

        cupLeftMost = -1.0f * (cups[0].GetComponent<Renderer>().bounds.size.x * 0.5f); // 4 / 10);
        cupRightMost = (cups[0].GetComponent<Renderer>().bounds.size.x * 0.5f); // 4 / 10);
        cupTopMost = (cups[0].GetComponent<Renderer>().bounds.size.y * 0.5f); // 9 / 20);
        cupBottomMost = (cups[0].GetComponent<Renderer>().bounds.size.y * 1 / 5); // 1 / 4);

        cupsPlaced = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            cupsPlaced[i] = true;
            var leftMost = cups[i].transform.position.x + cupLeftMost;
            var rightMost = cups[i].transform.position.x + cupRightMost;
            var bottomMost = cups[i].transform.position.y + cupBottomMost;
            var topMost = cups[i].transform.position.y + cupTopMost;
            Debug.DrawLine(new Vector2(leftMost, topMost), new Vector2(leftMost, bottomMost), Color.green, 300.0f);
            Debug.DrawLine(new Vector2(leftMost, bottomMost), new Vector2(rightMost, bottomMost), Color.green, 300.0f);
            Debug.DrawLine(new Vector2(rightMost, bottomMost), new Vector2(rightMost, topMost), Color.green, 300.0f);
            Debug.DrawLine(new Vector2(rightMost, topMost), new Vector2(leftMost, topMost), Color.green, 300.0f);
        }
        yourCupsPlaced = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            yourCupsPlaced[i] = true;
            var leftMost = playerCups[i].transform.position.x + cupLeftMost;
            var rightMost = playerCups[i].transform.position.x + cupRightMost;
            var bottomMost = playerCups[i].transform.position.y + cupBottomMost;
            var topMost = playerCups[i].transform.position.y + cupTopMost;
            Debug.DrawLine(new Vector2(leftMost, topMost), new Vector2(leftMost, bottomMost), Color.green, 300.0f);
            Debug.DrawLine(new Vector2(leftMost, bottomMost), new Vector2(rightMost, bottomMost), Color.green, 300.0f);
            Debug.DrawLine(new Vector2(rightMost, bottomMost), new Vector2(rightMost, topMost), Color.green, 300.0f);
            Debug.DrawLine(new Vector2(rightMost, topMost), new Vector2(leftMost, topMost), Color.green, 300.0f);
        }
        
        if (ballParent)
        {
            originalBallPosition = ballParent.transform.position;
            originalBallScale = ballParent.transform.localScale;

            var ball = ballParent.transform.Find("PingPongBall");
            if (ball)
            {
                ball.GetComponent<Animator>().enabled = false;
            }
        }
        if (opponentBallParent)
        {
            opponentOriginalBallPosition = opponentBallParent.transform.position;

            var ball = ballParent.transform.Find("OpponentBall");
            if (ball)
            {
                ball.GetComponent<Animator>().enabled = false;
            }
        }
        sliderHorizontalOriginalPosition = sliderHorizontal.transform.position;

        playersTurn = true;
        isShootingHorizontal = true;
		shootVerticalTimer = .2f;
		canShootVertical = false;
		isShootingVertical = false;

		isBouncing = false;
        opponentIsBouncing = false;

        descriptionText.GetComponent<Renderer>().enabled = false;
		descriptionTextGrowing = false;
		growthTimerRate = .06f;
		
		slideBarHorizontalActualLength = slideBarHorizontal.GetComponent<Renderer>().bounds.size.x * .9f; // HACK - This is just because the bar is curved.
		slideBarVerticalActualLength = slideBarVertical.GetComponent<Renderer>().bounds.size.y * .85f; // HACK - This is just because the bar is curved.
		sliderDirection = "right";
        sliderMultiplier = .01f; // Multiply this to increase the slider speed
		rightSlide = new Vector2( .1f + sliderMultiplier, 0.0f );
		leftSlide = new Vector2( -.1f - sliderMultiplier, 0.0f );

        ballMovement = new Vector2(0.0f, .1f + sliderMultiplier);

        cupsRemoved = 0;
	    for(int i=0; i<cups.Length; i++)
	    {
		    if( !cupsPlaced[i] )
		    {
		 	    cupsRemoved++;
		    }
	    }
	    if( cupsRemoved == 10 )
	    {
		    for(int i=0; i<cups.Length; i++)
		    {
		 	    cupsPlaced[i] = true;
		    }
	    }
	    else
	    {
		    for(int i=0; i<cups.Length; i++)
		    {
		 	    if( !cupsPlaced[i] )
		 	    {
                    GameObject.Destroy( cups[i] );
		 	    }
		    }
	    }
		cupAnimTimer = 0.0f;
        opponentCupAnimTimer = 0.0f;
		
		ballCursor.GetComponent<Renderer>().enabled = false;
		gameStarted = true;

		heatingUpText.GetComponent<Animator>().enabled = false;
		fireText.GetComponent<Animator>().enabled = false;

		// Fading instructions variables
		fadeTimer = 3.0f; // set duration time in seconds in the Inspector
		colorStart = instructionText.GetComponent<Renderer>().material.color;
		colorEnd = new Color( colorStart.r, colorStart.g, colorStart.b, 0.0f );
		fadeValue = 0.0f;
		
		RandomizeSliderStartPosition();
	}

    void ResetPlayer()
    {
        playersTurn = true;
        isShootingHorizontal = true;
        sliderHorizontal.transform.position = sliderHorizontalOriginalPosition;
        isBouncing = false;
        ballCursor.GetComponent<Renderer>().enabled = false;

        if (ballParent)
        {
            ballParent.transform.position = originalBallPosition;
            ballParent.transform.localScale = originalBallScale;
        }
    }

    void ResetOpponent()
    {
        if (playersTurn)
        {
            playersTurn = false;
            isBouncing = false;
            opponentIsBouncing = false;
            opponentShotTimer = 1.0f;

            CheckGameOver();

            if (opponentBallParent)
            {
                // Randomize starting ball position
                var randomX = Random.Range(
                    opponentOriginalBallPosition.x - 2.1f, opponentOriginalBallPosition.x + 2.1f);
                var newPosition = opponentOriginalBallPosition;
                newPosition.x = randomX;
                opponentBallParent.transform.position = newPosition;
                opponentBallParent.transform.localScale = originalBallScale;
                opponentDestinationY = Random.Range(
                    opponentOriginalBallPosition.y - 7.2f, opponentOriginalBallPosition.y - 5.5f);
                // Todo: Change logic to focus on one cup at a time, with margin for error
                // Higher level opponents can have a smaller margin for error
                // Really high levels can aim for islands
            }
        }
    }

    void CheckGameOver()
    {
        for(int i=0; i<10; i++)
        {
            if (cupsPlaced[i])
            {
                return;
            }
        }

        uiController.EndBeerPongMiniGame();
    }


    void RandomizeSliderStartPosition()
	{
		Vector3 tempSliderPosition = sliderHorizontal.transform.position;
		Vector3 tempBallPosition = ballParent.transform.position;
		
		float randomX = Mathf.Round( Random.value * slideBarHorizontalActualLength ) + 
			( slideBarHorizontal.transform.position.x - slideBarHorizontalActualLength/2 );
		tempSliderPosition.x = randomX;
		tempBallPosition.x = randomX;
		
		sliderHorizontal.transform.position = tempSliderPosition;
		ballParent.transform.position = tempBallPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "ExitButton":
                        if (ioController)
                        {
                            ioController.RevertToNormal();
                        }
                        GameObject.Destroy(transform.parent.gameObject);
                        break;
                }
            }
        }

        if ( gameStarted )
		{
			if( isShootingHorizontal )
			{
				if( "right" == sliderDirection )
				{
					ballParent.transform.Translate( rightSlide * 60.0f * Time.deltaTime );
					sliderHorizontal.transform.Translate( rightSlide * 60.0f * Time.deltaTime );
					
					if( sliderHorizontal.transform.position.x >= (slideBarHorizontal.transform.position.x + slideBarHorizontalActualLength/2) )
					{
						sliderDirection = "left";
					}
				}
				else // if "left" == sliderDirection
				{
					ballParent.transform.Translate( leftSlide * 60.0f * Time.deltaTime );
					sliderHorizontal.transform.Translate( leftSlide * 60.0f * Time.deltaTime );
					
					if( sliderHorizontal.transform.position.x <= (slideBarHorizontal.transform.position.x - slideBarHorizontalActualLength/2) )
					{
						sliderDirection = "right";
					}
				}
				
				if( Input.GetMouseButtonDown( 0 ) )
				{
					isShootingHorizontal = false;
					isShootingVertical = true;
					sliderDirection = "up";
				}
			}
			else if( isShootingVertical )
			{
				if( "up" == sliderDirection )
				{
					sliderVertical.transform.Translate( rightSlide * 60.0f * Time.deltaTime );
					
					if( sliderVertical.transform.position.y >= (slideBarVertical.transform.position.y + slideBarVerticalActualLength/2) )
					{
						sliderDirection = "down";
					}
				}
				else // if "down" == sliderDirection
				{
					sliderVertical.transform.Translate( leftSlide * 60.0f * Time.deltaTime );
					
					if( sliderVertical.transform.position.y <= (slideBarVertical.transform.position.y - slideBarVerticalActualLength/2) )
					{
						sliderDirection = "up";
					}
				}

				if( canShootVertical )
				{
					if( Input.GetMouseButtonDown( 0 ) )
					{
						thrownSFX.GetComponent<AudioSource>().Play();
						isShootingVertical = false;
						
						Vector3 tempPosition = new Vector3( sliderHorizontal.transform.position.x,
						                                   sliderVertical.transform.position.y, -3.0f );
						ballCursor.transform.position = tempPosition;
						ballCursor.GetComponent<Renderer>().enabled = true;
					}
				}
				else
				{
					shootVerticalTimer -= Time.deltaTime;

					if( shootVerticalTimer <= 0.0f )
						canShootVertical = true;
				}
			}
			
			if( fadeValue < 1.0f )
			{
				fadeTimer -= Time.deltaTime;
				fadeValue += Time.deltaTime;
				instructionText.GetComponent<Renderer>().material.color = Color.Lerp( colorStart, colorEnd, fadeValue/1.0f );
				
				if( fadeValue >= 1.0f )
				{
					GameObject.Destroy( instructionText );
				}
			}
			
			// Handles the "explosion" animation of the description text
			if( descriptionTextGrowing )
			{
				textGrowthTimer -= Time.deltaTime;
				
				if( textGrowthTimer <= 0.0f )
				{
					descriptionText.GetComponent<TextMesh>().fontSize = descriptionText.GetComponent<TextMesh>().fontSize + 3;
					growthTimerRate -= .004f * 60.0f * Time.deltaTime;
					textGrowthTimer = growthTimerRate;
				}
				if( descriptionText.GetComponent<TextMesh>().fontSize > 110 )
				{
					descriptionTextGrowing = false;
				}
			}
			
			BallMovement();
            OpponentBallMovement();
			TickTimers();
		}

        // For testing purposes only
        if (Input.GetKeyDown(KeyCode.A))
        {
            for(int i=0; i<10; i++)
            {
                if (cupsPlaced[i])
                {
                    cupsPlaced[i] = false;
                    GameObject.Destroy(cups[i]);
                    break;
                }
            }
        }
	}
	
	void BallMovement()
	{
		if( playersTurn && !isShootingVertical && !isShootingHorizontal ) // Then ball should be in air
		{
			if( !isBouncing )
			{
				ballParent.transform.Translate( ballMovement * 60.0f * Time.deltaTime );
				
				// If ball is still on the "up" trajectory
				if( ballParent.transform.position.y < 
				   (sliderVertical.transform.position.y - (sliderVertical.transform.position.y - sliderHorizontal.transform.position.y)/3 ) )
				{
					ballParent.transform.localScale = ballParent.transform.localScale * ballGrowRate;
				}
				else 	// If ball is falling down
				{
					ballParent.transform.localScale = ballParent.transform.localScale * ballShrinkRate;
				}
				
				if( ballParent.transform.position.y > (sliderVertical.transform.position.y - .07f) )
				{
					int finishType = CheckBallInCup();
					switch( finishType )
					{
					case 0:
						isBouncing = true;
						rimJobSFX.GetComponent<AudioSource>().Play();
                        var randomInsult = Random.Range(0, 4);
                        switch (randomInsult)
                        {
                            case 0:
                                descriptionText.GetComponent<TextMesh>().text = "Total Miss!";
                                break;
                            case 1:
                                descriptionText.GetComponent<TextMesh>().text = "Yikes!";
                                break;
                            case 2:
                                descriptionText.GetComponent<TextMesh>().text = "Embarassing!";
                                break;
                            case 3:
                                descriptionText.GetComponent<TextMesh>().text = "Ouch!";
                                break;
                        }
						break;
					case 1:
						descriptionText.GetComponent<TextMesh>().text = "Island!";
						break;
					case 2:
						descriptionText.GetComponent<TextMesh>().text = "Freshman Cup!";
						break;
					case 3:
						descriptionText.GetComponent<TextMesh>().text = "Water Cup!";
						break;
					case 4:
						descriptionText.GetComponent<TextMesh>().text = "Nice Shot!";
						break;
                    case 5:
						isBouncing = true;
						rimJobSFX.GetComponent<AudioSource>().Play();
						descriptionText.GetComponent<TextMesh>().text = "Close!";
						break;
					}
						
					descriptionText.GetComponent<Renderer>().enabled = true;
					descriptionText.GetComponent<TextMesh>().fontSize = 1;
					descriptionTextGrowing = true;

                    ResetOpponent();
				}
			}
			else 	// If in the process of bouncing away from the table. HACK - hardcoded values.
			{
				ballParent.transform.Translate( ballMovement * 30.0f * Time.deltaTime );
				
				// If ball is still on the "up" trajectory
				if( ballParent.transform.position.y < originalBallPosition.y + 7.3 )
				{
					ballParent.transform.localScale = ballParent.transform.localScale * ballGrowRate;
				}
				else 	// If ball is falling down
				{
					ballParent.transform.localScale = ballParent.transform.localScale * ballShrinkRate;
				}
				
				if( ballParent.transform.position.y > originalBallPosition.y + 8.3f )
				{
                    ResetOpponent();
				}
			}
		}
	}

    void OpponentBallMovement()
    {
        if (!playersTurn)
        {
            if (opponentShotTimer > 0.0f)
            {
                opponentShotTimer -= Time.deltaTime;
            }
            else
            {
                if (!opponentIsBouncing)
                {
                    opponentBallParent.transform.Translate(ballMovement * -60.0f * Time.deltaTime);

                    // If ball is still on the "up" trajectory
                    var distanceToDestination = opponentOriginalBallPosition.y - opponentDestinationY;
                    var halfWayPoint = opponentDestinationY + (distanceToDestination / 2);
                    if (opponentBallParent.transform.position.y > halfWayPoint)
                    {
                        opponentBallParent.transform.localScale = opponentBallParent.transform.localScale * ballGrowRate;
                    }
                    else    // If ball is falling down
                    {
                        opponentBallParent.transform.localScale = opponentBallParent.transform.localScale * ballShrinkRate;
                    }

                    if (opponentBallParent.transform.position.y < opponentDestinationY)
                    {
                        int finishType = CheckBallInYourCup();

                        switch (finishType)
                        {
                            case 0:
                            case 5:
                                opponentIsBouncing = true;
                                rimJobSFX.GetComponent<AudioSource>().Play();
                                break;
                            default:
                                break;
                        }

                        ResetPlayer();
                    }
                }
                else    // If in the process of bouncing away from the table. HACK - hardcoded values.
                {
                    opponentBallParent.transform.Translate(ballMovement * -30.0f * Time.deltaTime);

                    // If ball is still on the "up" trajectory
                    if (opponentBallParent.transform.position.y > opponentOriginalBallPosition.y - 7.9f)
                    {
                        opponentBallParent.transform.localScale = ballParent.transform.localScale * ballGrowRate;
                    }
                    else    // If ball is falling down
                    {
                        opponentBallParent.transform.localScale = ballParent.transform.localScale * ballShrinkRate;
                    }

                    if (opponentBallParent.transform.position.y < opponentOriginalBallPosition.y - 9.5f)
                    {
                        ResetPlayer();
                    }
                }
            }
        }
    }

    // Returns 1 if island, 2 if freshman cup, 3 if water cup, 4 if other
    int CheckBallInCup()
	{
        List<int> rimJobs = new List<int>();
		for( int i=0; i<10; i++ )
		{
			if( cupsPlaced[i] )
			{
                var ball = ballParent.transform.Find("PingPongBall");
                var ballCenterX = ball.transform.position.x;
                var ballCenterY = ball.transform.position.y;
                if ( ballCenterX >= (cups[i].transform.position.x + cupLeftMost)
					&& ballCenterX <= (cups[i].transform.position.x + cupRightMost)
					&& ballCenterY >= (cups[i].transform.position.y + cupBottomMost)
					&& ballCenterY <= (cups[i].transform.position.y + cupTopMost) )
				{	
					// Play ball in cup sound
					inCupSFX.GetComponent<AudioSource>().Play();
					// Remove cup in global controller
					cupsPlaced[i] = false;
                    // Play "moving" animation
                    cups[i].GetComponent<Animator>().Play("MadeBall");
					// Start timer until animation is over
                    cupAnimTimer = defaultCupAnimTime;
                    // Keep current cup for future use
                    cupIndex = i;
                    // Add party points
                    if (uiController)
                    {
                        uiController.AddPartyPoints(10, cups[i].transform.position);
                    }

                    // Kill off the ball
                    ResetOpponent();

					if( isIsland( cupIndex ) )
						return 1;
					if( isFreshman( cupIndex ) )
						return 2;
					// if cupIndex == 10, don't think this is implemented yet
					//   return 3
					return 4;
				}
				else if( ballCenterX >= (cups[i].transform.position.x - (cups[i].GetComponent<Renderer>().bounds.size.x*6/10))
					    && ballCenterX <= (cups[i].transform.position.x + (cups[i].GetComponent<Renderer>().bounds.size.x*6/10))
					    && ballCenterY >= (cups[i].transform.position.y)
					    && ballCenterY <= (cups[i].transform.position.y + (cups[i].GetComponent<Renderer>().bounds.size.y*6/10)) )
				{
                    rimJobs.Add(i);
				}
			}
		}

		// If you missed but got close to a cup, show it rattle
		if( rimJobs.Count > 0 )
		{
            // Play "moving" animation
            foreach (int rimJob in rimJobs)
            {
                cups[rimJob].GetComponent<Animator>().Play("CupNearMiss");
            }
            return 5;
		}
		
		return 0;
	}

    void CheckPlayerReRack()
    {
        int currentCupCount = 0;
        for(int i=0; i<10; i++)
        {
            if (cupsPlaced[i])
            {
                currentCupCount++;
            }
        }

        switch(currentCupCount)
        {
            case 3:
                List<GameObject> leftoverCups = new List<GameObject>();
                for (int i = 0; i < 10; i++)
                {
                    if (cupsPlaced[i])
                    {
                        leftoverCups.Add(cups[i]);
                        cups[i] = null;
                    }
                    cupsPlaced[i] = false;
                }
                cupsPlaced[1] = true;
                cupsPlaced[2] = true;
                cupsPlaced[5] = true;
                if (leftoverCups.Count != 3)
                {
                    Debug.Log("Error while re-racking");
                    return;
                }
                cups[1] = leftoverCups[0];
                cups[2] = leftoverCups[1];
                cups[5] = leftoverCups[2];
                cups[1].transform.position = new Vector3(
                    transform.position.x - 0.57f, transform.position.y + 2.5f, transform.position.z + 0.3f);
                cups[2].transform.position = new Vector3(
                    transform.position.x + 0.61f, transform.position.y + 2.5f, transform.position.z + 0.3f);
                cups[5].transform.position = new Vector3(
                    transform.position.x + 0.0f, transform.position.y + 2.0f, transform.position.z + 0.2f);
                break;
            default:
                break;
        }
    }

    int CheckBallInYourCup()
    {
        List<int> rimJobs = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            if (yourCupsPlaced[i])
            {
                var ball = opponentBallParent.transform.Find("OpponentBall");
                var ballCenterX = ball.transform.position.x;
                var ballCenterY = ball.transform.position.y;
                if (ballCenterX >= (playerCups[i].transform.position.x + cupLeftMost)
                    && ballCenterX <= (playerCups[i].transform.position.x + cupRightMost)
                    && ballCenterY >= (playerCups[i].transform.position.y + cupBottomMost)
                    && ballCenterY <= (playerCups[i].transform.position.y + cupTopMost))
                {
                    // Play ball in cup sound
                    inCupSFX.GetComponent<AudioSource>().Play();
                    // Remove cup in global controller
                    yourCupsPlaced[i] = false;
                    // Play "moving" animation
                    playerCups[i].GetComponent<Animator>().Play("MadeBall");
                    // Start timer until animation is over
                    opponentCupAnimTimer = defaultCupAnimTime;
                    // Keep current cup for future use
                    opponentCupIndex = i;

                    // Need these checks for opponent?
                    // if (isIsland(cupIndex))
                    //     return 1;
                    // if (isFreshman(cupIndex))
                    //     return 2;
                    // if cupIndex == 10, don't think this is implemented yet
                    //   return 3
                    return 4;
                }
                else if (ballCenterX >= (playerCups[i].transform.position.x - (playerCups[i].GetComponent<Renderer>().bounds.size.x * 6 / 10))
                        && ballCenterX <= (playerCups[i].transform.position.x + (playerCups[i].GetComponent<Renderer>().bounds.size.x * 6 / 10))
                        && ballCenterY >= (playerCups[i].transform.position.y)
                        && ballCenterY <= (playerCups[i].transform.position.y + (playerCups[i].GetComponent<Renderer>().bounds.size.y * 6 / 10)))
                {
                    rimJobs.Add(i);
                }
            }
        }

        // If you missed but got close to a cup, show it rattle
        if (rimJobs.Count > 0)
        {
            // Play "moving" animation
            foreach (int rimJob in rimJobs)
            {
                playerCups[rimJob].GetComponent<Animator>().Play("CupNearMiss");
            }
            return 5;
        }

        return 0;
    }

    void TickTimers()
	{
		if(cupAnimTimer > 0.0f)
		{
			cupAnimTimer -= Time.deltaTime;
			
			if( cupAnimTimer <= 0.0f )
			{
                // Try to fade it out eventually
                GameObject.Destroy( cups[cupIndex] );
                CheckPlayerReRack();
            }
		}
        if (opponentCupAnimTimer > 0.0f)
        {
            opponentCupAnimTimer -= Time.deltaTime;

            if (opponentCupAnimTimer <= 0.0f)
            {
                GameObject.Destroy(playerCups[opponentCupIndex]);
            }
        }
	}

	bool isIsland( int index )
	{
		switch( index )
		{
		case 0:
			if( !cupsPlaced[1] && !cupsPlaced[4] )
				return true;
			break;
		case 1:
			if( !cupsPlaced[0] && !cupsPlaced[2] && !cupsPlaced[4] && !cupsPlaced[5] )
				return true;
			break;
		case 2:
			if( !cupsPlaced[1] &&  !cupsPlaced[3] && !cupsPlaced[5] && !cupsPlaced[6] )
				return true;
			break;
		case 3:
			if( !cupsPlaced[2] && !cupsPlaced[6] )
				return true;
			break;
		case 4:
			if( !cupsPlaced[0] &&  !cupsPlaced[1] && !cupsPlaced[5] && !cupsPlaced[7] )
				return true;
			break;
		case 5:
			if( !cupsPlaced[1] && !cupsPlaced[2] && !cupsPlaced[4] &&
			   !cupsPlaced[6] && !cupsPlaced[7] && !cupsPlaced[8] )
				return true;
			break;
		case 6:
			if( !cupsPlaced[2] && !cupsPlaced[3] && !cupsPlaced[5] &&  !cupsPlaced[8] )
				return true;
			break;
		case 7:
			if(!cupsPlaced[4] && !cupsPlaced[5] && !cupsPlaced[8] && !cupsPlaced[9] )
				return true;
			break;
		case 8:
			if(!cupsPlaced[5] && !cupsPlaced[6] && !cupsPlaced[7] && !cupsPlaced[9] )
				return true;
			break;
		case 9:
			if( !cupsPlaced[7] &&
			   !cupsPlaced[8] )
				return true;
			break;
		}

		return false;
	}

	bool isFreshman( int index )
	{
		if( index == 5 )
		{
			if(cupsPlaced[0] && cupsPlaced[1] && cupsPlaced[2] && cupsPlaced[3] &&
			   cupsPlaced[4] && cupsPlaced[6] && cupsPlaced[7] && cupsPlaced[8] && cupsPlaced[9] )
				return true;
		}

		return false;
	}
}
