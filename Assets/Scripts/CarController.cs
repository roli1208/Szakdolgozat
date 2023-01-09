using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float driftFactor = 0.6f;
    public float accelerationFactor = 5.0f;
    public float turnFactor = 5.2f;
    public float maxSpeed = 10;

    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    //Accest to Unity components
    Rigidbody2D carRigidbody2D;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

     void ApplySteering()
    {
        float minSpeedBeforeTurn = carRigidbody2D.velocity.magnitude / 8;
        minSpeedBeforeTurn = Mathf.Clamp01(minSpeedBeforeTurn);
        //Change angle based on the input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeTurn;

        //Apply the rotation to the car Rigidbody
         carRigidbody2D.MoveRotation(rotationAngle);
    }

    void ApplyEngineForce()
    {
        if (accelerationInput == 0)
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 2.0f, Time.fixedDeltaTime * 2);
        else
            carRigidbody2D.drag = 0;

        if(carRigidbody2D.velocity.magnitude >= maxSpeed)
        {
            return;
        }
        //Generate a vector force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        //Apply the force to the car Rigidbody
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);

    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rigtVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rigtVelocity * driftFactor;
    }

    public void setInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float getLateralVelocity()
    {
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool IsDrifting(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = getLateralVelocity();
        isBraking = false;

        if (accelerationInput < 0 && Vector2.Dot(transform.up, carRigidbody2D.velocity) > 0)
        {
            isBraking = true;
            return true;
        }
        if(Mathf.Abs(getLateralVelocity()) > 1.5f){
            return true;
        }
        return false;
    }
}
