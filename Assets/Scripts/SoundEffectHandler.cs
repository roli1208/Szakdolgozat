using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectHandler : MonoBehaviour
{
    public AudioSource tireScreeachingAS;
    public AudioSource engineAS;
    public AudioSource hitAS;

    float enginePitch;
    float tireScreechPitch;

    CarController carController;

    void Awake()
    {
        carController= GetComponent<CarController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EngineSFX();
        TireScreechSFX();
    }

    private void TireScreechSFX()
    {
        if (carController.IsDrifting(out float lateralVelocity, out bool isBraking))
        {
            if (isBraking)
            {
                tireScreeachingAS.volume = Mathf.Lerp(tireScreeachingAS.volume, 1.0f, Time.deltaTime * 10);
                tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);

            }
            else
            {
                tireScreeachingAS.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
        else
            tireScreeachingAS.volume = Mathf.Lerp(tireScreeachingAS.volume, 0, Time.deltaTime * 10);
    }

    void EngineSFX()
    {
        float velocityMagnitude = carController.getVelocityMagnitude();

        float engineVolume = velocityMagnitude * 0.05f;

        engineVolume = Mathf.Clamp(engineVolume, 0.2f, 1.0f);

        engineAS.volume = Mathf.Lerp(engineAS.volume, engineVolume, Time.deltaTime * 20);

        enginePitch = velocityMagnitude * 0.2f;
        enginePitch = Mathf.Clamp(enginePitch, 0.4f, 1.9f);
        engineAS.pitch = Mathf.Lerp(engineAS.pitch, enginePitch, Time.deltaTime * 1.3f);
    }
}
