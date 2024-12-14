using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDmanager : MonoBehaviour
{
    [SerializeField] TMP_Text moneyTextObject;

    [SerializeField] GameSettingsSO gameSettings;

    [SerializeField] Tower towerDefault;
    [SerializeField] Tower towerFast;
    [SerializeField] Tower towerHeavy;
    private Enemy enemy;

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject tutorialScreen;
    [SerializeField] TutorialLevel1 tutorialPanel;
    public int numberOfTutorialScreens;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject controlsScreen;
    [SerializeField] public GameObject randomEventScreen;
    [SerializeField] GameObject winScreen;

    [SerializeField] EventManagerSO eventManager;

    [SerializeField] RandomEvent randomEvent;

    [SerializeField] Button buttonControls;
    [SerializeField] Button buttonPause;
    [SerializeField] Button buttonCloseControls;
    [SerializeField] Button buttonMute;

    [SerializeField] Button buttonResume;

    [SerializeField] TMP_Text towerDefaultCost;
    [SerializeField] TMP_Text towerFastCost;
    [SerializeField] TMP_Text towerHeavyCost;

    [SerializeField] AudioScript audioScript;

    private void Start()
    {
        UpdateMoneyText();

        if (pauseScreen.activeInHierarchy == true)
        {
            pauseScreen.SetActive(false);
        }

        towerDefaultCost.text = $"x{towerDefault.towerCost}";
        towerFastCost.text = $"x{towerFast.towerCost}";
        towerHeavyCost.text = $"x{towerHeavy.towerCost}";
        audioScript.audioSource.Play();
        Debug.Log($"{tutorialPanel.screens.Length}");
        tutorialPanel.currentTutorialScreen = 1;


    }

    private void Update()
    {
        if (winScreen.activeInHierarchy == true)
        {
            randomEventScreen.SetActive(false);
        }

        if (gameOverScreen.activeInHierarchy == true)
        {
            randomEventScreen.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            if (!audioScript.audioSource.mute)
            {
                audioScript.audioSource.mute = true;
            }
            else if (audioScript.audioSource.mute)
            {
                audioScript.audioSource.mute = false;
            }
        }

        if(tutorialScreen.activeInHierarchy == true)
        {
            if(Input.GetMouseButtonUp(0))
            {
                tutorialPanel.CycleThroughTutorialScreens();
                if (tutorialPanel.currentTutorialScreen >= tutorialPanel.screens.Length)
                {
                    gameSettings.previousGameState = gameSettings.currentGameState;
                    HideTutorial();
                    eventManager.ResumeGame();
                    gameSettings.currentGameState = GameStates.inGame;
                    Time.timeScale = 1f;
                }
            }
        }
        
    }


    private void OnEnable()
    {
        buttonPause.onClick.AddListener(() => {
            eventManager.PauseGame();
            gameSettings.currentGameState = GameStates.paused;
            Time.timeScale = 0f;
        });

        buttonControls.onClick.AddListener(() => {
            ShowControls();
            gameSettings.currentGameState = GameStates.showingControls;
            Time.timeScale = 0f;
        });

        buttonCloseControls.onClick.AddListener(() =>
        {
            HideControls();
            gameSettings.currentGameState = GameStates.inGame;
            Time.timeScale = 1f;
        });

        buttonResume.onClick.AddListener(() =>
        {
            HidePauseScreen();
            gameSettings.currentGameState = GameStates.inGame;
            Time.timeScale = 1f;
        });

        buttonMute.onClick.AddListener(() =>
        {
            if (!audioScript.audioSource.mute)
            {
                audioScript.audioSource.mute = true;
            }
            else if (audioScript.audioSource.mute)
            {
                audioScript.audioSource.mute = false;
            }
        });

        //buttonExit.onClick.AddListener(() =>
        //{
        //    gameSettings.previousGameState = gameSettings.currentGameState;
        //    SceneManager.LoadScene("Main_Menu");
        //    gameSettings.currentGameState = GameStates.inMainMenu;
        //});

        //buttonRestart.onClick.AddListener(() => {
        //    gameSettings.previousGameState = gameSettings.currentGameState;
        //    SceneManager.LoadScene("Level_1");
        //    gameSettings.currentGameState = GameStates.inGame;
        //});



        eventManager.onGameOver += DisplayGameOverScreen;
        eventManager.onWin += DisplayWinScreen;
        eventManager.onPauseGame += DisplayPauseScreen;
        eventManager.onResumeGame += HidePauseScreen;
        eventManager.onEnemyDestroyed += UpdateMoneyText;
        eventManager.onRandomEvent += ShowRandomEventScreen;




    }

    private void OnDisable()
    {
        eventManager.onGameOver -= DisplayGameOverScreen;
        eventManager.onWin -= DisplayWinScreen;
        eventManager.onPauseGame -= DisplayPauseScreen;
        eventManager.onResumeGame -= HidePauseScreen;
        eventManager.onEnemyDestroyed -= UpdateMoneyText;
        eventManager.onRandomEvent -= ShowRandomEventScreen;


        //buttonExit.onClick.RemoveListener(() =>
        //{
        //    gameSettings.previousGameState = gameSettings.currentGameState;
        //    SceneManager.LoadScene("Main_Menu");
        //    gameSettings.currentGameState = GameStates.inMainMenu;
        //});

        //buttonRestart.onClick.RemoveListener(() => {
        //    gameSettings.previousGameState = gameSettings.currentGameState;
        //    SceneManager.LoadScene("Level_1");
        //    gameSettings.currentGameState = GameStates.inGame;
        //});

    }

    public void UpdateMoneyText()
    {
        moneyTextObject.text = $"x {gameSettings.money}";
    }

    //public void SubtractTowerCost()
    //{
    //    gameSettings.money -= tower.towerCost;
    //    moneyTextObject.text = $"x {gameSettings.money}";
    //}

    public void DisplayGameOverScreen()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        gameOverScreen.SetActive(true);
        gameSettings.currentGameState = GameStates.gameOver;
    }

    public void DisplayWinScreen()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        winScreen.SetActive(true);
        gameSettings.currentGameState = GameStates.win;
        Time.timeScale = 0f;
    }

    public void DisplayTutorial()
    {
        gameSettings.previousGameState = GameStates.inMainMenu;
        tutorialScreen.SetActive(true);
        gameSettings.currentGameState = GameStates.inTutorial;
    }

    public void HideTutorial()
    {
        tutorialScreen.SetActive(false);

    }

    public void DisplayPauseScreen()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        pauseScreen.SetActive(true);
        gameSettings.currentGameState = GameStates.paused;
    }

    public void HidePauseScreen()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        pauseScreen.SetActive(false);
        gameSettings.currentGameState = GameStates.inGame;
    }

    public void ShowControls()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        controlsScreen.SetActive(true);
    }

    public void HideControls()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        controlsScreen.SetActive(false);
    }

    public void ShowRandomEventScreen()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        randomEventScreen.SetActive(true);
        randomEvent.Randomize();
        gameSettings.currentGameState = GameStates.inRandomEvent;
    }

    public void HideRandomEventScreen()
    {
        randomEventScreen.SetActive(false);
    }
}


 