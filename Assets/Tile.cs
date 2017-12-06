using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {

    public enum TileType
    {
        Floor, Wall
    }

    public TileType type;
    public SpriteRenderer sr;
    public bool isNavigable
    {
        get
        {
            return type == TileType.Floor;
        }
    }
    
    public Player currentPlayer
    {
        get
        {
            return GameController.instance.currentPlayer;
        }
    }

    public void OnLightUpdate()
    {
        float maxLight = currentPlayer.lightRange;
        int distance = ManhattanFrom(currentPlayer.transform.position);

        sr.color = Color.Lerp(Color.white, Color.black, distance / maxLight);
    }

    private int ManhattanFrom(Vector2 position)
    {
        return Mathf.FloorToInt(Mathf.Abs(position.x - transform.position.x) + Mathf.Abs(position.y - transform.position.y));
    }
}
