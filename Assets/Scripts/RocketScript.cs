using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{ 
    // Create the rigidbody for the rocket so it can have collisions 
    public Rigidbody2D rocketBody;
    // Create the rocket sprites for on and off
    public Sprite rocketOn;
    public Sprite rocketOff;

    // Create the horizontal vector
    // https://docs.unity3d.com/ScriptReference/Vector2-ctor.html
    public Vector2 horizontalVelocity;
    


    // Start is called before the first frame update
    void Start()
    {
        // Set our horizontal velocity to zero on start
        horizontalVelocity = new Vector2(0.3f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {

        // This is just a lil tester function for making the shit move, use a and s to move it

        if (Input.GetKey("a")){
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
            rocketBody.velocity -= horizontalVelocity;
            rocketBody.rotation -= (float) -1.5;
        }
        else if (Input.GetKey("d")) {
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOn;
            rocketBody.velocity += horizontalVelocity;
            rocketBody.rotation += (float) -1.5;
        }
        else {
            rocketBody.GetComponent<SpriteRenderer>().sprite = rocketOff;
        }
    }
}
