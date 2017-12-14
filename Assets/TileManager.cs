using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public Tile tilePrefab;
    public int width = 17;
    public int height = 9;
    public float delta = 1;

    [HideInInspector]
    public Tile[,] tiles;

	// Use this for initialization
	void Start () {
        tiles = new Tile[width, height];
		for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, transform.position + (Vector3.right * i + Vector3.down * j) * delta, Quaternion.identity, transform);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
