using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBarScript : MonoBehaviour
{
    // Access rocketscript so we can send when the fuel is empty
    private RocketScript rocketScript;

    public bool isRocketOn;


    public RawImage fuelBar;

    public Texture fuel100;

    public Texture fuel75;

    public Texture fuel50;

    public Texture fuel25;

    public Texture fuel0;

    public float fuelLevel = 100;

    public float fuelDegradeRate = 0.33f;


    // Start is called before the first frame update
    void Start()
    {
        // Set the script to a var, we set the fuelbar gameobject to have the tag Player
        rocketScript = GameObject.FindGameObjectWithTag("Player").GetComponent<RocketScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // If rocket is pressed then reduce fuel
        if (isRocketOn){
            
            fuelLevel -= fuelDegradeRate;
        }

        if (fuelLevel > 75 && fuelLevel <= 100){
            fuelBar.GetComponent<RawImage>().texture = fuel100;
        }
        else if (fuelLevel > 50 && fuelLevel <= 75){
            fuelBar.GetComponent<RawImage>().texture = fuel75;
        }
        else if (fuelLevel > 25 && fuelLevel <= 50){
            fuelBar.GetComponent<RawImage>().texture = fuel50;
        }
        else if (fuelLevel > 0 && fuelLevel <= 25){
            fuelBar.GetComponent<RawImage>().texture = fuel25;
        }
        else if (fuelLevel <= 0){
            fuelBar.GetComponent<RawImage>().texture = fuel0;
            // Send to the rocket script that it is empty
            rocketScript.fuelIsEmpty = true;
        }
            
        
        

    }
}
