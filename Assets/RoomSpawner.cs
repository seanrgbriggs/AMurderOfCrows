using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    static float elapsed = 0;
    public BoidSpawner spawner;
    public float roomSize;
    static int roomsSpawned = 0;
    float[,] coordinates = new float[16, 2];

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

        if (elapsed >= 30 && roomsSpawned < 16) {
            SpawnRooms(boids);
        }
    }

    //IDEA: Have the boid keep moving if it would overlap with other rooms.  
    void SpawnRooms(GameObject[] boids) {
        System.Array.Sort(boids, (x, y) => x.GetComponent<Boid>().spawnNum.CompareTo(y.GetComponent<Boid>().spawnNum));

        for (int i = 0; i < 16; i++) {
            bool overlaps = false;

            float left = boids[i].transform.position.x - roomSize / 2;
            float down = boids[i].transform.position.y - roomSize / 2;

            boids[i].GetComponent<Boid>().shouldMove = false;

            for (int j = 0; j < 16; j++) {
                if(coordinates[j, 0] != 0 && !(left > coordinates[j, 0] + roomSize || 
                                                  left + roomSize < coordinates[j, 0] ||
                                                  down > coordinates[j, 1] + roomSize ||
                                                  down + roomSize < coordinates[j, 1])){
                    overlaps = true;
                }
            }
            if (!overlaps) {
                boids[i].GetComponent<Boid>().isDone = true;
                boids[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                coordinates[i, 0] = left;
                coordinates[i, 1] = down;

                float right = coordinates[i, 0] + roomSize;
                float top = coordinates[i, 1] + roomSize;

                print("Top Left: " + left + ", " + top +
                "    Top Right: " + right + ", " + top +
                "    Bottom Right: " + right + ", " + down +
                "    Bottom Left: " + left + ", " + down);

                roomsSpawned++;
                print(roomsSpawned + ", " + boids[i].GetComponent<Boid>().spawnNum);
            }
            else if(boids[i].GetComponent<Boid>().isDone == false){
                print("ding, " + boids[i].GetComponent<Boid>().spawnNum);
                //boids[i].GetComponent<Rigidbody2D>().velocity = Vector2.down;
            }
        }
    }
}
