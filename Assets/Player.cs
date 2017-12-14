using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int lightRange = 5;
    public int movePoints = 150;
    public int playerID = 0;
    public int keysCollected = 0;

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
        else {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        
    }

    void AttemptMove(float vertical, float horizontal) {
        if (vertical != 0 || horizontal != 0) {
            movePoints--;
        }
        GetComponent<Rigidbody2D>().velocity = 3 * ((Vector2.up * vertical) + (Vector2.right * horizontal));
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.tag.Equals("Key")) {
            Destroy(collision.collider.gameObject);
            keysCollected++;
            print("Got Eeeeeeem");
        }
    }
}
