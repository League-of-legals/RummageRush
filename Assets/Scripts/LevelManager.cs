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

    [SerializeField] Tower towerDefault;
    [SerializeField] Tower towerFast;
    [SerializeField] Tower towerHeavy;

    [SerializeField] Enemy enemyDefault;
    [SerializeField] Enemy enemyFast;
    [SerializeField] Enemy enemyHeavy;

    [SerializeField] Enemy[] enemiesInTheScene;

    [SerializeField] TMP_Text randomItemDescription;

    private void Awake()
    {
        gameSettings.currentGameState = GameStates.inGame;
        Time.timeScale = 1f;

        if (gameSettings.previousGameState == GameStates.inMainMenu)
        {
            
            hudManager.DisplayTutorial();
            gameSettings.currentGameState = GameStates.inTutorial;
            Time.timeScale = 0f;
        }

        if (SceneManager.GetActiveScene().name == "Level_1")
        {
            //gameSettings.currentGameState = GameStates.inGame;
            //Time.timeScale = 1f;
            gameSettings.ResetMoney();
            gameSettings.ResetDamageDealt();
            gameSettings.enemiesSpawned = 7;
            gameSettings.enemiesDestroyed = 0;

        }
        
    }

    private void Update()
    {
        FindEnemiesInTheScene();

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

                if (randomEvent.itemName == "Banana peel")
                {
                 
                    foreach (Enemy enemy in enemySpawner.enemies)
                    enemy.speed = 1f;

                    foreach (Enemy enemy in enemiesInTheScene)
                    enemy.speed = 1f;
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
                    foreach (Tower tower in towerSpawner.towers)
                    tower.firingDelay = 0.3f;
                }

                else if (randomEvent.itemName == "Lavalamp")
                {
                    randomItemDescription.text =
                        $"Raccons are distracted and can't defend for {randomEventDuration} seconds!";
                    foreach (Tower tower in towerSpawner.towers)
                        tower.firingDelay = 10f;
                }

                else if (randomEvent.itemName == "Moldy brownie")
                {
                    randomItemDescription.text =
                        $"Raccons are sick and can't defend for {randomEventDuration} seconds!";
                    foreach (Tower tower in towerSpawner.towers)
                        tower.firingDelay = 10f;
                }

                else if (randomEvent.itemName == "Plastic knife")
                {
                    randomItemDescription.text =
                        $"For {randomEventDuration} seconds enemies' maximum health is reduced!";

                    foreach (Enemy enemy in enemiesInTheScene)
                    {
                        enemy.maxHealth = 5f;
                        enemy.currentHealth = 5f;
                    }
                        
                    

                }

            }

            else if (randomEventTimer >= randomEventDuration)
            {
                randomEvent.item = null;
                randomEvent.itemName = null;
                FindEnemiesInTheScene();

                foreach (Enemy enemy in enemySpawner.enemies)
                    enemy.speed = enemy.defaultSpeed;

                foreach (Enemy enemy in enemiesInTheScene)
                    enemy.speed = enemy.defaultSpeed;

                foreach (Enemy enemy in enemySpawner.enemies)
                    enemy.maxHealth = enemy.defaultHealth;

                foreach (Tower tower in towerSpawner.towers)
                    tower.firingDelay = tower.defaultFiringDelay;

                homebase.damageTakingDelay = homebase.defaultDamageTakingDelay;
            }


        }
                    
         
    }


    private void FindEnemiesInTheScene()
    {
        enemiesInTheScene = UnityEngine.Object.FindObjectsOfType<Enemy>();

    }


}

    

