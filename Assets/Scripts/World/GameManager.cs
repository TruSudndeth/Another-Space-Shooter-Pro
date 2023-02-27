using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : DontDestroyHelper<GameManager>
{
    private void Start()
    {
        InputManager.Instance.Exit.started += _ => ExitGame();
        UIManager.Load_Scene += LoadScene;
        UIManager.ResetLevel += RestartCurrentLevel;
    }
    private void OnDestroy()
    {
        if(Instance == this)
        {
            InputManager.Instance.Exit.started -= _ => ExitGame();
            UIManager.Load_Scene -= LoadScene;
            UIManager.ResetLevel -= RestartCurrentLevel;
        }
    }
    void LoadScene(Types.GameState gameState)
    {
        //Delete: Debug.Log("Load Scene: " + gameState);
        Debug.Log("Load scene " + gameState);
        SceneManager.LoadScene((int) gameState);
    }
    void RestartCurrentLevel()
    {
        //Delete: Debug.Log
        //Debug: GamemanagerDesabled this should behave similar to LoadScene(stage level one)
        Debug.Log("Current Scene reloaded");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void ExitGame()
    {
        //Todo: change to exit to main menu once in main menu Exit application
        Application.Quit();
    }
}
