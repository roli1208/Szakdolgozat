using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    LapCounter lapCounter;

    static public int currentID = 0;

    Tilemap checkpoint;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        rotationAngle = carRigidbody2D.transform.localEulerAngles.z;
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpoint = GameObject.Find("Checkpoint").GetComponent<Tilemap>();
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        lapCounter = GetComponent<LapCounter>();
        if (collision.name == "Checkpoint")
        {
            Tilemap map = collision.GetComponent<Tilemap>();
            List<TilemapData> mapdata = SaveHandler.GetInstance().getMap();
            foreach (var item in mapdata)
            {
                if (item.key == "Checkpoint")
                {
                    foreach (var i in item.tiles)
                    {
                        if ((i.position == map.WorldToCell(transform.position) && (currentID + 1) == i.id) || (i.id == 0 && i.position == map.WorldToCell(transform.position)))
                        {
                            currentID = i.id;
                            Vector3Int removePos = map.WorldToCell(transform.position);
                            map.SetTile(removePos, null);
                        }
                    }
                }
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    List<TilemapData> map = SaveHandler.GetInstance().getMap();
    //    if (collision == track)
    //    {
    //        actualMaxSpeed = maxSpeed;
    //        actualDriftFactor = driftFactor;
    //    }
    //    Tilemap tilemap = collision.GetComponent<Tilemap>();


    //    if (collision.name == "Checkpoint")
    //    {
    //        if (tilemap != null)
    //        {

                
    //            Debug.Log("Collided Tile Position: " + collision.ClosestPoint(transform.position));
    //        }
    //        Debug.Log("CP");
    //        Vector3Int cellPosition = new Vector3Int((int)collision.ClosestPoint(transform.position).x, (int)collision.ClosestPoint(transform.position).y, 0);
    //        Debug.Log(cellPosition);
    //        tilemap.SetTile(cellPosition, null);
    //    }
    //    foreach(var item in map)
    //    {
    //        if(item.key == "Checkpoint")
    //        {
    //            foreach (var i in item.tiles)
    //            {
    //                if (i.position == Vector3Int.RoundToInt(transform.position)){
    //                    Debug.Log(i.id);
    //                }
    //            }
    //        }
    //    }
    //    Debug.Log("Collision with: " + tilemap.GetTile(Vector3Int.RoundToInt(transform.position)));
    //}
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
