using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.iOS;

public class RocketScript : MonoBehaviour
{ 

    // Access the fuelbarscript to give it access to wether the rocket is on or not
    private FuelBarScript fuelBarScript;

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

    // Change this back to private
    private bool isEjectMode;

    private float speedModifier;

    // Default Y position
    private readonly float defaultYPos = -3f;
    void Start(){
        // Set a speed modifier to change how sensitive the touch is
        speedModifier = 0.004f;

        // Fuel starts full
        fuelIsEmpty = false;

        // Start ejectmode off
        isEjectMode = false;

        // Set the script to a var, we set the fuelbar gameobject to have the tag fuelbar
        fuelBarScript = GameObject.FindGameObjectWithTag("FuelBar").GetComponent<FuelBarScript>();

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

            if (touch.phase == TouchPhase.Moved && !fuelIsEmpty){
                // Increase the position of the rocket as well as turn on the engines when it is touched
                transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, defaultYPos);
                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
                fuelBarScript.isRocketOn = true;
                // Checks that we moved the finger enough down and that it is below the correct part of the screen.
                // TODO Make sure it is based off the screen height
                if(touch.deltaPosition.y < -20 && touch.position.y < 700 && touch.position.y > 250){
                    // Put rocket into eject mode
                    isEjectMode = true;

                }
            }
            else if (touch.phase == TouchPhase.Stationary && !fuelIsEmpty){
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
        if (Input.touchCount > 0 && isEjectMode){
            touch = Input.GetTouch(0);

            // Detect the touch of the 2d collider in the rocket to turn off eject mode
            // https://discussions.unity.com/t/how-to-detect-a-touch-on-box-collider-2d-in-unity-4-3/87027
            // Basically converts the camera position to touch position
            Vector3 touchInWorldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchWorldPos = new Vector2(touchInWorldPosition.x, touchInWorldPosition.y);

            // Sets the rocket into eject mode
            Debug.Log("DEBUG: EJECT MODE ON");


            // Shut off rocket
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
            fuelBarScript.isRocketOn = false;

            Debug.Log("DEBUG: Rocket Position:" + transform.position);
            Debug.Log("DEBUG: Finger Location: " + touchWorldPos);

            // Draw a line between transfor.position and touchWorldPos
            ejectLine.SetPosition(0, transform.position);
            ejectLine.SetPosition(1, touchWorldPos);


            

            


            // Check if they overlap with the finger
            if (ejectCollider == Physics2D.OverlapPoint(touchWorldPos))
            {
                Debug.Log("DEBUG: EJECT MODE OFF");
                isEjectMode = false;
                // Set the eject line to (0, 0, 0) so it is invisible
                ejectLine.SetPosition(0, Vector3.zero);
                ejectLine.SetPosition(1, Vector3.zero);
            }


        }
        // Check if the finger is let go and we are in eject mode
        if (Input.touchCount == 0 && isEjectMode){
            
            // If there is no touch on the screen and we are in eject mode then we eject the player
            Debug.Log("DEBUG: EJECTED");
            isEjectMode = false;
            // Set the eject line to (0, 0, 0) so it is invisible
            ejectLine.SetPosition(0, Vector3.zero);
            ejectLine.SetPosition(1, Vector3.zero);
        }
    }

}
