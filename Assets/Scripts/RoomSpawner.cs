using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {



    static float elapsed = 0;
    public float timeToKillBoids = 15f;
    public int NUM_ROOMS = 16;
    public int NUM_PLAYERS = 4;
    public BoidSpawner spawner;
    public float roomSize;
    static int roomsSpawned = 0;
    float[,] coordinates;
    public bool playersSpawned = false;
    public Player playerTemp;

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

        if (elapsed >= timeToKillBoids && roomsSpawned < NUM_ROOMS) {
            SpawnRooms(boids);
            CreateRooms();
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
            boids[i].GetComponent<SpriteRenderer>().enabled = false;

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
                /*
                print("Top Left: " + left + ", " + top +
                "    Top Right: " + right + ", " + top +
                "    Bottom Right: " + right + ", " + down +
                "    Bottom Left: " + left + ", " + down);
                */
                roomsSpawned++;
                //print(roomsSpawned + ", " + boids[i].GetComponent<Boid>().spawnNum);
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
            Key key = Instantiate(GameController.instance.keyTemp, boids[NUM_PLAYERS + i].transform.position, Quaternion.identity);
            key.GetComponent<SpriteRenderer>().sprite = GameController.instance.keySprites[i];
            key.keyID = i;
            GameController.instance.keys[i] = key;
        }

        int rand = Random.Range(0, 4);
        GameController.instance.players[rand].isMurderer = true;
    }


    public class Room
    {
        public Tile.TileType type;
        public int x, y, size;
        public bool canExpand;
        //left edge is x value of left wall
        //top edge is y value of top wall

        public Room(Tile.TileType type, int x, int y, int size)
        {
            this.type = type;

            this.x = x;
            this.y = y;
            this.size = size;

            canExpand = true;
        }
    }

    void CreateRooms() {
        roomsSpawned = 0;

        TileManager tileManager = FindObjectOfType<TileManager>();
        GameController gc = GameController.instance;

        List<Room> rooms = new List<Room>();
        foreach (Boid b in FindObjectsOfType<Boid>()) {
            int x = Mathf.RoundToInt(b.transform.position.x);
            int y = Mathf.RoundToInt(b.transform.position.y);
            Tile.TileType tileType = gc.getTileForBoid(b.flockID);

            rooms.Add(new Room(tileType, x, y, 1));

            Tile t = tileManager.nearestToCoords(x, y);
            if (t == null) {
                rooms.RemoveAt(rooms.Count - 1);
            }
            else {
                t.UpdateTile(tileType);
            }
        }
        while (roomsSpawned < NUM_ROOMS) {
            for (int i = 0; i < rooms.Count; i++) {
                List<Tile> nextExpansion = new List<Tile>();

                Room curRoom = rooms[i];

                if (!curRoom.canExpand) {
                    continue;
                }

                /*
                for (int x = -2; x <= 2; x++) {
                    for (int y = -2; y <= 2; y++) {
                        Tile nextTile = tileManager.nearestToCoords(curRoom.x + x, curRoom.y + y);
                        if (nextTile != null && (nextTile.type == Tile.TileType.Grass || nextTile.type == Tile.TileType.Wall)) {
                            nextTile.UpdateTile(curRoom.type);
                        }
                    }
                }
                for (int x = -3; x <= 3; x ++) {
                    for (int y = -3; y <= 3; y ++) {
                        Tile nextTile = tileManager.nearestToCoords(curRoom.x + x, curRoom.y + y);
                        if (nextTile != null && nextTile.type == Tile.TileType.Grass) {
                            nextTile.UpdateTile(Tile.TileType.Wall);
                        }
                    }
                }*/
                nextExpansion.Clear();
                for (int x = -curRoom.size; x <= curRoom.size; x++) {
                    for (int y = -curRoom.size; y <= curRoom.size; y++) {
                        if (y == -curRoom.size || y == curRoom.size || x == -curRoom.size || x == curRoom.size) {
                            Tile nextTile = tileManager.nearestToCoords(curRoom.x + x, curRoom.y + y);
                            if (nextTile == null) {
                                curRoom.canExpand = false;
                            }
                            else if (nextTile.type != Tile.TileType.Grass) {
                                curRoom.canExpand = false;
                                nextExpansion.Add(nextTile);
                                //nextTile.UpdateTile(Tile.TileType.Wall);
                            }
                            else {
                                nextExpansion.Add(nextTile);
                            }
                        }
                    }
                }

                if (!curRoom.canExpand) {
                    int rand = Mathf.RoundToInt(Random.Range(0, nextExpansion.Count));
                    for (int m = 0; m < nextExpansion.Count; m++) {
                        if (nextExpansion[m].type == Tile.TileType.Grass) {
                            nextExpansion[m].UpdateTile(Tile.TileType.Wall);
                        }
                    }
                    nextExpansion[rand].UpdateTile(Tile.TileType.Door);
                    roomsSpawned++;
                    continue;
                }
                else {
                    curRoom.size++;
                    foreach (Tile t in nextExpansion) {
                        t.UpdateTile(curRoom.type);
                    }
                }
            }
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
        tileManager.SetTiles();
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
