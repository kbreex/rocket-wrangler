using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Access the rocketScript
    private RocketScript rocketScript;

    public Rigidbody2D playerBody;

    // Create a public double for rocketscript to access so we can change the speed of the player ejected
    public double ejectSpeed;

    public float ejectSlope;

    public bool ejectAboveDefualtRocketY;

    // Set a vector that acts as the direction we eject in
    private Vector2 direction;


    void Start()
    {
        // Set the script to a var, we set the rocket gameobject to have the tag Rocket
        rocketScript = GameObject.FindGameObjectWithTag("Rocket").GetComponent<RocketScript>();
        ejectSpeed = 0;

        // Set to false just in case
        ejectAboveDefualtRocketY = false;
    }

    void Update()
    {


        // Get the direction the player ejects in from the normalized slope
        // Check the direction of the slope and if aboveDefualtRocketY is true to determine which way we normalize it 
        if (ejectSlope > 0 && !ejectAboveDefualtRocketY){
            direction = new Vector2(1, ejectSlope).normalized;
            // Move along the direction vector
            transform.position += (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }
        else if (ejectSlope <= 0 && !ejectAboveDefualtRocketY){
            direction = new Vector2(-1, -ejectSlope).normalized;
            transform.position += (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }
         else if (ejectSlope > 0 && ejectAboveDefualtRocketY){
            direction = new Vector2(1, ejectSlope).normalized;
            transform.position -= (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }
        else if (ejectSlope <= 0 && ejectAboveDefualtRocketY){
            direction = new Vector2(1, ejectSlope).normalized;
            transform.position -= (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }


    
        
    }
}
