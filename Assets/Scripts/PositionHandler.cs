using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{

    public List<LapCounter> carLapList = new List<LapCounter>();

    // Start is called before the first frame update
    void Start()
    {
        LapCounter[] carLapArray = FindObjectsOfType<LapCounter>();

        carLapList = carLapArray.ToList<LapCounter>();

        Debug.Log(carLapList);
        
        foreach (LapCounter lapCounter in carLapList)
        {
            Debug.Log(lapCounter);
            lapCounter.onPassCheckPoint += onPassCheckpoint;
        }
    }
    void onPassCheckpoint(LapCounter lapCounter)
    {
        Debug.Log($"Car: {lapCounter.gameObject.name} passed a checkpoint: {lapCounter.passedCheckPointNum}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
