using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [System.Serializable]
    public class BoidColorTileMatch {
        public Boid.BoidType boidType;
        public Tile.TileType tileType;
        public Sprite tileSprite;
    }

    public static GameController instance;

    public List<Player> players;
    public int currentPlayerIndex = -1;
    public List<BoidColorTileMatch> colorTileDictionary;
    public Key keyTemp;
    public bool[] alive = { true, true, true, true };
    //Sprites
    public Sprite[] keySprites;

    [HideInInspector]
    public Key[] keys;

    public Player currentPlayer
    {
        get
        {
            return players[currentPlayerIndex];
        }
        set
        {
            currentPlayerIndex = players.FindIndex(p => p == value);
        }
    }

    void Awake() {
        instance = this;
        keys = new Key[4];
    }

	// Use this for initialization
	void Start () {
        List<Boid.BoidType> boidTypes = new List<Boid.BoidType>((Boid.BoidType[]) System.Enum.GetValues(typeof(Boid.BoidType)));
        for(int i = 0; i < colorTileDictionary.Count; i++) {
            int randomIndex = Random.Range(0, boidTypes.Count);
            colorTileDictionary[i].boidType = boidTypes[randomIndex];
            boidTypes.RemoveAt(randomIndex);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<RoomSpawner>().playersSpawned) {
            if (!alive[currentPlayerIndex]) {
                AdvanceTurn();
                return;
            }

            if (players[currentPlayerIndex].movePoints <= 0 || Input.GetKeyDown(KeyCode.Space)) {
                AdvanceTurn();
            }

            Camera.main.transform.position = players[currentPlayerIndex].transform.position + new Vector3(0, 0, -10);
            Camera.main.orthographicSize = 5;
        }
	}


    public void AdvanceTurn() {
        players[currentPlayerIndex].movePoints = 150;
        players[currentPlayerIndex].GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (currentPlayerIndex == players.Count - 1) {
            currentPlayerIndex = 0;
        }
        else {
            currentPlayerIndex++;
        }

        TileManager.instance.UpdateTiles();
    }

    public Tile.TileType getTileForBoid(Boid.BoidType boidType) {
        BoidColorTileMatch bctm = colorTileDictionary.Find(match => match.boidType == boidType);
        return bctm.tileType;
    }

    public Sprite getSpriteForTile(Tile.TileType tileType) {
        BoidColorTileMatch bctm = colorTileDictionary.Find(match => match.tileType == tileType);

        return (bctm == null) ? null: bctm.tileSprite;
    }
}
