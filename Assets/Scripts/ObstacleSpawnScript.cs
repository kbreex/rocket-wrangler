using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsticalSpawnScript : MonoBehaviour
{



    private float timer;

    public float spawnRate;

    public GameObject asteroid;


    // Start is called before the first frame update
    void Start()
    {
        // Set timer to 0 and the spawn rate and spawn the first asteroid
        timer = 0;
        spawnRate = 5;

        SpawnAsteroid();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate){
            // Incament the timer
            timer += Time.deltaTime;
        }
        else{
            // Spawn the next asteroid and set the time to 0
            SpawnAsteroid();
            timer = 0;
        }

    }

    void SpawnAsteroid(){

        Instantiate(asteroid, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);

    }
}
