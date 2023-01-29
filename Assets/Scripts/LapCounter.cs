using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class LapCounter : MonoBehaviour
{
    public CheckPoint[] checkPoints;
    public int passedCheckPointNum = 0;
    public TMP_Text lapCounterText;
    public TMP_Text winnerName;
    public GameObject raceCompletePanel;

    public int lapNum = 1;
    int completedLapNum = 0;

    bool isRaceComplete = false;

    public event Action<LapCounter> onPassCheckPoint;

    private void Start()
    {
        checkPoints = GameObject.FindObjectsOfType<CheckPoint>();
        foreach (CheckPoint cp in checkPoints)
        {
            if(cp.checkPointNum != 1)
            {
                cp.gameObject.SetActive(false);
            }
        }
    }
    private void FixedUpdate()
    {
        if(!isRaceComplete)
        lapCounterText.text = $"Laps: {completedLapNum+1} / {lapNum}";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (isRaceComplete)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;
            raceCompletePanel.SetActive(true);
            winnerName.text = $"Winner is: {user.DisplayName}";
            Time.timeScale = 0f;
            return;
        }
        if (collision.CompareTag("CheckPoint"))
        {
            CheckPoint checkPoint = collision.GetComponent<CheckPoint>();

            if(passedCheckPointNum + 1 == checkPoint.checkPointNum)
            {
                passedCheckPointNum = checkPoint.checkPointNum;

                if (checkPoint.isFinish)
                {
                    passedCheckPointNum = 0;
                    completedLapNum++;
                    if(completedLapNum >= lapNum)
                    {
                        isRaceComplete = true;
                    }
                }

                onPassCheckPoint?.Invoke(this);
            }
        }
        foreach (CheckPoint cp in checkPoints)
        {
            if (cp.checkPointNum == passedCheckPointNum + 1)
            {
                cp.gameObject.SetActive(true);
            }
            else
                cp.gameObject.SetActive(false);
        }
    }
}
