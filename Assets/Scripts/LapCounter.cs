using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LapCounter : MonoBehaviour
{
    public CheckPoint[] checkPoints;
    public int passedCheckPointNum = 0;
    float timeFromLastCheckPoint = 0;

    int passedCheckPointCount = 0;

    int lapNum = 1;
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (isRaceComplete)
        {
            Debug.Log("Race is complete");
            return;
        }
        if (collision.CompareTag("CheckPoint"))
        {
            CheckPoint checkPoint = collision.GetComponent<CheckPoint>();

            if(passedCheckPointNum + 1 == checkPoint.checkPointNum)
            {
                passedCheckPointNum = checkPoint.checkPointNum;

                passedCheckPointCount++;

                timeFromLastCheckPoint = Time.time;

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
