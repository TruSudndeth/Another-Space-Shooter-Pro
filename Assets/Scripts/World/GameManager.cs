using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int _sceneIndex = 0; //Todo: delete this line
    private void Start()
    {
        InputManager.Instance.Exit.started += _ => ExitGame();
        UI._loadScene += LoadScene;
        UI._resetLevel += RestartCurrentLevel;
        if(Instance)
        { 
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnDestroy()
    {
        InputManager.Instance.Exit.started -= _ => ExitGame();
        UI._loadScene -= LoadScene;
        UI._resetLevel -= RestartCurrentLevel;
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
