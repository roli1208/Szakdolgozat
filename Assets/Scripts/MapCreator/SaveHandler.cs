using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveHandler : MonoBehaviour
{
    Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    [SerializeField] BoundsInt bounds;
    [SerializeField] string filename = "tilemapData.json";
    private void Start()
    {
        initTilemaps();
    }

    private void initTilemaps()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach(var map in maps)
        {
            tilemaps.Add(map.name, map);
        }
    }

    public void onSave()
    {
        List<TilemapData> data = new List<TilemapData>();

        foreach(var map in tilemaps)
        {
            TilemapData mapData = new TilemapData();
            mapData.key = map.Key;


            BoundsInt mapBounds = map.Value.cellBounds;

            for(int x = mapBounds.xMin; x < mapBounds.xMax; x++)
            {
                for (int y = mapBounds.yMin; y < mapBounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0); 
                    TileBase tile = map.Value.GetTile(pos);

                    if(tile != null)
                    {
                        TileInfo ti = new TileInfo(tile,pos);
                        mapData.tiles.Add(ti);
                    }
                }
            }
            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, filename);

    }
}
[Serializable]
public class TilemapData
{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public TileBase tile;
    public Vector3Int position;

    public TileInfo(TileBase tile, Vector3Int position)
    {
        this.tile = tile;
        this.position = position;
    }
}