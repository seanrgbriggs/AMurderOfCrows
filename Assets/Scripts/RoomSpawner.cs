using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    private enum TilePatternTemplate
    {
        Any, Floor, Wall, Door
    }

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

        Boid[] boids = FindObjectsOfType<Boid>();
        System.Array.Sort(boids, (x, y) => x.spawnNum.CompareTo(y.spawnNum));

        if (boids.Length >= NUM_ROOMS) {
            spawner.shouldSpawn = false;
        }

        if (elapsed >= timeToKillBoids && roomsSpawned < NUM_ROOMS) {
            AlignBoidsToGrid(boids);
            CreateRooms();
        }

        if(roomsSpawned >= NUM_ROOMS && !playersSpawned) {
            SpawnPlayers(boids);
            GameController.instance.AdvanceTurn();
        }
    }

    //IDEA: Have the boid keep moving if it would overlap with other rooms.  
    void AlignBoidsToGrid(Boid[] boids) {
        for (int i = 0; i < NUM_ROOMS; i++) {
            bool overlaps = false;

            float left = boids[i].transform.position.x - roomSize / 2;
            float down = boids[i].transform.position.y - roomSize / 2;

            boids[i].shouldMove = false;
            boids[i].spriteRenderer.enabled = false;

            Vector3 pos = boids[i].transform.position;
            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);

            pos.x = Mathf.Sign(pos.x) * Mathf.Clamp(Mathf.Abs(pos.x), 0, boids[i].GetComponent<LeftRightWrap>().rightX - 1);
            pos.y = Mathf.Sign(pos.y) * Mathf.Clamp(Mathf.Abs(pos.y), 0, boids[i].GetComponent<LeftRightWrap>().topY - 1);

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
                boids[i].isDone = true;
                boids[i].rb.velocity = Vector2.zero;

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
            else if(boids[i].isDone == false){
                //print("ding, " + boids[i].GetComponent<Boid>().spawnNum);
                //boids[i].GetComponent<Rigidbody2D>().velocity = Vector2.down;
            }
        }
    }

    void SpawnPlayers(Boid[] boids) {
        playersSpawned = true;

        Color[] playerColors = new Color[] { Color.red, Color.blue, Color.green, Color.magenta };
        for (int i = 0; i < NUM_PLAYERS; i++) {
            Player player = Instantiate(playerTemp, boids[i].transform.position, Quaternion.identity);
            player.playerID = i;
            player.color = playerColors[i];
            player.GetComponent<SpriteRenderer>().color = player.color;
            GameController.instance.players.Add(player);
        }

        for(int i = 0; i < 4; i++) {
            Key key = Instantiate(GameController.instance.keyPrefab, boids[NUM_PLAYERS + i].transform.position, Quaternion.identity);
            key.GetComponent<SpriteRenderer>().sprite = GameController.instance.keySprites[i];
            key.keyID = i;
            GameController.instance.keys[i] = key;
        }

        Instantiate(GameController.instance.wingsPrefab, boids[NUM_PLAYERS + 4].transform.position, Quaternion.identity);

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


        //Construct the list of room points.
        List<Room> rooms = new List<Room>();
        foreach (Boid b in FindObjectsOfType<Boid>()) {
            int x = Mathf.RoundToInt(b.transform.position.x);
            int y = Mathf.RoundToInt(b.transform.position.y);
            Tile.TileType tileType = gc.getTileForBoid(b.flockID);

            rooms.Add(new Room(tileType, x, y, 1));

            Tile t = tileManager.nearestToCoords(x, y);
            if (t == null) {
                rooms.RemoveAt(rooms.Count - 1);
                Destroy(b.gameObject);
            }
            else {
                t.UpdateTile(tileType);
            }
        }

        //Build all rooms in the list
        while (roomsSpawned < NUM_ROOMS) {
            for (int i = 0; i < rooms.Count; i++) {
                List<Tile> nextExpansion = new List<Tile>();

                Room curRoom = rooms[i];

                if (!curRoom.canExpand) {
                    continue;
                }


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
                    for (int m = 0; m < nextExpansion.Count; m++) {
                        if (nextExpansion[m].type == Tile.TileType.Grass) {
                            nextExpansion[m].UpdateTile(Tile.TileType.Wall);
                        }
                    }

                    //int rand = Mathf.RoundToInt(Random.Range(0, nextExpansion.Count));
                    //nextExpansion[rand].UpdateTile(Tile.TileType.Door);
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
            spawnDoors();
            RemoveExtraDoors();

            //Finally, clean up the boids
            foreach (Boid b in FindObjectsOfType<Boid>())
            {
                Destroy(b.gameObject);
            }
        }
    }

    void spawnDoors() {
        TilePatternTemplate[,] threeByThreeVertical = new TilePatternTemplate[3, 3] {
            { TilePatternTemplate.Any, TilePatternTemplate.Floor, TilePatternTemplate.Any },
            { TilePatternTemplate.Wall, TilePatternTemplate.Wall, TilePatternTemplate.Wall },
            { TilePatternTemplate.Any, TilePatternTemplate.Floor, TilePatternTemplate.Any}
            };

        TilePatternTemplate[,] threeByThreeHorizontal = new TilePatternTemplate[3, 3] {
            { TilePatternTemplate.Any, TilePatternTemplate.Wall, TilePatternTemplate.Any },
            { TilePatternTemplate.Floor, TilePatternTemplate.Wall, TilePatternTemplate.Floor},
            { TilePatternTemplate.Any, TilePatternTemplate.Wall, TilePatternTemplate.Any}
            };

        //Add Doors to the rooms, where there are two tiles on opposite sides of a 1x3 wall.
        Tile[,] tiles = TileManager.instance.tiles;
            for (int i = 1; i < TileManager.instance.width - 1; i++) {
                for (int j = 0; j < TileManager.instance.height - 3; j++) { 
                    if(CheckPattern(threeByThreeVertical, i, j) || CheckPattern(threeByThreeHorizontal,i,j))
                    {
                        tiles[i + 1, j + 1].UpdateTile(Tile.TileType.Door);
                    }
                }
            }

        //Store the original values of all tiles
        TileManager.instance.SetTiles();


    }

    private bool CheckPattern(TilePatternTemplate[,] pattern, int x, int y)
    {
        List<Tile.TileType> dataDump;
        return CheckPattern(pattern, x, y, out dataDump);
    }

    private bool CheckPattern(TilePatternTemplate[,] pattern, int x, int y, out List<Tile.TileType> types) {
        types = new List<Tile.TileType>();
        TileManager tileManager = TileManager.instance;
        for (int i = 0; i < pattern.GetLength(0); i++) {
            for (int j = 0; j < pattern.GetLength(1); j++) {
                if (i + x >= tileManager.tiles.GetLength(0) || j + y >= tileManager.tiles.GetLength(1)) {
                    return false;
                }

                Tile curTile = TileManager.instance.tiles[i + x, j + y];

                if (curTile == null) {
                    return false;
                }

                types.Add(curTile.type);

                switch (pattern[i, j]) {

                    case TilePatternTemplate.Any:
                        continue;
                    case TilePatternTemplate.Floor:
                        if (curTile.type == Tile.TileType.Wall || curTile.type == Tile.TileType.Door)
                            return false;
                        break;
                    case TilePatternTemplate.Door:
                        if (curTile.type != Tile.TileType.Door)
                            return false;
                        break;
                    case TilePatternTemplate.Wall:
                        if (curTile.type != Tile.TileType.Wall)
                            return false;
                        break;
                }
            }
        }
        return true;
    }

    void RemoveExtraDoors() {
        TilePatternTemplate[,] verticalFive = new TilePatternTemplate[1,5] {
            { TilePatternTemplate.Door,
            TilePatternTemplate.Any,
            TilePatternTemplate.Door,
            TilePatternTemplate.Any,
            TilePatternTemplate.Door }
        };

        TilePatternTemplate[,] horizontalFive = new TilePatternTemplate[5, 1] {
            { TilePatternTemplate.Door },
            {TilePatternTemplate.Any },
            {TilePatternTemplate.Door },
            { TilePatternTemplate.Any },
            { TilePatternTemplate.Door }
        };

        TilePatternTemplate[,] verticalDoubleDoors = new TilePatternTemplate[3, 3]
        {
            {TilePatternTemplate.Floor, TilePatternTemplate.Floor, TilePatternTemplate.Floor },
            {TilePatternTemplate.Door, TilePatternTemplate.Wall, TilePatternTemplate.Door },
            {TilePatternTemplate.Floor, TilePatternTemplate.Floor, TilePatternTemplate.Floor}
        };

        TilePatternTemplate[,] horizontalDoubleDoors = new TilePatternTemplate[3, 3]
        {
            {TilePatternTemplate.Floor, TilePatternTemplate.Door, TilePatternTemplate.Floor },
            {TilePatternTemplate.Floor, TilePatternTemplate.Wall, TilePatternTemplate.Floor },
            {TilePatternTemplate.Floor, TilePatternTemplate.Door, TilePatternTemplate.Floor}
        };

        //Add Doors to the rooms, where there are two tiles on opposite sides of a 1x3 wall.
        Tile[,] tiles = TileManager.instance.tiles;
        for (int rescan = 0; rescan < 10; rescan++)
        {
            for (int i = 0; i < TileManager.instance.width; i++)
            {
                for (int j = 0; j < TileManager.instance.height; j++)
                {
                    List<Tile.TileType> data;
                    if (CheckPattern(verticalFive, i, j))
                    {
                        tiles[i, j + 2].UpdateTile(Tile.TileType.Wall);
                    }
                    else if (CheckPattern(horizontalFive, i, j))
                    {
                        tiles[i + 2, j].UpdateTile(Tile.TileType.Wall);
                    }
                    else if (CheckPattern(verticalDoubleDoors, i, j, out data))
                    {

                        TileManager.instance.tiles[i + 1, j - 1].UpdateTile(Tile.TileType.Wall);
                        TileManager.instance.tiles[i + 1, j].UpdateTile(Tile.TileType.Wall);
                        TileManager.instance.tiles[i + 1, j + 1].UpdateTile(Tile.TileType.Wall);

                    }
                    else if (CheckPattern(horizontalDoubleDoors, i, j))
                    {

                        tiles[i - 1, j + 1].UpdateTile(Tile.TileType.Wall);
                        tiles[i, j + 1].UpdateTile(Tile.TileType.Wall);
                        tiles[i + 1, j + 1].UpdateTile(Tile.TileType.Wall);
                    }else if(tiles[i,j].type == Tile.TileType.Door)
                    {
                        int adjacentWalls = 0;
                        new List<Tile>() { tiles[i - 1, j], tiles[i + 1, j], tiles[i, j - 1], tiles[i, j + 1] }
                            .ForEach(tile => adjacentWalls += (tile.type == Tile.TileType.Wall) ? 1 : 0);
                        if(adjacentWalls > 2)
                        {
                            tiles[i, j].UpdateTile(Tile.TileType.Wall);
                        }
                    }
                }
            }
        }
        

        //Store the original values of all tiles
        TileManager.instance.SetTiles();

        //Finally, clean up the boids
        foreach (Boid b in FindObjectsOfType<Boid>()) {
            Destroy(b.gameObject);
        }

    }
     
}
