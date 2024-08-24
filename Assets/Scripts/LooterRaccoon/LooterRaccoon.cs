using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LooterRaccoon : MonoBehaviour
{
    [SerializeField] public LooterRaccoonPath path;
    private int currentTargetWaypoint;
    public float speed = 2;
    public float speedDefault = 2;
    public float speedHauling = 1.5f;
    private int numberOfWaypoints;
    private bool hasLoot;
    [SerializeField] public float resourceGain = 50f;
    [SerializeField] public float resources;    
    
    [SerializeField] public Camera cameraMain;

    [SerializeField] GameSettingsSO gameSettings;

    [SerializeField] ResourceLoadIndicatorUI resourceLoadIndicatorUI;

    [SerializeField] GameObject resourcePool;
    [SerializeField] GameObject homebase;



    private void Start()
    {
        numberOfWaypoints = path.GetNumberOfWaypoints();
        hasLoot = false;
        //cameraMain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        resourceLoadIndicatorUI = GetComponentInChildren<ResourceLoadIndicatorUI>();
        resourcePool = GameObject.FindGameObjectWithTag("ResourcePool");
        homebase = GameObject.FindGameObjectWithTag("Homebase");
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

        if (hasLoot)
        {
            if (Vector3.Distance(transform.position, path.GetWaypoint(currentTargetWaypoint).position) < 0.1f
            && currentTargetWaypoint > 0f)
            {
                currentTargetWaypoint--;
            }
        }



    }

    private void OnTriggerEnter(Collider other)
    {            
        Debug.Log($"something touched me!");

        if (other.gameObject == resourcePool && !hasLoot)
        {
            StartCoroutine(DrawResources());
        }

        if (other.gameObject == homebase && hasLoot)
        {
            StartCoroutine(UnloadResources());
        }


    }


    public void SetLooterPath(LooterRaccoonPath incomingPath)
    { path = incomingPath; }

   IEnumerator DrawResources()
    {
        yield return new WaitForSeconds(0.5f);
        speed = 0f;
        yield return new WaitForSeconds(1f);
        resources += resourceGain;
        yield return new WaitForSeconds(1f);
        resources += resourceGain;
        yield return new WaitForSeconds(1f);
        resources += resourceGain;
        yield return new WaitForSeconds(1f);
        speed = speedHauling;
        hasLoot = true;
    }

    IEnumerator UnloadResources()
    {
        Debug.Log($"Unloading...");
        yield return new WaitForSeconds(1f);
        speed = 0f;
        yield return new WaitForSeconds(2f);
        resources = 0;
        hasLoot = false;
        speed = speedDefault;
    }

}
