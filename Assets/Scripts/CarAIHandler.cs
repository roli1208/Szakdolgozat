using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarAIHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> waypoints;
    [SerializeField] GameObject currentWaypoint;
    Vector3 targetPosition;
    [SerializeField] CarController carController;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        carController = GetComponent<CarController>();
    }
    void getWaypoints()
    {
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint").Where(obj => obj.name == "Waypoint(Clone)"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;
        followWaypoints();
        inputVector.x = turnToTarget();
        distanceFromWaypoint = (targetPosition - transform.position).magnitude;
        inputVector.y = applyThrottleOrBrake();


        carController.setInputVector(inputVector);
    }

    float turnToTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        float angleVector = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleVector *= -1;

        float steerAmount = angleVector / 45.0f;

        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }
    [SerializeField] float distanceFromWaypoint;
    [SerializeField] float difference;
    void followWaypoints()
    {
        if (waypoints.Count <= 1)
        {
            getWaypoints();
        }
        currentWaypoint = waypoints[0];
        if (waypoints.Count > 1)
        {
            Vector3 midpoint1 = Vector3.Lerp(waypoints[0].transform.position, waypoints[1].transform.position, 0.5f);

            targetPosition = Vector3.Lerp(transform.position, midpoint1, 1.0f);
        }
        else
        {
            targetPosition = waypoints[0].transform.position;
        }
        distanceFromWaypoint = (targetPosition - transform.position).magnitude;
        Waypoint currWaypoint = currentWaypoint.GetComponent<Waypoint>();
        currWaypoint.distace = distanceFromWaypoint;
        if (!currWaypoint.updated)
        {
            if (currWaypoint.distace > currWaypoint.previousDistance && currWaypoint.distace > 1.55f)
            {
                difference = 2 - Mathf.Log(currWaypoint.distace + 1, 2);
                difference = Mathf.Clamp(difference, 0.8f, 1);
                Debug.Log("DECREASING SPEED: " + difference);
                currWaypoint.maxSpeed *= difference;
            }
            if (currWaypoint.distace != 0)
            {
                currWaypoint.previousDistance = currWaypoint.distace;
            }
            currWaypoint.updated = true;
        }
        if (distanceFromWaypoint <= 0.85 && currentWaypoint == waypoints[0])
        {
            currWaypoint.updated = false;
            waypoints.Remove(currentWaypoint);
            currentWaypoint = waypoints[0];
        }
        Debug.DrawLine(transform.position, targetPosition, Color.red);
    }

    void updateWaypoint(Waypoint waypoint, float distance)
    {
        if (!waypoint.updated)
        {
            waypoint.maxSpeed /= (Mathf.Log(distance, 1.5f) * 1.2f);
            Debug.Log("Waypoint updated: " + waypoint.maxSpeed);
        }
        waypoint.updated = true;
    }

    [SerializeField]float weigthedSpeedLimit;
    [SerializeField]float brakingForce;
    [SerializeField] float accelerationForce;
    [SerializeField] float speedDifference;
    float applyThrottleOrBrake()
    {
        if (waypoints.Count >= 4)
        {
            Debug.Log("IN IF");
            Waypoint firstWps = waypoints[1].GetComponent<Waypoint>();
            Waypoint secondWps = waypoints[2].GetComponent<Waypoint>();
            Waypoint thirdWps = waypoints[3].GetComponent<Waypoint>();
            float currentSpeed = carController.getVelocityMagnitude();

            weigthedSpeedLimit = ((firstWps.maxSpeed * 3f) + (secondWps.maxSpeed * 2f) + (thirdWps.maxSpeed * 1f)) / 6;
            speedDifference = currentSpeed - weigthedSpeedLimit;
            float brakeFactor = 0.1f;
            float throttleFactor = 0.18f;
            if (speedDifference > 0)
            {
                brakingForce = brakeFactor * speedDifference;
                return -brakingForce;
            }
            if (speedDifference < 0)
            {
                accelerationForce = throttleFactor * Mathf.Abs(speedDifference);
                return accelerationForce;
            }
        }
        Debug.Log("OUT IF");
        return carController.accelerationInput;
    }
}
