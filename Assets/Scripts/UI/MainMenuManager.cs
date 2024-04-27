using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameSettingsSO gameSettings;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject controlsScreen;


    private void Update()
    {
        if (creditsScreen.activeInHierarchy || controlsScreen.activeInHierarchy)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                creditsScreen.SetActive(false);
                controlsScreen.SetActive(false);
            }
        }
    }



    public void LoadMenuScene()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        SceneManager.LoadScene("Main_Menu");
        gameSettings.currentGameState = GameStates.inMainMenu;
    }

    public void LoadLevelScene()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        gameSettings.currentGameState = GameStates.inGame;
        SceneManager.LoadScene("Level_1");
      
    }

}
