using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float driftFactor = 0.75f;
    public float accelerationFactor = 3.2f;
    public float turnFactor = 5.2f;

    public float actualMaxSpeed = 7f;
    public float actualDriftFactor = 0.75f;

    public float maxSpeed = 7f;
    public float changedMaxSpeed = 4f;
    public float reverseMaxSpeed = 3f;
    public float changedDriftFactor = 0.95f;

    public float accelerationInput = 0;
    public float steeringInput = 0;

    public float rotationAngle = 0;

    public PolygonCollider2D track;

    //Accest to Unity components
    Rigidbody2D carRigidbody2D;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        rotationAngle = carRigidbody2D.transform.localEulerAngles.z;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != collision.CompareTag("CheckPoint"))
        {
            actualMaxSpeed = changedMaxSpeed;
            actualDriftFactor = changedDriftFactor;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == track)
        {
            actualMaxSpeed = maxSpeed;
            actualDriftFactor = driftFactor;
        }
    }
    void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }
    public float minSpeedBeforeTurn;
     void ApplySteering()
    {
        minSpeedBeforeTurn = carRigidbody2D.velocity.magnitude / 9;
        minSpeedBeforeTurn = Mathf.Clamp01(minSpeedBeforeTurn);


        //Change angle based on the input
        if(accelerationInput >= 0)
        {
            rotationAngle -= steeringInput * turnFactor * minSpeedBeforeTurn;
        }else
            if(accelerationInput < 0 && Vector2.Dot(transform.up, carRigidbody2D.velocity) < 0)
                rotationAngle += steeringInput * turnFactor * minSpeedBeforeTurn;
        else
            rotationAngle -= steeringInput * turnFactor * minSpeedBeforeTurn;


        //Apply the rotation to the car Rigidbody
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void ApplyEngineForce()
    {
        if (accelerationInput == 0)
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 1.9f, Time.fixedDeltaTime * 2);
        else
            if (accelerationInput < 0)
                carRigidbody2D.drag = 1.2f;
                 else
                     carRigidbody2D.drag = 0;

        if(accelerationInput > 0 && carRigidbody2D.velocity.magnitude >= actualMaxSpeed)
        {
            return;
        }
        if (accelerationInput < 0 && carRigidbody2D.velocity.magnitude >= reverseMaxSpeed)
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

        carRigidbody2D.velocity = forwardVelocity + rigtVelocity * actualDriftFactor;
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
    public float getVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
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
        if(Mathf.Abs(getLateralVelocity()) > 1.2f){
            return true;
        }
        return false;
    }

}
