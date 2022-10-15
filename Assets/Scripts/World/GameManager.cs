using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //create an enum for all levels in the Build
    private int sceneIndex = 0;
    void OnEnable()
    {
        UI.ResetLevel += RestartCurrentLevel;
    }
    void OnDisable()
    {
        UI.ResetLevel -= RestartCurrentLevel;
    }
    void RestartCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
