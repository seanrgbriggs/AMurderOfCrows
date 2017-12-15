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
    public bool isFree = false;

    [Header("Aesthetics")]
    public Sprite[] sprites;
    public Sprite freedomSprite;
    public Color color;
    public new Light light;

    // Use this for initialization
    void Start()
    {
        standingOn = TileManager.instance.nearestToCoords(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)).type;
    }
	
	// Update is called once per frame
	void Update () {

        if (isFree)
        {
            GetComponent<SpriteRenderer>().sprite = freedomSprite;
            GetComponent<Collider2D>().enabled = false;
            transform.position += Vector3.up * Time.deltaTime;
            return;
        }

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

        movePoints -= Mathf.Abs(vertical) + Mathf.Abs(horizontal);

        GetComponent<Rigidbody2D>().velocity = 2 * ((Vector2.up * vertical) + (Vector2.right * horizontal));
    }

    void OnCollisionEnter2D(Collision2D collision) {
        
        if (isMurderer && collision.collider.gameObject.tag.Equals("Player"))
        {
            for (int i = 0; i < 4; i++)
            {
                if (collision.collider.GetComponent<Player>().hasKey[i])
                {
                    hasKey[i] = false;
                    Key key = Instantiate(GameController.instance.keyPrefab, collision.collider.transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                    key.GetComponent<SpriteRenderer>().sprite = GameController.instance.keySprites[i];
                    key.keyID = i;
                    GameController.instance.keys[i] = key;
                }
            }

            GameController.instance.alive[collision.collider.GetComponent<Player>().playerID] = false;
            Destroy(collision.collider.gameObject);

            if (System.Array.FindAll(GameController.instance.alive, living => !living).Length >= GameController.instance.players.Count - 1)
            {
                isFree = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isMurderer && collider.gameObject.tag.Equals("Key"))
        {
            hasKey[collider.GetComponent<Key>().keyID] = true;
            Destroy(collider.gameObject);
            keysCollected++;
            //print("Got Eeeeeeem");
        }
    }

    void OnDestroy() {
        //GameController.instance.players.Remove(this);
    }
}
