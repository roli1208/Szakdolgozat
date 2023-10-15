using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : TileBase
{
    public int id; // The ID of the tile

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // Call the base method to set up the tile
        base.StartUp(position, tilemap, go);

        // Set the custom property on the tile
        go.GetComponent<CustomTile>().id = 2;

        return true;
    }
}
