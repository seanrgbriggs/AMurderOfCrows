using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {

    public enum TileType
    {
        Dirt, Grass, Cobblestone, Brick, Wood, Metal, Wall, Blank, Door
    }

    public TileType type;
    public SpriteRenderer sr;
    [Header("Sprites")]
    public Sprite dirt;
    public Sprite grass;
    public Sprite cobble;
    public Sprite brick;
    public Sprite wood;
    public Sprite metal;
    public Sprite wall;
    public Sprite blank;
    public Sprite door;


    public bool isNavigable
    {
        get
        {
            return type != TileType.Wall;
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

    public void UpdateTile(TileType type)
    {
        this.type = type;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        switch (type) {
            case TileType.Door:
                spriteRenderer.sprite = door;
                break;
            case TileType.Grass:
                spriteRenderer.sprite = grass;
                break;
            default:
                spriteRenderer.sprite = GameController.instance.getSpriteForTile(type);
                break;
        } 
        GetComponent<Collider2D>().enabled = type == TileType.Wall;
    }
}
