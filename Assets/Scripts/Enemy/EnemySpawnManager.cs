using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public delegate void NewWave();
    public static event NewWave NewWaveEvent;

    [SerializeField] private List<Transform> _enemyAsset;
    private List<Transform> _enemies;
    private int _enemyCount = 0;
    [Space]
    private float _boundsOffset = 0;
    private readonly float _cameraAspecRatio = 1.7777778f;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private int _maxPool = 10;
    private int _maxPoolTemp = 0;
    //Delete: Timmer might delete variable timmer && _iterateEnemy
    //[SerializeField] private float _spawnRate = 0.5f; 
    //private int _iterateEnemy = 0;
    private bool _isPoolMaxed = false;
    private bool _gameOver = false;
    private bool _gameStarted = false;
    [Space]
    private bool _beatEnemySpawner = false;
    private int _difficulty = 1;
    [Space]
    [SerializeField] private float _spawnDelay = 5.0f; //Temp: Timmer might delete variable Timmer
    private float _canSpawnTime = 0.0f;
    [Space]
    [SerializeField] private int _waveSize = 3;
    private int _enemiesKilled = 0;
    [Space]
    private int _enemyDroneCount = 0;
    private int _enemyDroneKilled = 0;
    private int _enemyMiniBossCount = 0;
    private int _enemyMiniBossKilled = 0;
    private int _proEnemyCount = 0;
    private int _proEnemyKilled = 0;
    [SerializeField]
    private bool _isWaveComplete = false;

    private void Awake()
    {
        //_waveIsPaused = false;
        _maxPool = _enemyAsset.Count * 10; //Note: EnemySpawnManager Enemy count hard coded to 10*Count
        _maxPoolTemp = _maxPool;
        _enemies = new(_maxPool);
        _enemyCount = _enemyAsset.Count;
        PopulatePool();
    }
    
    private void PopulatePool()
    {
        for (int i = 0; i < _enemyAsset.Count; i++)
        {
            for (int j = 0; j < _maxPool / _enemyAsset.Count; j++)
            {
                _enemies.Add(Instantiate(_enemyAsset[i], new Vector3(0, 0, 0), Quaternion.identity, transform));
                _enemies[^1].gameObject.SetActive(false);
                _isPoolMaxed = true;
            }
        }
    }

    void Start()
    {
        BossFightManager.ContinueWaves += () => SpawnWaveState(false);
        BossFightManager.PauseWaves += () => SpawnWaveState(true);

        EnemyCollisons.EnemyPointsEvent += EnemiesKilled;
        BombEplode.BombExplosionEvent += () => BPMPauseSpawn();
        StartGameAsteroids.SetDifficulty += () => GameDificulty();
        BackGroundMusic_Events.BGM_Events += () => { _beatEnemySpawner = true; };
        StartGameAsteroids.GameStarted += GameStarted;
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;

        Player.Game_Over += PlayerIsDead;
        SpawnRandomEnemiesWave();
    }
    private void SpawnWaveState(bool isWavesPaused = false)
    {
        _gameStarted = !isWavesPaused;
        _isWaveComplete = isWavesPaused;
    }
    public void ContinueWaves(bool isWavesPaused)
    {
        _isWaveComplete = isWavesPaused;
    }
    private void BPMPauseSpawn()
    {
        _canSpawnTime = Time.time;
    }

    private void GameDificulty()
    {
        _difficulty++;
        _maxPoolTemp = _maxPool * _difficulty;
    }

    private void GameStarted()
    {
        _maxPool = _maxPoolTemp;
        _gameStarted = true;
    }

    void FixedUpdate()
    {
        if (_gameOver || !_gameStarted) return;
        //Note: _isWavesPaused could cause a glitch with current enemy count or point system.
        //Temp: Timmer might not have to spawn enemies with time 
        //Temp: Timmer if (_canSpawn + _spawnRate > Time.time) return;
        //Note: Spawn manager was stopped during game play for a brif moment
        if (_beatEnemySpawner && _spawnDelay + _canSpawnTime < Time.time && !_isWaveComplete)
        {
            //return all active enemies int the list in hierarchy
            int activeEnemies = _enemies.FindAll(x => x.gameObject.activeSelf).Count;
            if (activeEnemies >= _waveSize - _enemiesKilled) return;
            SpawnSystem();
            _beatEnemySpawner = false;
            if (_difficulty <= 1) return;
            if (_difficulty > 1 && Random.Range(0, 100) < 50) SpawnSystem(); //Note: Hard coded Randoms 001
            if (_difficulty > 2 && Random.Range(0, 100) < 80) SpawnSystem(); //Note: Hard coded Randoms 002
        }
    }
    private void EnemiesKilled(int notUsed, string enemyName)
    {
        _enemiesKilled++;
        if (enemyName == Types.Enemy.Scifi_Drone_04.ToString()) _enemyDroneKilled++;
        if (enemyName == Types.Enemy.Alien_Ship_001.ToString()) _enemyMiniBossKilled++;
        if (enemyName == Types.Enemy.Enemy000.ToString()) _proEnemyKilled++;
        if (_enemiesKilled >= _waveSize)
        {
            if (_waveSize > int.MaxValue) _waveSize = int.MaxValue;
            _enemiesKilled = 0;
            _waveSize += Random.Range(1,3);
            _canSpawnTime = Time.time;
            SpawnRandomEnemiesWave();
            _isWaveComplete = true;
        }
    }
    private void SpawnRandomEnemiesWave()
    {
        Debug.Log("New Wave Called");
        NewWaveEvent?.Invoke();
        _enemyDroneCount = 0;
        _enemyMiniBossCount = 0;
        _enemyDroneKilled = 0;
        _enemyMiniBossKilled = 0;
        _proEnemyCount = 0;
        _proEnemyKilled = 0;
        for (int i = 0; i < _waveSize; i++)
        {
            if (Random.Range(0, 101) <= 25) _enemyMiniBossCount++;
            else _enemyDroneCount++;
            _proEnemyCount++;
        }
    }
    private void SpawnSystem()
    {
        if (_isPoolMaxed)
        {
            Debug.Log("BPM " + _enemies[0].name + " CheckLoop");
            int sifiDroneCount = _enemies.FindAll(x => x.name == Types.Enemy.Scifi_Drone_04.ToString() + "(Clone)" && x.gameObject.activeSelf).Count;
            bool hasSifiDrons = _enemies.FindIndex(x => x.name == Types.Enemy.Scifi_Drone_04.ToString() + "(Clone)" && !x.gameObject.activeSelf) != -1;
            int alienShipCount = _enemies.FindAll(x => x.name == Types.Enemy.Alien_Ship_001.ToString() + "(Clone)" && x.gameObject.activeSelf).Count;
            bool hasAlienShip = _enemies.FindIndex(x => x.name == Types.Enemy.Alien_Ship_001.ToString() + "(Clone)" && !x.gameObject.activeSelf) != -1;
            int defaultESCount = _enemies.FindAll(x => x.name == Types.Enemy.Enemy000.ToString() + "(Clone)" && x.gameObject.activeSelf).Count;
            bool HasdefaultES = _enemies.FindIndex(x => x.name == Types.Enemy.Enemy000.ToString() + "(Clone)" && !x.gameObject.activeSelf) != -1;
            
            if (sifiDroneCount < _enemyDroneCount - _enemyDroneKilled && hasSifiDrons)
                SpawnEnemyType(Types.Enemy.Scifi_Drone_04);
            else if (alienShipCount < _enemyMiniBossCount - _enemyMiniBossKilled && hasAlienShip)
                SpawnEnemyType(Types.Enemy.Alien_Ship_001);
            else if (defaultESCount < _proEnemyCount - _proEnemyKilled && HasdefaultES)
            {
                Debug.Log("BPM " + _enemies[0].name);
                SpawnEnemyType(Types.Enemy.Enemy000);
            }
            else
                Debug.Log("Could not find Enemy of type XXX", transform);
        }
    }
    
    private void SpawnEnemyType(Types.Enemy enemyType)
    {
        if (_enemies.FindAll(x => x.name == enemyType.ToString() + "(Clone)").Count <= _enemies.Count / _enemyAsset.Count)
        {
            int tempEnemy = _enemies.FindIndex(x => !x.gameObject.activeSelf && x.name == enemyType.ToString() + "(Clone)");
            if (tempEnemy != -1)
            {
                _enemies[tempEnemy].gameObject.SetActive(true);
                _enemies[tempEnemy].position = RandomEnemySpawn() + CalcOffset(_enemies[tempEnemy]);
            }
        }
        else
            Debug.Log("Could not find Enemy of type XXX", transform);
    }

    private Vector3 CalcOffset(Transform enemyAsset)
    {
        float enemyBounds = enemyAsset.localScale.x * 0.5f;
        return new Vector3(enemyBounds, 0, 0);
    }

    private Vector3 RandomEnemySpawn() // DebugIt Move this script to EnemySpawnManager
    {
        //Todo: if eneymy already occupies space dont spawn there
        float _randomRandx = Random.Range(-(_xyBounds.x - _boundsOffset) * 1000, _xyBounds.x * 1000);
        _randomRandx *= 0.001f;
        return new Vector3(_randomRandx, _xyBounds.y, transform.position.z);
    }
    private void PlayerIsDead()
    {
        Debug.Log("GameOver :)");
        _gameOver = true;
    }
    private void OnDisable()
    {
        BossFightManager.ContinueWaves += () => SpawnWaveState(false);
        BossFightManager.PauseWaves += () => SpawnWaveState(true);

        EnemyCollisons.EnemyPointsEvent -= EnemiesKilled;
        BombEplode.BombExplosionEvent -= () => BPMPauseSpawn();
        Player.Game_Over -= PlayerIsDead;
        StartGameAsteroids.GameStarted -= GameStarted;
        StartGameAsteroids.SetDifficulty -= () => GameDificulty();
        BackGroundMusic_Events.BGM_Events -= () => { _beatEnemySpawner = true; };
    }
}
