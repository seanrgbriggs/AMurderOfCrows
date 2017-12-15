using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Transition : MonoBehaviour {
    
    public Text txt;

    //public string player;

	// Use this for initialization
	void Start () {
        string player;
        int playerNum = GameController.instance.currentPlayerIndex;

        player = (playerNum == 0) ? "4" : playerNum.ToString();
        
        if (txt != null)
        {
            txt.text = player;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            SceneManager.UnloadScene("Transition");
        }
	}

}
