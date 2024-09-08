using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] HUDmanager hudManager;
    [SerializeField] EventManagerSO eventManager;
    [SerializeField] RandomEvent randomEvent;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] TowerSpawner towerSpawner;


    [SerializeField] Homebase homebase;

    [SerializeField] float randomEventTimer;
    [SerializeField] public float randomEventDuration = 8f;

    //[SerializeField] Tower towerDefault;
    //[SerializeField] Tower towerFast;
    //[SerializeField] Tower towerHeavy;

    //[SerializeField] Enemy enemyDefault;
    //[SerializeField] Enemy enemyFast;
    //[SerializeField] Enemy enemyHeavy;
    //public Enemy enemy;

    //[SerializeField] List<Enemy> enemiesInTheScene;
    //[SerializeField] List<Tower> towersInTheScene;

    [SerializeField] TMP_Text randomItemDescription;

    //[SerializeField] Animator animator;

    private void Awake()
    {
        gameSettings.currentGameState = GameStates.inGame;
        Time.timeScale = 1f;

        

        if (SceneManager.GetActiveScene().name == "Level_1")
        {   
            if (gameSettings.previousGameState == GameStates.inMainMenu)
            {
                hudManager.DisplayTutorial();
                gameSettings.currentGameState = GameStates.inTutorial;
                Time.timeScale = 0f;
            }
            //gameSettings.currentGameState = GameStates.inGame;
            //Time.timeScale = 1f;
            gameSettings.ResetMoney();
            gameSettings.ResetDamageDealt();
            gameSettings.enemiesSpawned = 7;
            gameSettings.enemiesDestroyed = 0;

        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level_2")
        {
            gameSettings.ResetMoney();
            gameSettings.ResetDamageDealt();
            gameSettings.enemiesSpawned = 7;
            gameSettings.enemiesDestroyed = 0;
        }
    }

    /* private void OnEnable()
    {
        eventManager.onEnemyDestroyed += UpdateKillCount;
    }

    private void OnDisable()
    {
        eventManager.onEnemyDestroyed -= UpdateKillCount;
    } */


    private void Update()
    {
               

        if (gameSettings.enemiesDestroyed == gameSettings.enemiesSpawned)
        {
            eventManager.Win();
        }


        if (gameSettings.currentGameState== GameStates.inTutorial)
        {
            if (Input.anyKeyDown)
            {
                gameSettings.previousGameState = gameSettings.currentGameState;
                hudManager.HideTutorial();
                eventManager.ResumeGame();
                gameSettings.currentGameState = GameStates.inGame;
                Time.timeScale = 1f;
            }
        }

        else if(gameSettings.currentGameState == GameStates.inGame)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                eventManager.PauseGame();
                gameSettings.currentGameState = GameStates.paused;
                Time.timeScale = 0f;
            }
        }

        else if (gameSettings.currentGameState == GameStates.paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                eventManager.ResumeGame();
                gameSettings.currentGameState = GameStates.inGame;
                Time.timeScale = 1f;
            }
        }

        else if (gameSettings.currentGameState == GameStates.showingControls)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                hudManager.HideControls();
                eventManager.ResumeGame();
                gameSettings.currentGameState = GameStates.inGame;
                Time.timeScale = 1f;
            }
        }

        if (gameSettings.currentGameState == GameStates.inGame)
        {
            if (gameSettings.damageDealt == 60f)
            {
                randomEvent.item = null;
                randomEvent.itemName = null;
                gameSettings.previousGameState = GameStates.inGame;
                gameSettings.currentGameState = GameStates.inRandomEvent;
                Time.timeScale = 0f;
                eventManager.RandomEvent();
                gameSettings.damageDealt = 0f;
            }
        }

        else if (gameSettings.currentGameState == GameStates.inRandomEvent)
        {
            if (Input.anyKeyDown)
            {
                randomEventTimer = 0f;
                gameSettings.previousGameState = gameSettings.currentGameState;
                hudManager.HideRandomEventScreen();
                gameSettings.currentGameState = GameStates.inGame;
                Time.timeScale = 1f;
            }
        }

        if (gameSettings.previousGameState == GameStates.inRandomEvent && 
            gameSettings.currentGameState == GameStates.inGame && 
            randomEvent.item != null &&
            randomEvent.itemName != null )

        {
            randomEventTimer += Time.deltaTime;

            if (randomEventTimer < randomEventDuration)
            {
                //FindEnemiesInTheScene();
                //FindTowersInTheScene();
                gameSettings.damageDealt = 0;

                if (randomEvent.itemName == "Banana peel")
                {
                 
                    foreach (Enemy enemy in enemySpawner.enemiesInScene)
                    enemy.agent.speed = 1f;

                    //foreach (Enemy enemy in enemiesInTheScene)
                    //enemy.speed = 1f;
                }

                else if (randomEvent.itemName == "Cardboard box")
                {
                    randomItemDescription.text = 
                        $"Homebase is immune to damage for {randomEventDuration} seconds!";
                    homebase.damageTakingDelay = 10f;
                }

                else if (randomEvent.itemName == "Crushed can")
                {
                    randomItemDescription.text =
                        $"Raccons shoot faster for {randomEventDuration} seconds!";
                    foreach (Tower tower in towerSpawner.towersInScene)
                    tower.firingDelay = 0.3f;
                }

                else if (randomEvent.itemName == "Lavalamp")
                {
                    randomItemDescription.text =
                        $"Raccons are distracted and can't defend for {randomEventDuration} seconds!";
                    foreach (Tower tower in towerSpawner.towersInScene)
                    {
                        if (tower != null)
                        {
                            tower.towerScanningTimer = 0;
                            tower.animator.ResetTrigger("Throw");
                            tower.animator.SetTrigger("Idle");
                            eventManager.RandomEventTowers();
                            tower.firingDelay = 10f;
                        }
                    }
                }

                else if (randomEvent.itemName == "Moldy brownie")
                {
                    randomItemDescription.text =
                        $"Raccons are sick and can't defend for {randomEventDuration} seconds!";
                    foreach (Tower tower in towerSpawner.towersInScene)
                    {
                        if (tower != null)
                        {
                            tower.towerScanningTimer = 0;
                            tower.animator.ResetTrigger("Throw");
                            tower.animator.SetTrigger("Idle");
                            eventManager.RandomEventTowers();
                            tower.firingDelay = 10f;
                        }
                    }
                }

                else if (randomEvent.itemName == "Plastic knife")
                {
                    randomItemDescription.text =
                        $"For {randomEventDuration} seconds enemies' maximum health is reduced!";

                    foreach (Enemy enemy in enemySpawner.enemiesInScene)
                    {
                        enemy.maxHealth = 5f;
                    }                  

                }

            }

            else if (randomEventTimer >= randomEventDuration)
            {
                randomEvent.item = null;
                randomEvent.itemName = null;
                //FindEnemiesInTheScene();
                eventManager.RandomEventStop();

                foreach (Enemy enemy in enemySpawner.enemiesInScene)
                    enemy.agent.speed = enemy.defaultSpeed;

                //foreach (Enemy enemy in enemiesInTheScene)
                   // enemy.speed = enemy.defaultSpeed;

                foreach (Enemy enemy in enemySpawner.enemiesInScene)
                    enemy.maxHealth = enemy.defaultHealth;

                //foreach (Enemy enemy in enemiesInTheScene)
                  //  enemy.maxHealth = enemy.defaultHealth;

                foreach (Tower tower in towerSpawner.towersInScene)
                {
                    if (tower != null)
                    {
                        tower.towerScanningTimer += Time.deltaTime;
                        tower.firingDelay = tower.defaultFiringDelay;

                    }
                }
                    

                homebase.damageTakingDelay = homebase.defaultDamageTakingDelay;
            }


        }

                    
         
    }

    private void UpdateKillCount()
    {
        gameSettings.enemiesDestroyed += 1;
    }



    /*private void FindEnemiesInTheScene()
    {
        enemiesInTheScene = UnityEngine.Object.FindObjectsOfType<Enemy>();

    }

    private void FindTowersInTheScene()
    {
        towersInTheScene = UnityEngine.Object.FindObjectsOfType<Tower>();
    }
    */


}

    

