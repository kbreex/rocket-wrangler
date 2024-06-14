using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.iOS;
using System;

public class RocketScript : MonoBehaviour
{ 

    // Access the fuelbarscript to give it access to wether the rocket is on or not
    private FuelBarScript fuelBarScript;

    // Access the player script 
    private PlayerScript playerScript;

    // Create a fuel is empty bool for the fuelbar script to access
    public bool fuelIsEmpty;

    // Create the rigidbody for the rocket so it can have collisions 
    public Rigidbody2D rocketBody;

    // Create the circle collier so the ricket can detect eject mode
    public CircleCollider2D ejectCollider;

    // Create the eject line
    public LineRenderer ejectLine;

    // Create the rocket sprites for on and off
    public Sprite rocketOn;
    public Sprite rocketOff;

    private Touch touch;

    // Bool for seeing if we are in ejectMode
    private bool isEjectMode;

    // Bool for player being on the ship
    private bool playerOnRocket;

    private float speedModifier;

    // Default Y position
    private readonly float defaultYPos = -2f;
    void Start(){
        // Set a speed modifier to change how sensitive the touch is
        speedModifier = 0.004f;

        // Fuel starts full
        fuelIsEmpty = false;

        // Start ejectmode off and player on the rocket
        isEjectMode = false;
        playerOnRocket = true;

        // Set the script to a var, we set the fuelbar gameobject to have the tag fuelbar
        fuelBarScript = GameObject.FindGameObjectWithTag("FuelBar").GetComponent<FuelBarScript>();

        // Set the script for the player to a var so we can eject the player when eject mode is set
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

        // Set the eject line to (0, 0, 0) so it is invisible
        ejectLine.SetPosition(0, Vector3.zero);
        ejectLine.SetPosition(1, Vector3.zero);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !isEjectMode){
            // Assign the touch variable to the first finger that has touched the screen
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved && !fuelIsEmpty && playerOnRocket){
                // Increase the position of the rocket as well as turn on the engines when it is touched
                transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, defaultYPos);
                // Change the position of the player as well
                playerScript.playerBody.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, defaultYPos);

                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
                fuelBarScript.isRocketOn = true;
                // Checks that we moved the finger enough down and that it is below the correct part of the screen.
                // TODO Make sure it is based off the screen height
                if(touch.deltaPosition.y < -20 && touch.position.y < 600 && playerOnRocket){
                    // Put rocket into eject mode
                    isEjectMode = true;

                }
            }
            else if (touch.phase == TouchPhase.Stationary && !fuelIsEmpty && playerOnRocket){
                // Turn on the engine if they are just holding the touch there
                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
                fuelBarScript.isRocketOn = true;

            }
            else{
                // Set rocket sprite to be off when we are not touching it
                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
                fuelBarScript.isRocketOn = false;
            }

        }
        // Check if we have a finger on the screen and we are in eject mode
        if (Input.touchCount > 0 && isEjectMode ){
            touch = Input.GetTouch(0);

            // Detect the touch of the 2d collider in the rocket to turn off eject mode
            // https://discussions.unity.com/t/how-to-detect-a-touch-on-box-collider-2d-in-unity-4-3/87027
            // Basically converts the camera position to touch position
            Vector3 touchInWorldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchWorldPos = new Vector2(touchInWorldPosition.x, touchInWorldPosition.y);

            // Shut off rocket
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
            fuelBarScript.isRocketOn = false;

            // Draw a line between transfer.position and touchWorldPos
            ejectLine.SetPosition(0, transform.position);
            ejectLine.SetPosition(1, touchWorldPos);

            // Point the rocket towards the touch world position
            // Create the vector from touch positio to rocket position
            Vector2 vector = touchInWorldPosition - transform.position;

            // Calculate the angle in radians
            float angleRadians = Mathf.Atan2(vector.y, vector.x);

            // Convert the angle to degrees
            float angleDegrees = angleRadians * Mathf.Rad2Deg;

            Debug.Log("DEBUG: Y: " + touch.position.y);


            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDegrees + 90));


            // Set the PlayerScript value of ejectAboveDefualtRocketY if the finger is above the default value of the ship
            if (touchWorldPos.y > defaultYPos){
                playerScript.ejectAboveDefualtRocketY = true;
            }
            else{
                playerScript.ejectAboveDefualtRocketY = false;
            }

            // Check if they overlap with the finger
            if (ejectCollider == Physics2D.OverlapPoint(touchWorldPos))
            {
                // Turn off eject mode
                isEjectMode = false;
                // Set the eject line to (0, 0, 0) so it is invisible
                ejectLine.SetPosition(0, Vector3.zero);
                ejectLine.SetPosition(1, Vector3.zero);
            }


        }
        // Check if the finger is let go and we are in eject mode
        // This statement ejects the player
        if (Input.touchCount == 0 && isEjectMode){

            // Find the slope and distance
            float y2Minusy1 = ejectLine.GetPosition(0).y - ejectLine.GetPosition(1).y;
            float x2Minusx1 = ejectLine.GetPosition(0).x - ejectLine.GetPosition(1).x;

            // The distance will be the speed at which the player travels
            // Slope is used to find the result location
            float pullSlope = y2Minusy1 / x2Minusx1;
            double pullDistance = Math.Sqrt((y2Minusy1 * y2Minusy1) + (x2Minusx1 * x2Minusx1));


            // Set the eject line to (0, 0, 0) so it is invisible
            ejectLine.SetPosition(0, Vector3.zero);
            ejectLine.SetPosition(1, Vector3.zero);

            // Send the players speed to be the strength of the pull
            playerScript.ejectSpeed = pullDistance;
            playerScript.ejectSlope = pullSlope;

            // If there is no touch on the screen and we are in eject mode then we eject the player
            isEjectMode = false;
            playerOnRocket = false;

        }
    }

}
