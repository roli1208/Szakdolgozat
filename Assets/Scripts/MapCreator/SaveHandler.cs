using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.IO;
using System.Linq;

public class SaveHandler : Singleton<SaveHandler>
{
    Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    Dictionary<TileBase, BuildingObjectBase> tileBase = new Dictionary<TileBase, BuildingObjectBase>();
    Dictionary<string, TileBase> guidBase = new Dictionary<string, TileBase>();
    Dictionary<Vector3Int, int> CheckpointIDs = new Dictionary<Vector3Int, int>();
    [SerializeField] GameObject savePanel;
    [SerializeField] GameObject loadPanel;
    [SerializeField] TMP_InputField saveNameText;
    [SerializeField] TMP_Dropdown mapList;
    [SerializeField] BoundsInt bounds;
    [SerializeField] List<TilemapData> data;

    private void initTileReferences()
    {
        BuildingObjectBase[] buildables = Resources.LoadAll<BuildingObjectBase>("");

        Debug.Log("Buildables: " + buildables.Length);
        foreach (BuildingObjectBase buildable in buildables)
        {
            if (!tileBase.ContainsKey(buildable.TileBase))
            {
                tileBase.Add(buildable.TileBase, buildable);
                guidBase.Add(buildable.name, buildable.TileBase);
            }
            else
            {
                Debug.LogError("Tilebase is already in use!");
            }
        }
    }
    private void initTilemaps()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();
        Debug.Log("maps init: " + maps);

        foreach (var map in maps)
        {
            Debug.Log("map init: " + map);
            tilemaps.Add(map.name, map);
        }

    }

    public void showSavePanel()
    {
        savePanel.SetActive(true);
    }

    public void closeSavePanel()
    {
        savePanel.SetActive(false);
    }
    public void toJSON()
    {

    }
    public void onSave()
    {
        initTilemaps();
        initTileReferences();
        string fileName = saveNameText.text;
        List<TilemapData> data = new List<TilemapData>();
        CheckpointIDs = BuildingCreator.GetInstance().checkpointID;

        foreach (var map in tilemaps)
        {
            Debug.Log("map onsave: " + map.ToString());
            TilemapData mapData = new TilemapData();
            mapData.key = map.Key;
            TileInfo ti;

            BoundsInt mapBounds = map.Value.cellBounds;

            for (int x = mapBounds.xMin; x < mapBounds.xMax; x++)
            {
                for (int y = mapBounds.yMin; y < mapBounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    Debug.Log("map value: " + map.Value);
                    TileBase tile = map.Value.GetTile(pos);
                    Debug.Log("tile onsave: " + tile);
                    if (tile != null && tileBase.ContainsKey(tile))
                    {
                        Debug.Log("in if");
                        string guid = tileBase[tile].name;
                        Vector3Int key = new Vector3Int(x, y, 0);
                        if (CheckpointIDs.ContainsKey(key)) {
                            int id = CheckpointIDs[key];
                            ti = new TileInfo(pos, guid, id);
                        }
                        else { ti = new TileInfo(pos, guid, -1); }
                        Debug.Log("adding tile: " + ti);
                        mapData.tiles.Add(ti);
                    }
                }
            }
            Debug.Log("mapdata: " + mapData);
            data.Add(mapData);
        }
        TilemapData td = new TilemapData();
        td.numOfLaps = 8;
        td.numOfPlayers = 1;
        data.Add(td);

        FileHandler.SaveToJSON<TilemapData>(data, fileName + ".json");

    }

    public void onClickLoad()
    {
        loadPanel.SetActive(true);
        mapList.ClearOptions();
        string filePath = Application.persistentDataPath + "/maps/";
        Debug.Log(FileHandler.getMapNames(filePath));
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        mapList.AddOptions(FileHandler.getMapNames(filePath));
    }

    public void closeLoadPanel()
    {
        loadPanel.SetActive(false);
    }

    public void onLoad()
    {
        string selectedMapName = mapList.options[mapList.value].text;
        Debug.Log(selectedMapName);
        data = FileHandler.ReadListFromJSON<TilemapData>(selectedMapName);
        Debug.Log("data: " + data.ToString());
        try
        {
            foreach (var mapData in data)
            {
                Debug.Log("mapdata: " + mapData);
                Debug.Log("key: " + mapData.key);
                List<String> myKeys = tilemaps.Keys.ToList();
                foreach (string key in myKeys) {
                    Debug.Log("KEYS: " + key);
                }
                    var map = tilemaps[mapData.key];
                    Debug.Log("map: " + map);
                    map.ClearAllTiles();
                    Debug.Log("after clearalltiles: " + mapData.key);
                    Debug.Log("tiles: " + mapData.tiles);
                    Debug.Log("map tiles: " + mapData.tiles != null);
                    Debug.Log("map tiles count: " + mapData.tiles.Count);
                    Debug.Log("map tiles preview: " + mapData.key != "Preview");
                    if (mapData.tiles != null && mapData.tiles.Count > 0 && mapData.key != "Preview")
                    {
                        foreach (TileInfo tile in mapData.tiles)
                        {
                            Debug.Log("tile: " + tile);
                            if (guidBase.ContainsKey(tile.guid))
                            {
                                map.SetTile(tile.position, guidBase[tile.guid]);
                            }
                            else
                            {
                                Debug.LogError("Cannot found");
                            }

                        }
                }
            }
        } catch (Exception e)
        {
            Debug.Log(e);
        }
        loadPanel.SetActive(false);
    }
    public List<TilemapData> getMap()
    {
        return data;
    }
}
[Serializable]
public class TilemapData
{
    public string key;
    public List<TileInfo> tiles = new List<TileInfo>();
    public int numOfLaps;
    public int numOfPlayers;
}

[Serializable]
public class TileInfo
{
    public string guid;
    public Vector3Int position;
    public int id;

    public TileInfo(Vector3Int position, string guid, int id)
    {
        this.position = position;
        this.guid = guid;
        this.id = id;
    }
}