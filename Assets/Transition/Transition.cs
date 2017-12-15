using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Transition : MonoBehaviour {

    [SerializeField]
    private Text txt = null;

    public string player;

	// Use this for initialization
	void Start () {
        player = GameController.instance.currentPlayerIndex.ToString();
        if(player == "0")
        {
            player = "4"; 
        }
        if (txt != null)
        {
            txt.text = player;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            SceneManager.UnloadScene("Transition");
        }
	}

}
