using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Access the rocketScript
    private RocketScript rocketScript;

    // Create an object for the camera
    public Camera mainCamera;

    public Rigidbody2D playerBody;

    // Create a public double for rocketscript to access so we can change the speed of the player ejected
    public double ejectSpeed;

    public float ejectSlope;

    public bool ejectAboveRocketY;

    public bool isPlayerVisible;

    // Set a vector that acts as the direction we eject in
    private Vector2 direction;


    void Start()
    {
        // Set the script to a var, we set the rocket gameobject to have the tag Rocket
        rocketScript = GameObject.FindGameObjectWithTag("Rocket").GetComponent<RocketScript>();
        ejectSpeed = 0;

        // Set to false just in case
        ejectAboveRocketY = false;

        // Set player visible to false at the start
        isPlayerVisible = false;
    }

    void Update()
    {   

        // Set the player to be invisible or visible by setting its scale
        if (isPlayerVisible){
            transform.localScale = new Vector3(0.3f, 0.3f, 0f);
        }
        else{
            transform.localScale = Vector3.zero;
        }


        // Get the direction the player ejects in from the normalized slope
        // Check the direction of the slope and if aboveDefualtRocketY is true to determine which way we normalize it 
        if (ejectSlope > 0 && !ejectAboveRocketY){
            direction = new Vector2(1, ejectSlope).normalized;
            // Move along the direction vector
            transform.position += (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
            mainCamera.transform.position += (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }
        else if (ejectSlope <= 0 && !ejectAboveRocketY){
            direction = new Vector2(-1, -ejectSlope).normalized;
            transform.position += (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
            mainCamera.transform.position += (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }
         else if (ejectSlope > 0 && ejectAboveRocketY){
            direction = new Vector2(1, ejectSlope).normalized;
            transform.position -= (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
            mainCamera.transform.position -= (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }
        else if (ejectSlope <= 0 && ejectAboveRocketY){
            direction = new Vector2(-1, -ejectSlope).normalized;
            transform.position -= (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
            mainCamera.transform.position -= (float) ejectSpeed * Time.deltaTime * (Vector3)direction;
        }


    
        
    }
}
