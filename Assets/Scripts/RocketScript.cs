using System.Collections;
using System.Collections.Generic;
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

    // Create the rocket sprites for on and off
    public Sprite rocketOn;
    public Sprite rocketOff;

    private Touch touch;


    private float speedModifier;

    // Default Y position
    private float defaultYPos = -3f;
    void Start(){
        // Set a speed modifier to change how sensitive the touch is
        speedModifier = 0.004f;

        // Fuel starts full
        fuelIsEmpty = false;

        // Set the script to a var, we set the fuelbar gameobject to have the tag fuelbar
        fuelBarScript = GameObject.FindGameObjectWithTag("FuelBar").GetComponent<FuelBarScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0){
            // Assign the touch variable to the first finger that has touched the screen
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved && !fuelIsEmpty){
                // Increase the position of the rocket as well as turn on the engines when it is touched
                transform.position = new Vector2(transform.position.x + touch.deltaPosition.x * speedModifier, defaultYPos);
                rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
                fuelBarScript.isRocketOn = true;
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
    }
}
