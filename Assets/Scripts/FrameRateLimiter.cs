using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    public int targetFrameRate = 60;

    void Start()
    {
        // Set the target frame rate
        Application.targetFrameRate = targetFrameRate;
    }
}
