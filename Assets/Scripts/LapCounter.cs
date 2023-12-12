using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LapCounter : MonoBehaviour
{
    public int passedCheckPointNum = 0;
    public TMP_Text lapCounterText;
    public TMP_Text winnerName;
    public GameObject raceCompletePanel;
    [SerializeField] public List<TilemapData> mapdata;
    [SerializeField] public List<TileInfo> checkpoints;
    [SerializeField] public Tilemap checkpoint;
    [SerializeField] public TileBase checkpointTile;

    public int lapNum = 1;
    int completedLapNum = 0;

    bool isRaceComplete = false;

    public event Action<LapCounter> onPassCheckPoint;

    public void addTiles()
    {
        Debug.Log("Called!!!");
        foreach (var tile in checkpoints)
        {
            checkpoint.SetTile(tile.position, checkpointTile);
        }
    }

    private void Start()
    {
        mapdata = SaveHandler.GetInstance().getMap();
        checkpoint = GameObject.Find("Checkpoint").GetComponent<Tilemap>();
        foreach (var item in mapdata)
        {
           if(item.key == "Checkpoint")
            {
                foreach (var tile in item.tiles)
                {
                    checkpoints.Add(tile);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isRaceComplete)
            lapCounterText.text = $"Laps: {completedLapNum+1} / {lapNum}";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("COLLISION: " + collision.name);
        
        if (isRaceComplete)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;
            raceCompletePanel.SetActive(true);
            winnerName.text = $"The winner is:\n {user.DisplayName}";
            Time.timeScale = 0f;
            return;
        }
        if (collision.name == "Checkpoint")
        {
            int currentID = CarController.currentID;
            Debug.Log("currentID :" + currentID);
            Debug.Log("Count: " + checkpoints.Count);
            if (currentID >= (checkpoints.Count - 2))
            {
                completedLapNum++;
                addTiles();
            }
        }
    }
}
