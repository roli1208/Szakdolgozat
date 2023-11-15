using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarAIHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> waypoints;
    [SerializeField] GameObject currentWaypoint;
    Vector3 targetPosition;
    CarController carController;
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
        inputVector.y = applyThrottleOrBrake(inputVector.x);

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

    void followWaypoints()
    {
        if (waypoints.Count == 0)
            getWaypoints();
        currentWaypoint = waypoints[0];
        targetPosition = currentWaypoint.transform.position;
        distanceFromWaypoint = (targetPosition - transform.position).magnitude;
        if (distanceFromWaypoint <= 0.4)
        {
            waypoints.Remove(currentWaypoint);
            currentWaypoint = waypoints[0];
        }
    }

    float applyThrottleOrBrake(float inputX)
    {
        float actualThrottle = 1.0f;
        return actualThrottle;
        Waypoint firstWps = waypoints[1].GetComponent<Waypoint>();
        Waypoint secondWps = waypoints[2].GetComponent<Waypoint>();
        Waypoint thirdWps = waypoints[3].GetComponent<Waypoint>();
        actualThrottle = Mathf.Abs((firstWps.throttlePercent * 1.0f) + (secondWps.throttlePercent * 0.75f) + (thirdWps.throttlePercent * 0.5f));
    }
}
