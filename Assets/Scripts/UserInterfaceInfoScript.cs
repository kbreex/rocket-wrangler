using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceInfoScript : MonoBehaviour
{   
    public Text distanceText;

    [ContextMenu("Add Distance")]
    public void AddDistance(float distance){


        distanceText.text = distance.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
