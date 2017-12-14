using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public List<Player> players;
    public int currentPlayerIndex = -1;

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

	// Use this for initialization
	void Start () {
        instance = this;	
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<RoomSpawner>().playersSpawned) {
            if (players[currentPlayerIndex].movePoints <= 0 || Input.GetKeyDown(KeyCode.Space)) {
                AdvanceTurn();
            }
        }
	}


    public void AdvanceTurn() {
        print("Advancing Turn");
        players[currentPlayerIndex].movePoints = 150;
        players[currentPlayerIndex].GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (currentPlayerIndex == players.Count - 1) {
            currentPlayerIndex = 0;
        }
        else {
            currentPlayerIndex++;
        }

        //Camera.main.transform.position = players[currentPlayerIndex].transform.position + new Vector3(0, 0, -10);
        //Camera.main.orthographicSize = 2.5F;
    }
}
