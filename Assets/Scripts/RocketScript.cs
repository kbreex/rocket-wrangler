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

    // Create the rocket sprites for on and off
    public Sprite rocketOn;
    public Sprite rocketOff;

    private Touch touch;

    private bool isEjectMode;

    private UnityEngine.Vector3 rocketPosition;


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
                Debug.Log("DEBUG: Touch Y: " + touch.position.y);
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
        if (Input.touchCount > 0 && isEjectMode){
            // Sets the rocket into eject mode
            Debug.Log("DEBUG: INITIATIED WITH Y = " + touch.deltaPosition.y);

            // Shut off rocket
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
            fuelBarScript.isRocketOn = false;
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            if (ejectCollider == Physics2D.OverlapPoint(touchPos))
            {
                Debug.Log("DEBUG: WE DID IT!");
                isEjectMode = false;

            }
        }
        if (Input.touchCount == 0){
            // If there is no touch on the screen then we turn eject mode off
            isEjectMode = false;

        }
    }

}
