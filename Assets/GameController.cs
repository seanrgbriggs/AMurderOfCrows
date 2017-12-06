using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public List<Player> players;
    public int currentPlayerIndex = 0;

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
		
	}
	
	// Update is called once per frame
	void Update () {
	    	
	}
}
