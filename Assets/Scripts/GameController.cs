using System.Collections;
using UnityEngine.SceneManagement;
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
    public List<BoidColorTileMatch> colorTileDictionary;

    [Header("Player Information")]
    public List<Player> players;
    public int currentPlayerIndex = -1;
    public bool[] alive = { true, true, true, true };

    [Header("Prefabs")]
    public Key keyPrefab;
    public ExitWings wingsPrefab;

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
        DontDestroyOnLoad(this);
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
                if (currentPlayerIndex == players.Count - 1) {
                    currentPlayerIndex = 0;
                }
                else {
                    currentPlayerIndex++;
                }

                TileManager.instance.UpdateTiles();
                return;
            }

            if (currentPlayer.movePoints <= 0 || (!SceneManager.GetSceneByName("Transition").isLoaded && Input.GetKeyDown(KeyCode.Space))) {
                AdvanceTurn();
            }

            if (!alive[currentPlayerIndex]) return;
    
            
            Camera.main.transform.position = currentPlayer.transform.position + new Vector3(0, 0, -10);
            Camera.main.orthographicSize = 5;
        }
	}


    public void AdvanceTurn() {
        currentPlayer.movePoints = currentPlayer.maxMovePoints;

        currentPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        currentPlayer.light.enabled = false;

        if (currentPlayerIndex == players.Count - 1) {
            currentPlayerIndex = 0;
        }
        else {
            currentPlayerIndex++;
        }

        if (!alive[currentPlayerIndex]) return;

        currentPlayer.light.enabled = true;
        SceneManager.LoadScene("Transition", LoadSceneMode.Additive); //Commenting because I'm not a scrublord, Sean "I don't comment my shit like Matt Fortmeyer" briggs
                                                                      //It calls the transition.
                                                                      //You can't say you're commenting and then follow up with a four-word comment, nerd
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
