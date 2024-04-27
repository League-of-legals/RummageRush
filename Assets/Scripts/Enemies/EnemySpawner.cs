using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /* ENEMY SPAWNER
     * Spawns a wave of enemies to a chosen path 
     */

    // == PATHS ==
    //[SerializeField] EnemyPath enemyPathA;
    //[SerializeField] EnemyPath enemyPathB;

    [SerializeField] List<EnemyPath> paths;
    [SerializeField] public List<Enemy> enemies;

    // == ENEMIES ==
    [SerializeField] Enemy enemyDefault;
    [SerializeField] Enemy enemyFast;
    [SerializeField] Enemy enemyHeavy;

    // == SETTINGS ==
    //[SerializeField] int wave01Enemies = 8;

    // == DELAYS ==
    //[SerializeField] float randomDelayMin = .5f;
    //[SerializeField] float randomDelayMax = 4.0f;

    [SerializeField] GameSettingsSO gameSettings;

    private void SpawnEnemy(Enemy enemyToSpawn, EnemyPath chosenPath)
    {
        // which enemy to spawn, where, what rotation, which path
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity).SetEnemyPath(chosenPath);
        enemies.Add(enemyToSpawn);

    }
    private void Awake()
    {

    }

    private void Start()
    {
        StartCoroutine(Wave01()); 
    }


    IEnumerator Wave01()
    {

        //for(int i = 0; i<wave01Enemies; i++)
        //{
        //    SpawnEnemy(enemyDefault, paths[(int)Random.Range(0, paths.Count)]);
        //    yield return new WaitForSeconds(Random.Range(randomDelayMin, randomDelayMax));
        //}

        yield return new WaitForSeconds(4);                                     // wait for 2 seconds
        SpawnEnemy(enemyDefault, paths[(int)Random.Range(0, paths.Count)]);    // then spawn

        yield return new WaitForSeconds(2);     
        SpawnEnemy(enemyDefault, paths[(int)Random.Range(0, paths.Count)]);    

        yield return new WaitForSeconds(2);     
        SpawnEnemy(enemyFast, paths[(int)Random.Range(0, paths.Count)]);      

        yield return new WaitForSeconds(0.5f); 
        SpawnEnemy(enemyDefault, paths[(int)Random.Range(0, paths.Count)]);   

        yield return new WaitForSeconds(2);     
        SpawnEnemy(enemyDefault, paths[(int)Random.Range(0, paths.Count)]);   

        yield return new WaitForSeconds(4);     
        SpawnEnemy(enemyHeavy, paths[(int)Random.Range(0, paths.Count)]);    

        yield return new WaitForSeconds(2);     
        SpawnEnemy(enemyHeavy, paths[(int)Random.Range(0, paths.Count)]);     

    }

}
