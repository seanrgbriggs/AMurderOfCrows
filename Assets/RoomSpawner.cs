using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    static float elapsed = 0;
    public int NUM_ROOMS = 16;
    public int NUM_PLAYERS = 4;
    public BoidSpawner spawner;
    public float roomSize;
    static int roomsSpawned = 0;
    float[,] coordinates;
    public bool playersSpawned = false;
    public Player playerTemp;
    public Key keyTemp;

    //Sprites
    public Sprite[] keySprites;

    // Use this for initialization
    void Start () {
        coordinates = new float[NUM_ROOMS, 2];
    }
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;

        GameObject[] boids = GameObject.FindGameObjectsWithTag("Boid");
        System.Array.Sort(boids, (x, y) => x.GetComponent<Boid>().spawnNum.CompareTo(y.GetComponent<Boid>().spawnNum));

        if (boids.Length >= NUM_ROOMS) {
            spawner.shouldSpawn = false;
        }

        if (elapsed >= 30 && roomsSpawned < NUM_ROOMS) {
            SpawnRooms(boids);
        }

        if(roomsSpawned >= NUM_ROOMS && !playersSpawned) {
            SpawnPlayers(boids);
            GameController.instance.AdvanceTurn();
        }
    }

    //IDEA: Have the boid keep moving if it would overlap with other rooms.  
    void SpawnRooms(GameObject[] boids) {
        for (int i = 0; i < NUM_ROOMS; i++) {
            bool overlaps = false;

            float left = boids[i].transform.position.x - roomSize / 2;
            float down = boids[i].transform.position.y - roomSize / 2;

            boids[i].GetComponent<Boid>().shouldMove = false;

            Vector3 pos = boids[i].transform.position;
            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);
            boids[i].transform.position = pos;


            for (int j = 0; j < NUM_ROOMS; j++) {
                if(coordinates[j, 0] != 0 && !(left >= coordinates[j, 0] + roomSize || 
                                                  left + roomSize <= coordinates[j, 0] ||
                                                  down >= coordinates[j, 1] + roomSize ||
                                                  down + roomSize <= coordinates[j, 1])){
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
                //print("ding, " + boids[i].GetComponent<Boid>().spawnNum);
                //boids[i].GetComponent<Rigidbody2D>().velocity = Vector2.down;
            }
        }
    }

    void SpawnPlayers(GameObject[] boids) {
        playersSpawned = true;
        for (int i = 0; i < NUM_PLAYERS; i++) {
            Player player = Instantiate(playerTemp, boids[i].transform.position, Quaternion.identity);
            player.playerID = i;
            GameController.instance.players.Add(player);
        }

        for(int i = 0; i < 4; i++) {
            Key key = Instantiate(keyTemp, boids[NUM_PLAYERS + i].transform.position, Quaternion.identity);
            key.GetComponent<SpriteRenderer>().sprite = keySprites[i];
        }
        
    }


    public struct Room
    {
        public int topEdge, bottomEdge, leftEdge, rightEdge;
        //left edge is x value of left wall
        //top edge is y value of top wall

        public Room(int te, int be, int le, int re)
        {
            topEdge = te;
            bottomEdge = be;
            leftEdge = le;
            rightEdge = re;
        }
    }

    void CreateRooms() {
        /*
            for each boid
                create room at boid location

            roomsCanExpand = true
            while(roomsCanExpand)
                roomsCanExpand = false
                for each room
                    if(canRoomExpandLeft)
                        roomsCanExpand = true;
                        ExpandRoomLeft
                    if(canRoomExpandRight)
                        roomsCanExpand = true;
                        ExpandRoomRight
                    if(canRoomExpandUp)
                        roomsCanExpand = true;
                        ExpandRoomUp
                    if(canRoomExpandDown)
                        roomsCanExpand = true;
                        ExpandRoomDown
        


         */
    }

    bool canRoomExpandLeft(Room room) {
        /*
            int x = room.leftEdge - 1;
            int y = room.topEdge;

            while(y <= room.bottomEdge)
                if(world(x, y).isNotEmpty)
                    return false;
         */

        return true;
    }

    void expandRoomLeft(Room room) {
        /*
            int x = room.leftEdge - 1;
            int y = room.topEdge;

            while(y <= room.bottomEdge)
                world(x, y) = room

            room.leftEdge += 1;
         */
    }
}
