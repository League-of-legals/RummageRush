using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    // === ENEMY PATH : OWNER OF THE PATH ===

    // list of waypoints
    [SerializeField] public List<Transform> waypoints;

    private void Awake()
    {
        // initialize the list
        waypoints = new List<Transform>();

        // populate the list with waypoints in our transform
        foreach(Transform waypoint in transform)
        {
            // add the new waypoints to the list one by one 
            waypoints.Add(waypoint);
        }
    }

    public Transform GetWaypoint(int incomingIndex)
    {
        return waypoints[incomingIndex];
    }

    public int GetNumberOfWaypoints()
    {
        return waypoints.Count;
    }

}
