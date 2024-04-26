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
    private int currentTargetWaypoint = 0;


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
        if (gameSettings.currentGameState == GameStates.inGame)
        {
            animator.SetTrigger("walk");
            switch (enemyState)
            { 
            case EnemyState.Stopped:
                {

                    ScanForTower();


                    if (targetedTower != null && targetedTower.towerIsActive)
                        enemyState = EnemyState.MovingToAttack;
                    break;
                }

            case EnemyState.Traveling:
                {
                    if (currentTargetWaypoint >= enemyPath.GetNumberOfWaypoints())
                    {
                        enemyState = EnemyState.Stopped;

                        break;
                    }

                    transform.LookAt(enemyPath.GetWaypoint(currentTargetWaypoint));

                    transform.position = Vector3.MoveTowards(
                    transform.position,                                    // where from
                    enemyPath.GetWaypoint(currentTargetWaypoint).position,               // where to
                    speed * Time.deltaTime                                 // how fast
                    );

                    // are we close enough to the destination?
                    if (Vector3.Distance(transform.position,
                    enemyPath.GetWaypoint(currentTargetWaypoint).position) < 0.1f)
                    {
                        // increment the current target waypoint
                        currentTargetWaypoint++;
                    }

                    ScanForTower();

                    if (targetedTower != null && targetedTower.towerIsActive)
                        enemyState = EnemyState.MovingToAttack;
                    break;
                }


            case EnemyState.MovingToAttack:
                    {
                        if (targetedTower != null && targetedTower.towerIsActive)
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
                            transform.position,                                    // where from
                            enemySlotAroundTower.transform.position,               // where to
                            speed * Time.deltaTime                                 // how fast
                            );

                            if (Vector3.Distance(transform.position,
                                enemySlotAroundTower.transform.position) < 0.1f)
                            {
                                
                                enemyState = EnemyState.Attacking; break;
                            }
                            break;
                        }

                        // Nothing to do, go back to travelling
                        enemyState = EnemyState.Traveling; break;
                }

            case EnemyState.Attacking:
                {
                    if (targetedTower != null && targetedTower.towerIsActive)
                    {
                            animator.ResetTrigger("walk");
                            animator.SetTrigger("punch");
                        foreach (Collider tower in colliders)
                        {
                            damageDealingTimer += Time.deltaTime;
                            if (damageDealingTimer >= damageDealingDelay)
                            {
                                damageDealingTimer = 0;
                                targetedTower.TakeDamage(enemyDamage);
                            }
                        }
                        break;
                    }

                    // Nothing to do, go back to travelling
                    enemyState = EnemyState.Traveling; break;
                }
            }

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

    IEnumerator DeathSequence()
    {
        animator.ResetTrigger("walk");
        animator.ResetTrigger("punch");
        animator.SetTrigger("die");

        yield return new WaitForSeconds(3.5f);
        gameSettings.money += rewardCost;
        eventManager.EnemyDestroyed();
        Destroy(this.gameObject);
    }

}