using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooterRaccoonPath
    : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;

    private void Awake()
    {
        waypoints = new List<Transform>();

        foreach(Transform waypoint in transform)
        { waypoints.Add(waypoint); }
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
