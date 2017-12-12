using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    static float elapsed = 0;
    public BoidSpawner spawner;
    public float roomSize;
    bool roomsSpawned = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;

        GameObject[] boids = GameObject.FindGameObjectsWithTag("Boid");

        if(boids.Length >= 16) {
            spawner.shouldSpawn = false;
        }

        if (elapsed >= 30 && !roomsSpawned) {
            foreach(GameObject b in boids) {
                b.GetComponent<Boid>().shouldMove = false;
                b.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            SpawnRooms(boids);
        }
    }

    //IDEA: Have the boid keep moving if it would overlap with other rooms.  
    void SpawnRooms(GameObject[] boids) {
        float[,] coordinates = new float[16, 2];

        int count = 0;

        foreach (GameObject b in boids) {
            float left = b.transform.position.x - roomSize / 2;
            float down = b.transform.position.y - roomSize / 2;

            coordinates[count, 0] = left;
            coordinates[count, 1] = down;

            count++;
        }

        for(int i = 0; i < 16; i++) {
            float left = coordinates[i, 0];
            float right = coordinates[i, 0] + roomSize;
            float bottom = coordinates[i, 1];
            float top = coordinates[i, 1];


            print("Top Left: " + left + ", "  + top +   
                "    Top Right: " + right + ", " + top +
                "    Bottom Right: " + right + ", " + bottom +
                "    Bottom Left: " + left + ", " + bottom);
        }
        roomsSpawned = true;
    }
}
