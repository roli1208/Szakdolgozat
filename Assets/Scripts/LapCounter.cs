using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LapCounter : MonoBehaviour
{

    int passedCheckPointNum = 0;
    float timeFromLastCheckPoint = 0;

    int passedCheckPointCount = 0;

    public event Action<LapCounter> onPassCheckPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            CheckPoint checkPoint = collision.GetComponent<CheckPoint>();

            if(passedCheckPointNum + 1 == checkPoint.checkPointNum)
            {
                passedCheckPointNum = checkPoint.checkPointNum;

                passedCheckPointCount++;

                timeFromLastCheckPoint = Time.time;

                onPassCheckPoint?.Invoke(this);
            }
        }
    }
}
