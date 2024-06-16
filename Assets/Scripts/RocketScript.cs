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

    // Access the UI info script
    private UserInterfaceInfoScript userInterfaceInfoScript;

    // Create an object for the camera
    public Camera mainCamera;


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

    private float rocketDistance;

    // Declare the slow motion time variables
    public float slowMotionTimeScale;

    private float startTimeScale;
    private float startFixedDeltaTime;
    
    void Start(){
        // Set a speed modifier to change how sensitive the touch is
        speedModifier = 0.004f;

        rocketDistance = -2f;

        // Fuel starts full
        fuelIsEmpty = false;

        // Start ejectmode off and player on the rocket
        isEjectMode = false;
        playerOnRocket = true;

        // Set the slow motion effect time scale
        slowMotionTimeScale = 0.2f;
        // Set the start time scale and delta time scale for slow motion effect
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;

        // Set the script to a var, we set the fuelbar gameobject to have the tag fuelbar
        fuelBarScript = GameObject.FindGameObjectWithTag("FuelBar").GetComponent<FuelBarScript>();

        // Set the script for the player to a var so we can eject the player when eject mode is set
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

        // Set the script for the user interface
        userInterfaceInfoScript = GameObject.FindGameObjectWithTag("UserInterface").GetComponent<UserInterfaceInfoScript>();

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
                

                // Add this distance to the rockets UI, and to the the rockets position, player position and camera position
                rocketDistance += 0.1f;
                userInterfaceInfoScript.AddDistance(rocketDistance);

                // Create a deltaY var that takes the rocket distance and multiplies it by delta time to make it work with slow motion
                float deltaY = rocketDistance * Time.deltaTime;
                Debug.Log("deltaY: " + deltaY);

                // Increase the position of the rocket when it is touched
                transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, transform.position.y + deltaY);
                // Change the position of the player as well
                playerScript.playerBody.transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, playerScript.playerBody.transform.position.y + deltaY);

                // Change the position of the camaera as well. Keep the old Z value as otherwise we cannot see the player
                // We add two because we want the rocket to be -2 of the origin and the camera when we are moving so the finger can rest in the correct position
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + deltaY, mainCamera.transform.position.z);

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
                // Add this distance to the rockets UI
                rocketDistance += 0.1f;
                userInterfaceInfoScript.AddDistance(rocketDistance);

                // Create a deltaY var that takes the rocket distance and multiplies it by delta time to make it work with slow motion
                float deltaY = rocketDistance * Time.deltaTime;
                
                // Move the rocket, camera and player this distance
                transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, transform.position.y + deltaY);
                playerScript.playerBody.transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, playerScript.playerBody.transform.position.y + deltaY);
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + deltaY, mainCamera.transform.position.z);


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

            // Put the game into slow motion
            // https://www.youtube.com/watch?v=Jlg0riu0XEU&t=15s
            ToggleSlowMotion(true);

            rocketDistance += 0.1f;
            userInterfaceInfoScript.AddDistance(rocketDistance);
            float deltaY = rocketDistance * Time.deltaTime;

            // Keep moving the rocket at the current speed
            transform.position = new Vector2(transform.position.x, transform.position.y + deltaY);
            playerScript.playerBody.transform.position = new Vector2(playerScript.playerBody.transform.position.x, playerScript.playerBody.transform.position.y + deltaY);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + deltaY, mainCamera.transform.position.z);
            

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


            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDegrees + 90));


            // Set the PlayerScript value of ejectAboveDefualtRocketY if the finger is above the default value of the ship
            if (touchWorldPos.y > rocketDistance){
                playerScript.ejectAboveDefualtRocketY = true;
            }
            else{
                playerScript.ejectAboveDefualtRocketY = false;
            }

            // Check if they overlap with the finger, this cancels eject mode
            if (ejectCollider == Physics2D.OverlapPoint(touchWorldPos))
            {
                // Turn off eject mode
                isEjectMode = false;
                // Reset rotation
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                
                // Set the eject line to (0, 0, 0) so it is invisible
                ejectLine.SetPosition(0, Vector3.zero);
                ejectLine.SetPosition(1, Vector3.zero);
                ToggleSlowMotion(false);
            }




        }
        // Check if the finger is let go and we are in eject mode
        // This statement ejects the player
        if (Input.touchCount == 0 && isEjectMode){

            ToggleSlowMotion(false);

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
            playerScript.ejectSpeed = pullDistance * 2;
            playerScript.ejectSlope = pullSlope;

            // If there is no touch on the screen and we are in eject mode then we eject the player
            isEjectMode = false;
            playerOnRocket = false;

            // Set the player to be visible
            playerScript.isPlayerVisible = true;
            
        }
    }

    private void ToggleSlowMotion(bool isStart){

        if (isStart){
            Time.timeScale = slowMotionTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
            Debug.Log("DEBUG: Slow motion engaged");
        }
        else{
            Time.timeScale = startTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime;
            Debug.Log("DEBUG: Slow Disengaged");
        }

    }

}
