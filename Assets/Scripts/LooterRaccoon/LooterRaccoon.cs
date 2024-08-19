using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LooterRaccoon : MonoBehaviour
{
    [SerializeField] LooterRaccoonPath path;
    private int currentTargetWaypoint;
    private int speed = 2;
    private int numberOfWaypoints;
    private bool hasLoot;

    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] public float resourceGain = 80f;

    private void Start()
    {
        numberOfWaypoints = path.GetNumberOfWaypoints();
        hasLoot = false;
        
    }
    private void Update()
    {
        transform.LookAt(path.GetWaypoint(currentTargetWaypoint));

        transform.position = Vector3.MoveTowards(transform.position, path.GetWaypoint(currentTargetWaypoint).position,
           speed * Time.deltaTime);

        if (!hasLoot)
        {

            if (Vector3.Distance(transform.position, path.GetWaypoint(currentTargetWaypoint).position) < 0.1f
                && currentTargetWaypoint < numberOfWaypoints - 1f)
            {
                currentTargetWaypoint++;
            }
        }

        if (Vector3.Distance(transform.position, path.GetWaypoint(currentTargetWaypoint).position) < 0.01f
                && currentTargetWaypoint == numberOfWaypoints -1f && !hasLoot)
        {
            StartCoroutine(DrawResource());
        }

        if (hasLoot)
        {
            if (Vector3.Distance(transform.position, path.GetWaypoint(currentTargetWaypoint).position) < 0.1f
            && currentTargetWaypoint > 0f)
            {
                currentTargetWaypoint--;
            }
        }


    }
 IEnumerator DrawResource()
    {
        Debug.Log($"drawing resource");
        yield return new WaitForSeconds(5f);
        transform.position = Vector3.MoveTowards(transform.position, path.GetWaypoint(6).position,
           speed * Time.deltaTime);
        hasLoot = true;
        gameSettings.money += resourceGain;
    }



    public void SetLooterPath(LooterRaccoonPath incomingPath)
    { path = incomingPath; }

   

}
