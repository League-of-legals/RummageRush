using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homebase : MonoBehaviour
{

    [SerializeField] EventManagerSO eventManager;


    [SerializeField] float homebaseHealth = 100f;
    public float currentHomebaseHealth;
    [SerializeField] HomebaseHealthBar homebaseHealthBar;


    //Timers
    [SerializeField] float damageTakingTimer;
    [SerializeField] public float damageTakingDelay = 2f;

    //default or reset value
    [SerializeField] public float defaultDamageTakingDelay = 2f;

    //Enemy bookkeping
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] Collider[] colliders;
    [SerializeField] List<Enemy> enemiesInRange;

    [SerializeField] GameSettingsSO gameSettings;

    [SerializeField] List<EnemySlotHomebase> enemySlotsHomebase;


    private void Awake()
    {
        enemySlotsHomebase = new List<EnemySlotHomebase>();
        foreach (Transform t in transform)
            if (t.name == "EnemySlotsHomebase")
                foreach (Transform slot in t)
                    enemySlotsHomebase.Add(new EnemySlotHomebase(slot));
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHomebaseHealth = homebaseHealth;
        damageTakingDelay = defaultDamageTakingDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameSettings.currentGameState == GameStates.inGame)
        {

            ScanForEnemies();
            homebaseHealthBar.UpdateHomebaseHealthBar(currentHomebaseHealth, homebaseHealth);

            foreach (Enemy enemy in enemiesInRange)
            {
                damageTakingTimer += Time.deltaTime;
                if (damageTakingTimer >= damageTakingDelay)
                {
                    damageTakingTimer = 0;
                    TakeDamage(enemy.enemyDamage);
                }
            }
        }

    }


    private void ScanForEnemies()
    {
        colliders = Physics.OverlapBox(transform.position, transform.localScale/50, transform.rotation, enemyLayers);

        enemiesInRange.Clear();

        foreach (Collider collider in colliders)
        {
            enemiesInRange.Add(collider.GetComponent<Enemy>());
        }

    }

    public void TakeDamage(float enemyDamage)
    {
        if (enemiesInRange.Count > 0)
        {
           currentHomebaseHealth -= enemyDamage;

            if(currentHomebaseHealth <= 0)
            {
                Debug.Log($"Health = 0. Game over");
                eventManager.GameOver();
            }

        }
    }

    public bool GetEnemySlotHomebase(Enemy enemy, out Transform transform)
    {
        // This method allows the tower to tell an
        // attacking enemy where to go and stand

        // Is the enemy already occupying a slot?
        foreach (EnemySlotHomebase slot in enemySlotsHomebase)
        {
            if (slot.enemy == enemy)
            {
                transform = slot.transform;
                return true;
            }
        }

        // If not, is there an emptly slot available?
        for (int i = 0; i < enemySlotsHomebase.Count; i++)
        {
            if (enemySlotsHomebase[i].enemy == null)
            {
                Debug.Log($"Assigning a new homebase slot for enemy {enemy.name}");
                enemySlotsHomebase[i].SetEnemy(enemy);
                transform = enemySlotsHomebase[i].transform;
                return true;
            }
        }

        // No slots available
        transform = null;
        return false;
    }

    [Serializable]
    class EnemySlotHomebase
    {

        public Transform transform;
        public Enemy enemy;

        public EnemySlotHomebase(Transform transform)
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
