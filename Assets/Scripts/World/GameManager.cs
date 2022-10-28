using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //create an enum for all levels in the Build
    private int _sceneIndex = 0;
    void OnEnable()
    {
        UI._loadScene += LoadScene;
        UI._resetLevel += RestartCurrentLevel;
    }
    void OnDisable()
    {
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
}
