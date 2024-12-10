using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameSettingsSO gameSettings;


    public void LoadMenuScene()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        SceneManager.LoadScene("Main_Menu");
        gameSettings.currentGameState = GameStates.inMainMenu;
    }

    public void LoadLevel1Scene()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        gameSettings.currentGameState = GameStates.inGame;
        SceneManager.LoadScene("Level_1");
      

    }
    public void LoadLevel2Scene()
    {
        gameSettings.previousGameState = gameSettings.currentGameState;
        gameSettings.currentGameState = GameStates.inGame;
        SceneManager.LoadScene("Level_2");


    }

}
