using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceInfoScript : MonoBehaviour
{   
    public Text speedText;

    public Text distanceText;
    

    // Sets the speed text on the UI to the rockets current speed
    public void SetSpeedText(float speed){
        // Set the text to
        string currentSpeed = Math.Round(speed, 2).ToString() + " m/s";
        speedText.text = currentSpeed;
    }

    public void SetDistanceText(float distance){
        string currentDistance = Math.Round(distance, 2).ToString() + " m";
        distanceText.text = currentDistance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
