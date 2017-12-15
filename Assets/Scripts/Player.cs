using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Player Base Values")]
    public int lightRange = 5;
    [HideInInspector]
    public int movePoints = 225;
    public int maxMovePoints = 225;

    [Header("Player Game Stats")]
    public int playerID = 0;
    public int keysCollected = 0;
    public bool isMurderer = false;
    public Tile.TileType standingOn;
    public bool[] hasKey = { false, false, false, false };
    [Header("Aesthetics")]
    public Sprite[] sprites;
    public Color color;
    public Light light;

    // Use this for initialization
    void Start()
    {
        standingOn = TileManager.instance.nearestToCoords(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)).type;
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
            if (Input.GetKeyDown(KeyCode.M)) print(isMurderer);
            if (Input.GetKeyDown(KeyCode.S)) print(standingOn);
            if (Input.GetKeyDown(KeyCode.K)) print(hasKey[0] + ", " + hasKey[1] + ", " + hasKey[2] + ", " + hasKey[3]);

            AttemptMove(vertical, horizontal);

            Tile.TileType type = TileManager.instance.nearestToCoords(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)).type;
            if (type != standingOn && type != Tile.TileType.Door) {
                standingOn = TileManager.instance.nearestToCoords(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)).type;
                TileManager.instance.UpdateTiles();
            }
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
            hasKey[collision.collider.GetComponent<Key>().keyID] = true;
            Destroy(collision.collider.gameObject);
            keysCollected++;
            print("Got Eeeeeeem");
        }

        if (collision.collider.gameObject.tag.Equals("Player")) {
            if (isMurderer) {
                for(int i = 0; i < 4; i++) {
                    if(collision.collider.gameObject.GetComponent<Player>().hasKey[i]) {
                        hasKey[i] = false;
                        Key key = Instantiate(GameController.instance.keyTemp, collision.collider.transform.position, Quaternion.identity);
                        key.GetComponent<SpriteRenderer>().sprite = GameController.instance.keySprites[i];
                        key.keyID = i;
                        GameController.instance.keys[i] = key;
                    }
                }
                GameController.instance.alive[collision.collider.gameObject.GetComponent<Player>().playerID] = false;
                Destroy(collision.collider.gameObject);
            }
        }
    }

    void OnDestroy() {
        //GameController.instance.players.Remove(this);
    }
}
