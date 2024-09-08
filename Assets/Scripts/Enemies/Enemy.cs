using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine;
using UnityEngine.AI;

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

    // slots
    private Transform enemySlotAroundTower;
    private Transform enemySlotAroundHomebase;
    private Transform enemySlotAroundLooter;

    //default values or reset values
    [Header("Default values:")]
    [SerializeField] public float defaultSpeed;

    [SerializeField] public float defaultHealth;


    [Header("Values:")]
    //speed
    [SerializeField] public float speed;

    //range
    float range = 3.0f;

    //damage
    [SerializeField] public float enemyDamage = 5f;

    //drop reward
    [SerializeField] public float rewardCost = 30f;

    //damage timers
    [Header("Damage timer:")]
    [SerializeField] float damageDealingTimer;
    [SerializeField] float damageDealingDelay = 2f;

    [Header("Path:")]
    // get reference to the road
    [SerializeField] EnemyPath enemyPath;
   [Header("Enemy targeting:")]
    // hit target (where to hit the enemy)
    [SerializeField] Transform hitTarget;

    [Header("Health:")]
    // health
    [SerializeField] public float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] HealthBar healthBar;

    [Header("Target bookkeeping:")]

    [SerializeField] Collider[] colliders;
    //tower bookkeping
    [SerializeField] LayerMask towerLayers;
    [SerializeField] List<Tower> towersInRange;
    [SerializeField] Tower targetedTower;
    [SerializeField] TowerHealthBar towerHealthBar;

    //looter bookkeping
    [SerializeField] LayerMask looterLayers;
    [SerializeField] List<LooterRaccoon> looterRaccoonsInRange;
    [SerializeField] LooterRaccoon targetedRaccoon;

    //navigation
    public NavMeshAgent agent;

    [Header("Other references:")]
    //other references
    [SerializeField] public EnemySpawner enemySpawner;
    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] EventManagerSO eventManager;
    [SerializeField] Animator animator;

    [SerializeField] Homebase homebase;

    //[SerializeField] public TowerSpawner towerSpawner;

    // remember where to go
    private int currentTargetWaypoint;
    private float currentTime;

    private void Awake()
    {
        //set max health and values to default
        maxHealth = defaultHealth;
        currentHealth = maxHealth;
        speed = defaultSpeed; 
        enemyState = EnemyState.Traveling;
    }

    private void Start()
    {
        // get references
        animator = GetComponentInChildren<Animator>();
        homebase = FindFirstObjectByType<Homebase>();
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);

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

                animator.SetTrigger("stopped");


                ScanForTower();
                if (!targetedTower)
                { ScanForLooter(); }

                if (targetedTower && targetedTower.towerIsActive)
                {
                    enemyState = EnemyState.MovingToAttack;
                }

                if(targetedRaccoon)
                { enemyState = EnemyState.MovingToAttack; }
                break;



            case EnemyState.Traveling:
                animator.ResetTrigger("punch");
                animator.ResetTrigger("stopped");
                animator.SetTrigger("walk");
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

                //transform.LookAt(enemyPath.GetWaypoint(currentTargetWaypoint));

                agent.destination =
                enemyPath.GetWaypoint(currentTargetWaypoint).position;
                agent.speed = defaultSpeed;

                // are we close enough to the destination?
                if (Vector3.Distance(transform.position, enemyPath.GetWaypoint(currentTargetWaypoint).position) < 0.6f
                     && currentTargetWaypoint < enemyPath.waypoints.Count - 1) // we were getting more than 10 waypoints
                {
                    // increment the current target waypoint
                    currentTargetWaypoint++;
                }

                if (Vector3.Distance(transform.position, enemyPath.GetWaypoint(currentTargetWaypoint).position) < 0.6f
                    && currentTargetWaypoint == enemyPath.waypoints.Count - 1 )
                {
                    if (homebase)
                    {
                        enemyState = EnemyState.MovingToAttack; break;

                    }


                }

                ScanForTower(); 
                if (!targetedTower)
                { ScanForLooter(); }

                if (targetedTower && targetedTower.towerIsActive)
                    enemyState = EnemyState.MovingToAttack;

                if (targetedRaccoon)
                { enemyState = EnemyState.MovingToAttack; }

                break;
            
            case EnemyState.MovingToAttack:
                animator.ResetTrigger("punch");
                animator.ResetTrigger("stopped");
                animator.SetTrigger("walk");
                agent.speed = defaultSpeed;

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

                    //transform.LookAt(enemySlotAroundTower.transform.position);
                    //transform.position = Vector3.MoveTowards(
                    //transform.position,                                   // where from
                    //enemySlotAroundTower.transform.position,               // where to
                    //speed * Time.deltaTime                        // how fast
                    //);

                    agent.destination = enemySlotAroundTower.transform.position;

                    if (Vector3.Distance(transform.position,
                        enemySlotAroundTower.transform.position) < 0.6f)
                    {                    
                        enemyState = EnemyState.Attacking;
                    }
                    break;
                }

                if (targetedRaccoon)
                {
                    if (!enemySlotAroundLooter)
                    {
                        if (targetedRaccoon.GetEnemySlotLooter(this, out Transform slotTransform))
                        {
                            enemySlotAroundLooter = slotTransform;
                        }
                        else
                        {
                            enemyState = EnemyState.Traveling; break;
                        }
                    }

                    //transform.LookAt(enemySlotAroundLooter.transform.position);
                    //transform.position = Vector3.MoveTowards(
                    //transform.position,                                   // where from
                    //enemySlotAroundLooter.transform.position,               // where to
                    //speed * Time.deltaTime                        // how fast
                    //);


                    agent.destination = enemySlotAroundLooter.transform.position;

                    if (Vector3.Distance(transform.position,
                        enemySlotAroundLooter.transform.position) < 0.7f)
                    {
                        enemyState = EnemyState.Attacking;
                    }
                    break;

                }

                if (homebase)
                {

                    if (!enemySlotAroundHomebase)
                    {
                        if (homebase.GetEnemySlotHomebase(this, out Transform slotTransform))
                        {
                            enemySlotAroundHomebase = slotTransform;
                        }
                    }

                    agent.destination = enemySlotAroundHomebase.transform.position;

                    if (Vector3.Distance(transform.position,
                    enemySlotAroundHomebase.transform.position) < 1f)
                    {
                        enemyState = EnemyState.Attacking;
                    }
                    break;


                    
                }
                // Nothing to do, go back to traveling
                enemyState = EnemyState.Traveling;
                break;

            case EnemyState.Attacking:
                if (targetedTower && targetedTower.towerIsActive)
                {
                    transform.LookAt(targetedTower.transform);
                    animator.ResetTrigger("walk");
                    animator.ResetTrigger("stopped");
                    animator.SetTrigger("punch");
                    foreach (Collider collider in colliders)
                    {
                        damageDealingTimer += Time.deltaTime;
                        if (damageDealingTimer < damageDealingDelay)
                        {
                            return;
                        }
                        damageDealingTimer = 0;
                        targetedTower.TakeDamage(enemyDamage);
                    }
                    
                }

                if (targetedRaccoon)
                {
                    transform.LookAt(targetedRaccoon.transform);
                    animator.ResetTrigger("walk");
                    animator.ResetTrigger("stopped");
                    animator.SetTrigger("punch");

                    agent.speed = 0;

                    foreach (Collider c in colliders)
                    {
                        damageDealingTimer += Time.deltaTime;
                        if (damageDealingTimer < damageDealingDelay)
                        {
                            return;
                        }
                        damageDealingTimer = 0;
                        targetedRaccoon.TakeDamage(enemyDamage);
                    }

                }

                if (homebase)
                    if (!enemySlotAroundHomebase) 
                    { 
                        enemyState = EnemyState.MovingToAttack;
                    }
                    else if (Vector3.Distance(transform.position,
                    enemySlotAroundHomebase.transform.position) < 1f)
                    {
                        transform.LookAt(homebase.transform);
                        animator.ResetTrigger("walk");
                        animator.ResetTrigger("stopped");
                        animator.SetTrigger("punch");

                        agent.speed = 0;

                        //if (towerSpawner.towersInScene)
                        //{ ScanForTower(); }
                        //else { ScanForLooter(); }

                        ScanForTower();
                        if (!targetedTower)
                        { ScanForLooter(); }

                        if (targetedTower && targetedTower.towerIsActive)
                        enemyState = EnemyState.MovingToAttack;

                        if (targetedRaccoon)
                        { enemyState = EnemyState.MovingToAttack; }

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

    public void ScanForLooter()
    {
        colliders = Physics.OverlapSphere(transform.position, range, looterLayers);

        looterRaccoonsInRange.Clear();

        foreach (Collider c in colliders)
        {
            looterRaccoonsInRange.Add(c.GetComponent<LooterRaccoon>());
        }

        if (looterRaccoonsInRange.Count != 0)
        {
            targetedRaccoon = looterRaccoonsInRange[0];
        }

    }

    IEnumerator DeathSequence()
    {
        animator.ResetTrigger("walk");
        animator.ResetTrigger("punch");
        animator.ResetTrigger("stopped");
        animator.SetTrigger("die");
        enemySpawner.enemiesInScene.Remove(this);
        gameSettings.enemiesDestroyed += 1;
        agent.speed = 0f;
        yield return new WaitForSeconds(3.5f);
        gameSettings.money += rewardCost;
        //eventManager.EnemyDestroyed();
        Destroy(this.gameObject);
    }

}