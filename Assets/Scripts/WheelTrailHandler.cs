using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailHandler : MonoBehaviour
{
    CarController carController;
    TrailRenderer trailRender;

    void Awake()
    {
        carController = GetComponentInParent<CarController>();

        trailRender = GetComponent<TrailRenderer>();

        trailRender.emitting = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carController.IsDrifting(out float lateralVelocity, out bool isBraking))
            trailRender.emitting = true;
        else 
            trailRender.emitting = false;
    }
}
