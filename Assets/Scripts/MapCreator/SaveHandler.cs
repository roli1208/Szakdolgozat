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
    [SerializeField] List<Waypoint> waypoints = new List<Waypoint>();
    [SerializeField] public GameObject waypoint;
    [SerializeField] GameObject AICar;
    [SerializeField] GameObject PlayerCar;

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
    public void onSave()
    {
        closeSavePanel();
        initTilemaps();
        initTileReferences();
        waypoints = BuildingCreator.GetInstance().waypoints;
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
        data.Add(td);
        Debug.Log("waypoints: " + waypoints.Count);
        FileHandler.SaveToJSON<TilemapData>(waypoints, data, fileName + ".json");

    }

    public void closeLoadPanel()
    {
        loadPanel.SetActive(false);
    }


    public void onLoad(string name)
    {
        Debug.Log("LOAD MAP: " + name);
        initTilemaps();
        initTileReferences();
        string selectedMapName = name;
        data = FileHandler.ReadListFromJSON<TilemapData>(selectedMapName);
        try
        {
            foreach (var mapData in data)
            {
                List<String> myKeys = tilemaps.Keys.ToList();
                var map = tilemaps[mapData.key];
                map.ClearAllTiles();
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
        }catch(Exception e)
        {
            Debug.Log(e);
        }
        string wp_data = FileHandler.ReadFile(FileHandler.GetPath(selectedMapName + "_waypoints"));
        string[] wp_lines = wp_data.Split("\n");
        foreach(string line in wp_lines)
        {
            if (line != "" && line.Split()[0] != "Spawnpoint:")
            {
                float x = (float)Convert.ToDouble(line.Split()[1]);
                float y = (float)Convert.ToDouble(line.Split()[2]);
                float maxSpeed = (float)Convert.ToDouble(line.Split()[5]);
                Vector3 pos = new Vector3(x, y, 0);
                GameObject wp = Instantiate(waypoint, pos, Quaternion.identity);
                Waypoint wps = wp.GetComponent<Waypoint>();
                wps.maxSpeed = maxSpeed;
            }
            if(line.Split()[0] == "Spawnpoint:")
            {
                AICar.transform.position = new Vector3((float)Convert.ToDouble(line.Split()[1]),(float)Convert.ToDouble(line.Split()[2]),0);
            }
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