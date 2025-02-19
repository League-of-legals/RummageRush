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

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1f;
    }

    public void LoadnextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
