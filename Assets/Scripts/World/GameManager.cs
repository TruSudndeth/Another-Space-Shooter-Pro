using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : DontDestroyHelper<GameManager>
{
    private void Start()
    {
        InputManager.Instance.Exit.started += _ => ExitGame();
        UI.Load_Scene += LoadScene;
        UI.ResetLevel += RestartCurrentLevel;
    }
    private void OnDestroy()
    {
        InputManager.Instance.Exit.started -= _ => ExitGame();
        UI.Load_Scene -= LoadScene;
        UI.ResetLevel -= RestartCurrentLevel;
    }
    void LoadScene(Types.GameState gameState)
    {
        SceneManager.LoadScene((int) gameState);
    }
    void RestartCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void ExitGame()
    {
        //Todo: change to exit to main menu once in main menu Exit application
        Application.Quit();
    }
}
