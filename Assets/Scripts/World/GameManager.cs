using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineInternal;
using static Types;

public class GameManager : DontDestroyHelper<GameManager>
{
    //Todo: Bug, abuse next wave difficulty when picking up health and lowering your difficulty (Rare)
    public delegate void SetDifficulty(float difficulty);
    public static event SetDifficulty MasterDifficulty;
    public static event SetDifficulty NewDifficulty;
    // Add a difficulty curve
    // player should select their difficulty settings
    // apply a difficulty curve from -1 to 1 where selection is 0
    // brodcast difficulty curve to all interactables
    // Fire rate,Enemy follow speed, enemie moves and speed, enemy fire alert, collectable spawns, Ammo bank, wave size.
    // Enemy size mini boss (Drones are fine)
    // UX Might have to zoom player out for better reaction time.
    //
    /// <summary>
    /// Need an average difficulty settings for each (easy, medium, and hard) from play testers.
    /// easy is 1 (0-2) medium is 2 (1-3) hard is 3 (2-4)
    /// </summary>
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float _easyRange = 1.0f;
    [SerializeField]
    [Range(1.0f, 3.0f)]
    private float _mediumRange = 2.0f;
    [SerializeField]
    [Range(2.0f, 4.0f)]
    private float _hardRange = 3.0f;
    // 
    // Calculate the speed of lasers or the probability of shooting a laser early
    //      listen to enemies lasers count
    //Eq.   Lasers spawned/ player hit
    //
    // Calculate spawn rate difficulty
    //      listen to Enemies killed
    //      listen to Enemies spawned
    //      listen to Enemies collided with player
    //Eq.   Enemies killed/ player hit
    //Eq.   Enemies spawned/ player hit
    // 
    // Mother ship Difficulty curve
    //      how many mother ships were killed
    //      increase the number of hits required
    //
    //
    [SerializeField]
    private DifficultiesEnums.Modes _setMainDifficulty = DifficultiesEnums.Modes.easy;
    //Give access to any instance type that will control the main difficulty
    public DifficultiesEnums.Modes SetMainDifficulty { get { return _setMainDifficulty; } set { _setMainDifficulty = value; } }
    private float _currentDifficulty = 1;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _difficultyRateOfChange = 0.1f;
    private float _currentDifficultyAdjustment = 0;


    private float _enemySpawnRate = 0; // unsure if this will be used.
    private int _enemiesKilled = 0;
    private int _enemiesSpawned = 0;
    [SerializeField]
    private int _waveSize = 0;
    private float _playerHit = 0;
    private float _outOfAmmoBeforeReload = 0;
    //private

    private void CalculateDificulty()
    {
        // Wave Spawn Rate logic
        if (_enemiesSpawned >= _waveSize)
        {
            //increase difficulty by Scale
            Debug.Log("difficulty is too easy");
            Debug.Log("Difficulty " + (float)_enemiesSpawned / _waveSize + " remainder " + (float)_enemiesSpawned % _waveSize);
            _currentDifficultyAdjustment += _difficultyRateOfChange;
            NewDifficulty?.Invoke(_currentDifficulty + AdjustDifficultyRate(_currentDifficultyAdjustment));
        }
        else
        {
            Debug.Log("difficulty is too hard");
            _currentDifficultyAdjustment -= _difficultyRateOfChange;
            NewDifficulty?.Invoke(_currentDifficulty + AdjustDifficultyRate(_currentDifficultyAdjustment));
        }
        _enemiesSpawned = 0;
        // increase Ammo rate drop and reduced Damaged ammo probability.
    }
    private float AdjustDifficultyRate(float value)
    {
        value = Mathf.Clamp(value, -1, 1);
        return value;
    }
    private void Start()
    {
        StartGameAsteroids.GameStarted += GameStarted;
        UIManager.ExitApplication += ExitGame;
        UIManager.Load_Scene += LoadScene;
        UIManager.ResetLevel += RestartCurrentLevel;

        StartGameAsteroids.SetDifficulty += () => { _currentDifficulty++; };

        // All Feedback for setting A difficulty curve
        EnemySpawnManager.SpawnFeedbackCount += (x) => { _enemiesSpawned++; _waveSize = x; };
        EnemySpawnManager.EnemiesKilledFeedBack += (x) => { _enemiesKilled++; _waveSize = x; };
        EnemySpawnManager.NewWaveEvent += () => CalculateDificulty();
        // Player Feedback
        Player.PlayerOutOfAmmoFeedback += () => { _outOfAmmoBeforeReload++; };
        Player.PlayerDamagedFeedback += () => CalculateDificulty();

        //Set master difficulty
    }
    private void GameStarted()
    {
        _currentDifficulty = (float) _setMainDifficulty;
        MasterDifficulty?.Invoke(_currentDifficulty);
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            StartGameAsteroids.SetDifficulty -= () => { _currentDifficulty++; };

            UIManager.Load_Scene -= LoadScene;
            UIManager.ResetLevel -= RestartCurrentLevel;
            UIManager.ExitApplication -= ExitGame;

            EnemySpawnManager.NewWaveEvent -= () => CalculateDificulty();
            EnemySpawnManager.SpawnFeedbackCount -= (x) => { _enemiesSpawned++; _waveSize = x; };
            EnemySpawnManager.EnemiesKilledFeedBack -= (x) => { _enemiesKilled++; _waveSize = x; };
            // Player Feedback
            Player.PlayerOutOfAmmoFeedback -= () => { _outOfAmmoBeforeReload++; };
            Player.PlayerDamagedFeedback -= () => CalculateDificulty();
        }
    }
    void LoadScene(Types.GameState gameState)
    {
        //Delete: Debug.Log("Load Scene: " + gameState);
        Debug.Log("Load scene " + gameState);
        SceneManager.LoadScene((int)gameState);
    }
    void RestartCurrentLevel()
    {
        //Delete: Debug.Log
        //Debug: GamemanagerDesabled this should behave similar to LoadScene(stage level one)
        Debug.Log("Current Scene reloaded");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResetVariables();
    }
    private void ResetVariables()
    {
        _currentDifficulty = 1.0f;
        _currentDifficultyAdjustment = 0.0f;
        MasterDifficulty?.Invoke(_currentDifficulty);
    }
    private void ExitGame()
    {
        //Todo: change to exit to main menu once in main menu Exit application
        Application.Quit();
    }
}
