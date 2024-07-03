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

    // Create an object for teh Obstacle spawner
    public GameObject obstacleSpawner;

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

    // Create a fuel is empty bool for the fuelbar script to access
    public bool fuelIsEmpty;

    // Bool for player being on the ship
    private bool playerOnRocket;

    private float touchSensitivity;

    private float rocketSpeed;

    private float rocketAcceleration;

    // Declare the slow motion time variables
    public float slowMotionTimeScale;
    private float startTimeScale;
    private float startFixedDeltaTime;
    
    void Start(){
        // Set a sensitivity modifier to change how sensitive the touch is
        touchSensitivity = 0.004f;

        // Set the speed to 0 at the start
        rocketSpeed = 0f;

        // This is hopw much we accerate by while holding down the screen
        rocketAcceleration = 0.1f;

        // Set the slow motion effect time scale
        slowMotionTimeScale = 0.2f;

        // Fuel starts full
        fuelIsEmpty = false;

        // Start ejectmode off and player on the rocket
        isEjectMode = false;
        playerOnRocket = true;

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
        // Debug spot

        // Get the rockets distance given the position of it relative to the origin
        // Add two as the rocket starts at -2
        float rocketDistance = transform.position.y + 2;

        // Set the distance in the UI
        userInterfaceInfoScript.SetDistanceText(rocketDistance);

        // Check if more than 0 fingers touched the screen and we are not in eject mode
        if (Input.touchCount > 0 && !isEjectMode){
            // Assign the touch variable to the first finger that has touched the screen
            touch = Input.GetTouch(0);

            // Increase the rocket speed if either of these touch phases are initiated
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary){
                // Turn the rocket on
                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
                fuelBarScript.isRocketOn = true;

                // Incrase speed
                rocketSpeed += rocketAcceleration * Time.deltaTime;

                // Set the updated speed in the UI
                userInterfaceInfoScript.SetSpeedText(rocketSpeed);
            }
            
            // This checks if the user is touching or moving on the screen, and if the rocket is full and the player is currently on that rocket
            if ((touch.phase == TouchPhase.Moved && !fuelIsEmpty && playerOnRocket) || (touch.phase == TouchPhase.Stationary && !fuelIsEmpty && playerOnRocket)){        
                // Increase the position of the rocket when it is touched
                transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * touchSensitivity, transform.position.y + rocketSpeed * Time.deltaTime);

                // Change the position of the player as well
                playerScript.playerBody.transform.position = new Vector2(playerScript.playerBody.transform.position.x + touch.deltaPosition.x * touchSensitivity, playerScript.playerBody.transform.position.y + rocketSpeed * Time.deltaTime);

                // Change the position of the camaera as well. Keep the old Z value as otherwise we cannot see the player
                // We add two because we want the rocket to be -2 of the origin and the camera when we are moving so the finger can rest in the correct position
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + rocketSpeed * Time.deltaTime, mainCamera.transform.position.z);

                // Change the position of the obstacle spawner
                obstacleSpawner.transform.position = new Vector2(obstacleSpawner.transform.position.x, obstacleSpawner.transform.position.y + rocketSpeed * Time.deltaTime);

                // Checks that we moved the finger enough down and that it is below the correct part of the screen.
                // TODO Make sure it is based off the screen height
                if(touch.deltaPosition.y < -20 && touch.position.y < 600 && playerOnRocket){
                    // Put rocket into eject mode
                    isEjectMode = true;

                }
            }
        }
        // Check if we just release our finger
        else if (Input.touchCount == 0 && !isEjectMode){
                // Set rocket sprite to be off when we are not touching it, but keep moving everything at the current speed
                transform.position = new Vector2(transform.position.x, transform.position.y + rocketSpeed * Time.deltaTime);
                playerScript.playerBody.transform.position = new Vector2(playerScript.playerBody.transform.position.x, playerScript.playerBody.transform.position.y + rocketSpeed * Time.deltaTime);
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + rocketSpeed * Time.deltaTime, mainCamera.transform.position.z);
                obstacleSpawner.transform.position = new Vector2(obstacleSpawner.transform.position.x, obstacleSpawner.transform.position.y + rocketSpeed * Time.deltaTime);

                // Turn off the rocket
                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
                fuelBarScript.isRocketOn = false;
        }
        // Check if we have a finger on the screen and we are in eject mode
        if (Input.touchCount > 0 && isEjectMode ){
            // Set touch to the first finger that touches the screen
            touch = Input.GetTouch(0);

            // Put the game into slow motion
            // https://www.youtube.com/watch?v=Jlg0riu0XEU&t=15s
            ToggleSlowMotion(true);

            // Shut off rocket
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
            fuelBarScript.isRocketOn = false;

            // Keep moving the rocket and others at the current speed
            transform.position = new Vector2(transform.position.x, transform.position.y + rocketSpeed * Time.deltaTime);
            playerScript.playerBody.transform.position = new Vector2(playerScript.playerBody.transform.position.x, playerScript.playerBody.transform.position.y + rocketSpeed * Time.deltaTime);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + rocketSpeed * Time.deltaTime, mainCamera.transform.position.z);
            obstacleSpawner.transform.position = new Vector2(obstacleSpawner.transform.position.x, obstacleSpawner.transform.position.y + rocketSpeed * Time.deltaTime);

            // Detect the touch of the 2d collider in the rocket to turn off eject mode
            // https://discussions.unity.com/t/how-to-detect-a-touch-on-box-collider-2d-in-unity-4-3/87027
            // Basically converts the camera position to touch position
            Vector3 touchInWorldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchWorldPos = new Vector2(touchInWorldPosition.x, touchInWorldPosition.y);

            // Draw a line between transfer.position and touchWorldPos
            ejectLine.SetPosition(0, transform.position);
            ejectLine.SetPosition(1, touchWorldPos);

            // Point the rocket towards the touch world position
            // Create the vector from touch position to rocket position
            Vector2 vector = touchInWorldPosition - transform.position;

            // Calculate the angle and convert to degrees
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            // Rotate the rocket
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

            // GONNA HAVE TO FIX THIS
            // Set the PlayerScript value of ejectAboveRocketY if the finger is above the default value of the ship
            if (touchInWorldPosition.y > transform.position.y){
                playerScript.ejectAboveRocketY = true;
            }
            else{
                playerScript.ejectAboveRocketY = false;
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
            
            // Turn off slow motion
            ToggleSlowMotion(false);

            // Set the eject line to (0, 0, 0) so it is invisible
            ejectLine.SetPosition(0, Vector3.zero);
            ejectLine.SetPosition(1, Vector3.zero);

            // Find the slope and distance
            float y2Minusy1 = ejectLine.GetPosition(0).y - ejectLine.GetPosition(1).y;
            float x2Minusx1 = ejectLine.GetPosition(0).x - ejectLine.GetPosition(1).x;

            // The distance will be the speed at which the player travels
            // Slope is used to find the result location
            float pullSlope = y2Minusy1 / x2Minusx1;
            double pullDistance = Math.Sqrt((y2Minusy1 * y2Minusy1) + (x2Minusx1 * x2Minusx1));

            // Send the players speed to be the strength of the pull
            playerScript.ejectSpeed = pullDistance * 2;
            playerScript.ejectSlope = pullSlope;

            // If there is no touch on the screen and we are in eject mode then we eject the player
            isEjectMode = false;
            playerOnRocket = false;

            // Set the player to be visible
            playerScript.isPlayerVisible = true;

            // Explode the rocket??
            
        }
    }

    // Function to toggle the slow motion of eject mode
    private void ToggleSlowMotion(bool isStart){

        if (isStart){
            Time.timeScale = slowMotionTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
        }
        else{
            Time.timeScale = startTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime;
        }

    }

}
