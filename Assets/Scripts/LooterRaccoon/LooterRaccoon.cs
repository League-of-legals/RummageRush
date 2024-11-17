using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LooterRaccoon : MonoBehaviour
{
    [SerializeField] public LooterRaccoonPath path;
    private int currentTargetWaypoint;
    public float speed = 2.5f;
    public float speedDefault = 2.5f;
    public float speedHauling = 1.5f;
    private int numberOfWaypoints;
    private bool hasLoot;

    //[SerializeField] public float defaultPrice = 20f;
    //[SerializeField] public float currentPrice = 20f;

    [SerializeField] public float resourceGain = 50f;
    [SerializeField] public float resources;    
    
    [SerializeField] public Camera cameraMain;

    [SerializeField] public GameSettingsSO gameSettings;

    [SerializeField] ResourceLoadIndicatorUI resourceLoadIndicatorUI;

    [SerializeField] public GameObject resourcePool;
    [SerializeField] public GameObject homebase;
    [SerializeField] public HUDmanager hudManager;
    [SerializeField] public LooterRaccoonSpawner looterRaccoonSpawner;

    [SerializeField] List<EnemySlotLooter> enemySlotsLooter;

    // health
    [SerializeField] public float raccoonHealth = 30f;
    public float currentRaccoonHealth;

    private void Awake()
    {
        enemySlotsLooter = new List<EnemySlotLooter>();
        foreach (Transform t in transform)
            if (t.name == "EnemySlotsLooter")
                foreach (Transform slot in t)
                    enemySlotsLooter.Add(new EnemySlotLooter(slot));
    }


    private void Start()
    {
        numberOfWaypoints = path.GetNumberOfWaypoints();
        hasLoot = false;
      
        resourceLoadIndicatorUI = GetComponentInChildren<ResourceLoadIndicatorUI>();

        resourceLoadIndicatorUI.UpdateLoadText(" ");

        currentRaccoonHealth = raccoonHealth;
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
        resourceLoadIndicatorUI.UpdateLoadText($"{resources}");
        yield return new WaitForSeconds(1f);
        resources += resourceGain;
        resourceLoadIndicatorUI.UpdateLoadText($"{resources}");
        yield return new WaitForSeconds(1f);
        resources += resourceGain;
        resourceLoadIndicatorUI.UpdateLoadText($"{resources}");
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
        gameSettings.money += resources;
        resourceLoadIndicatorUI.UpdateLoadText($" ");
        hudManager.UpdateMoneyText();
        resources = 0;
        hasLoot = false;
        speed = speedDefault;
    }


    public void TakeDamage(float enemyDamage)
    {
        currentRaccoonHealth -= enemyDamage;

        if (currentRaccoonHealth <= 0)
        {
            
            looterRaccoonSpawner.LootersInScene.Remove(this);
            if (looterRaccoonSpawner.LootersInScene.Count == 0f)
            { gameSettings.looterCurrentPrice = 0; }
            else
            {
                gameSettings.looterCurrentPrice = gameSettings.looterDefaultPrice +
                    (looterRaccoonSpawner.LootersInScene.Count * gameSettings.looterPriceModifier);
            }

            Destroy(this.gameObject);
        }
    }

    public bool GetEnemySlotLooter(Enemy enemy, out Transform transform)
    {
        // This method allows the tower to tell an
        // attacking enemy where to go and stand

        // Is the enemy already occupying a slot?
        foreach (EnemySlotLooter slot in enemySlotsLooter)
        {
            if (slot.enemy == enemy)
            {
                transform = slot.transform;
                return true;
            }
        }

        // If not, is there an emptly slot available?
        for (int i = 0; i < enemySlotsLooter.Count; i++)
        {
            if (enemySlotsLooter[i].enemy == null)
            {
                enemySlotsLooter[i].SetEnemy(enemy);
                transform = enemySlotsLooter[i].transform;
                return true;
            }
        }

        // No slots available
        transform = null;
        return false;
    }


    [Serializable]
    class EnemySlotLooter
    {

        public Transform transform;
        public Enemy enemy;

        public EnemySlotLooter(Transform transform)
        {
            this.transform = transform;
            this.enemy = null;
        }

        public void SetEnemy(Enemy enemy)
        {
            this.enemy = enemy;
        }
    }

}
