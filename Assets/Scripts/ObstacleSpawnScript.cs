using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsticalSpawnScript : MonoBehaviour
{



    private float timer;

    public float spawnRate;

    public GameObject asteroid;

    public float newXPosition;


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

        // Find the position x of the spawner and add a random int to it to change the position of that spawned asteroid
        newXPosition = transform.position.x + Random.Range(-1.8f, 1.8f);

        Instantiate(asteroid, new Vector3(newXPosition, transform.position.y, 0), transform.rotation);

    }
}
