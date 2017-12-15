using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public Tile tilePrefab;
    public int width = 17;
    public int height = 9;
    public float delta = 1;
    public bool shouldHideExternalTiles = true;

    [HideInInspector]
    public Tile[,] tiles;
    public Tile.TileType[,] origTiles;

    public static TileManager instance;

    void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        tiles = new Tile[width, height];
        origTiles = new Tile.TileType[width, height];
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, transform.position + (Vector3.right * i + Vector3.down * j) * delta, Quaternion.identity, transform);
            }
        }
    }
	

	public void UpdateTiles() {
        ResetTiles();

        if (shouldHideExternalTiles)
        {
            HideExternalTiles();
        }

    }

    private void HideExternalTiles()
    {
        Transform player = GameController.instance.currentPlayer.transform;
        int x = Mathf.RoundToInt(player.position.x);
        int y = Mathf.RoundToInt(player.position.y);

        Tile.TileType playerType = nearestToCoords(x, y).type;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile.TileType type = tiles[i, j].type;
                if (type != playerType && type != Tile.TileType.Wall && type != Tile.TileType.Door)
                {
                    tiles[i, j].UpdateTile(Tile.TileType.Blank);
                }
            }
        }
    }

    public void ResetTiles() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                tiles[i, j].UpdateTile(origTiles[i, j]);
            }
        }
    }

    public void SetTiles() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                origTiles[i, j] = tiles[i, j].type;
            }
        }
    }

    public Tile nearestToCoords(int x, int y)
    {
        int deltaX = x - Mathf.RoundToInt(transform.position.x);
        int deltaY = Mathf.RoundToInt(transform.position.y) - y;


        if(deltaX < 0 || deltaX >= width || deltaY < 0 || deltaY >= height)
        {
            return null;
        }

        return tiles[deltaX, deltaY];
    }
}
