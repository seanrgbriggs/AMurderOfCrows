using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int lightRange = 5;
    public int movePoints = 225;
    public int playerID = 0;
    public int keysCollected = 0;
    public Sprite[] sprites;

	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
        int vertical = 0;
        int horizontal = 0;

        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1;

        if(GameController.instance.currentPlayerIndex == playerID) {
            AttemptMove(vertical, horizontal);
        }
        else {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        
    }

    void AttemptMove(int vertical, int horizontal) {
        if(horizontal == -1) {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        } else if (horizontal == 1) {
            GetComponent<SpriteRenderer>().sprite = sprites[2];
        } else if (vertical == 1) {
            GetComponent<SpriteRenderer>().sprite = sprites[3];
        } else {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }

        if((transform.position.x >= 26 && horizontal == 1) || (transform.position.x <= -26 && horizontal == -1)) {
            horizontal = 0;
        }
        if ((transform.position.y >= 14 && vertical == 1) || (transform.position.y <= -14 && vertical == -1)) {
            vertical = 0;
        }

        movePoints -= Math.Abs(vertical) + Math.Abs(horizontal);

        GetComponent<Rigidbody2D>().velocity = 2 * ((Vector2.up * vertical) + (Vector2.right * horizontal));
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.tag.Equals("Key")) {
            Destroy(collision.collider.gameObject);
            keysCollected++;
            print("Got Eeeeeeem");
        }
    }
}
