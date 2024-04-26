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
        colliders = Physics.OverlapBox(transform.position, transform.localScale/2, transform.rotation, enemyLayers);

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

}
