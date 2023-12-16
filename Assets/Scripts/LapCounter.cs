using Firebase.Auth;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LapCounter : MonoBehaviour
{
    public int passedCheckPointNum = 0;
    [SerializeField] public TMP_Text lapCounterText;
    [SerializeField] public TMP_Text winnerName;
    [SerializeField] public GameObject raceCompletePanel;
    [SerializeField] public List<TilemapData> mapdata;
    [SerializeField] public List<TileInfo> checkpoints;
    [SerializeField] public Tilemap checkpoint;
    [SerializeField] public TileBase checkpointTile;
    bool aiWon = false;

    static public int lapNum = 3;
    int completedLapNum = 0;
    int aiCompletedLapNum;

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
        aiCompletedLapNum = 0;
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
        if (aiCompletedLapNum > lapNum)
        {
            isRaceComplete = true;
            aiWon = true;
        }
        if (completedLapNum > lapNum)
        {
            isRaceComplete = true;
        }
        if (isRaceComplete)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;
            raceCompletePanel.SetActive(true);
            if (!aiWon)
                winnerName.text = $"The winner is:\n {user.DisplayName}";
            else
                winnerName.text = $"The winner is:\n AICar";

            Time.timeScale = 0f;
            return;
        }
        if (collision.name == "Finish" && gameObject.name == "AICar")
        {
            aiCompletedLapNum++;
        }
        if (collision.name == "Finish" && CarController.currentID == (checkpoints.Count - 1))
        {
            addTiles();
            completedLapNum++;
        }
    }
}
