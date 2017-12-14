using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int lightRange = 5;
    static int movePoints = 150;
    public int playerID = 0;

	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
        float vertical = 0;
        float horizontal = 0;

        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1;

        if(GameController.instance.currentPlayerIndex == playerID) {
            AttemptMove(vertical, horizontal);
        }
        
    }

    void AttemptMove(float vertical, float horizontal) {
        if (vertical != 0 || horizontal != 0) {
            movePoints--;
        }

        if (movePoints <= 0) {
            movePoints = 150;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GameController.instance.AdvanceTurn();
        }
        else {
            GetComponent<Rigidbody2D>().velocity = 3 * ((Vector2.up * vertical) + (Vector2.right * horizontal));
        }
    }
}
