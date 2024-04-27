using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private enum EnemyState
    {
        Stopped,
        Traveling,
        MovingToAttack,
        Attacking
    }

    private EnemyState enemyState;

    private Transform enemySlotAroundTower;

    //default values or reset values
    [SerializeField] public float defaultSpeed;

    [SerializeField] public float defaultHealth;



    //speed
    [SerializeField] public float speed;

    //range
    float range = 2.0f;

    //damage
    [SerializeField] public float enemyDamage = 5f;

    //drop reward
    [SerializeField] public float rewardCost = 30f;
 
    //damage timers
    [SerializeField] float damageDealingTimer;
    [SerializeField] float damageDealingDelay = 2f;

    // get reference to the road
    [SerializeField] EnemyPath enemyPath;

    // hit target (where to hit the enemy)
    [SerializeField] Transform hitTarget;

    // health
    [SerializeField] public float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] HealthBar healthBar;

    //tower bookkeping
    [SerializeField] LayerMask towerLayers;
    [SerializeField] Collider[] colliders;
    [SerializeField] List<Tower> towersInRange;
    [SerializeField] Tower targetedTower;
    [SerializeField] TowerHealthBar towerHealthBar;

    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] EventManagerSO eventManager;
    [SerializeField] Animator animator;


    // remember where to go
    private int currentTargetWaypoint;
    private float currentTime;

    private void Awake()
    {
        //set max health
        maxHealth = defaultHealth;
        currentHealth = maxHealth;
        speed = defaultSpeed;
        enemyState = EnemyState.Traveling;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        // If we aren't in game, then update fails, and we don't do anything
        if (gameSettings.currentGameState != GameStates.inGame)
        {
            return;
        }
        
        // Set animation state
        animator.ResetTrigger("punch");
        animator.ResetTrigger("stopped");
        animator.SetTrigger("walk");

        // switch on the current state of the enemy
        switch (enemyState)
        {
            default:
            case EnemyState.Stopped:
                animator.ResetTrigger("walk");
                animator.ResetTrigger("punch");
                
                // Regardless of actually attacking, if stopped, it will punch
                animator.SetTrigger("stopped");

                ScanForTower();

                if (targetedTower && targetedTower.towerIsActive)
                {
                    enemyState = EnemyState.MovingToAttack;
                }
                break;

            case EnemyState.Traveling:
                // if (currentTargetWaypoint >= enemyPath.GetNumberOfWaypoints())
                // {
                    /* TODO: Fix waypoint issue with enemy, documented steps to reproduce below
                     - Let the enemy go to our base
                     - spawn tower behind them
                     - they attack tower
                     - stand still looking away from tower
                     
                     Potential issues:
                     - No line of sight to base
                     - Way points may no longer be targeted
                     - Way point count may need to be tracked
                     - Looks like EnemyPathB doesn't have 6-9 filled out, this might be the culprit.
                    
                     Bug found:
                     - More than 10 way points reported back
                    */
                //     Debug.Log($"We stopped for no reason. Waypoint: {currentTargetWaypoint}");
                //     enemyState = EnemyState.Stopped;
                //     break;
                // }

                transform.LookAt(enemyPath.GetWaypoint(currentTargetWaypoint));

                transform.position = Vector3.MoveTowards(
                transform.position,                                    // where from
                enemyPath.GetWaypoint(currentTargetWaypoint).position,  // where to
                speed * Time.deltaTime                         // how fast
                );

                // are we close enough to the destination?
                if (Vector3.Distance(transform.position, enemyPath.GetWaypoint(currentTargetWaypoint).position) < 0.1f
                     && currentTargetWaypoint < 10) // we were getting more than 10 waypoints
                {
                    // increment the current target waypoint
                    currentTargetWaypoint++;
                }

                ScanForTower();

                if (targetedTower && targetedTower.towerIsActive)
                    enemyState = EnemyState.MovingToAttack;
                break;
            
            case EnemyState.MovingToAttack:
                if (targetedTower && targetedTower.towerIsActive)
                {
                    if (!enemySlotAroundTower)
                    {
                        if (targetedTower.GetEnemySlot(this, out Transform slotTransform))
                        {
                            enemySlotAroundTower = slotTransform;
                        }
                        else
                        {
                            // Cannot find a free slot around the tower
                            // Getting out of here
                            enemyState = EnemyState.Traveling; break;
                        }
                    }
                    
                    transform.LookAt(enemySlotAroundTower.transform.position);
                    transform.position = Vector3.MoveTowards(
                    transform.position,                                   // where from
                    enemySlotAroundTower.transform.position,               // where to
                    speed * Time.deltaTime                        // how fast
                    );

                    if (Vector3.Distance(transform.position,
                        enemySlotAroundTower.transform.position) < 0.1f)
                    {
                        
                        enemyState = EnemyState.Attacking;
                    }
                    break;
                }

                // Nothing to do, go back to travelling
                enemyState = EnemyState.Traveling;
                break;

            case EnemyState.Attacking:
                if (targetedTower && targetedTower.towerIsActive)
                {
                    animator.ResetTrigger("walk");
                    animator.ResetTrigger("stopped");
                    animator.SetTrigger("punch");
                    foreach (Collider tower in colliders)
                    {
                        damageDealingTimer += Time.deltaTime;
                        if (damageDealingTimer < damageDealingDelay)
                        {
                            return;
                        }
                        damageDealingTimer = 0;
                        targetedTower.TakeDamage(enemyDamage);
                    }
                    break;
                }

                // Nothing to do, go back to travelling
                enemyState = EnemyState.Traveling;
                break;
        }
    }

    // let the enemy know which path to follow
    public void SetEnemyPath(EnemyPath incomingPath)
    {
        enemyPath = incomingPath;
    }

    public void InflictDamage(float incomingDamage)
    {
        Debug.Log("Got Hit");
        currentHealth -= incomingDamage;
        gameSettings.damageDealt += incomingDamage;

        // update the healthbar
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        // gg wp
        if (currentHealth <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    public Transform getHitTarget()
    {
        return hitTarget;
    }

    public void ScanForTower()
    {
        colliders = Physics.OverlapSphere(transform.position, range, towerLayers);

        towersInRange.Clear();

        foreach (Collider collider in colliders)
        {
            towersInRange.Add(collider.GetComponent<Tower>());
        }

        if (towersInRange.Count != 0)
        {
            targetedTower = towersInRange[0];
        }


    }

    IEnumerator DeathSequence()
    {
        animator.ResetTrigger("walk");
        animator.ResetTrigger("punch");
        animator.ResetTrigger("stopped");
        animator.SetTrigger("die");

        yield return new WaitForSeconds(3.5f);
        gameSettings.money += rewardCost;
        eventManager.EnemyDestroyed();
        Destroy(this.gameObject);
    }

}