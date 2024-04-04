using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public delegate void NewWave();
    public static event NewWave NewWaveEvent;

    public delegate void FeedBack(int maxCount);
    public static event FeedBack SpawnFeedbackCount;
    public static event FeedBack EnemiesKilledFeedBack;
    public static event FeedBack IncreaseDifficultyCurve;

    [SerializeField] private List<Transform> _enemyAsset;
    private List<Transform> _enemies;
    private int _enemyCount = 0;
    [Space]
    private readonly float _cameraAspecRatio = 1.7777778f;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private int _maxPool = 10;
    private bool _isPoolMaxed = false;
    private bool _gameOver = false;
    private bool _gameStarted = false;
    [Space]
    private bool _beatEnemySpawner = false;
    private float _difficulty = 0.0f;
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
    [SerializeField]
    private bool _isMotherShipActive = false;
    //Complete: limit number of enemies on screen on easy and slow.
    private int _enemiesSpawnCount = 0;
    private void Awake()
    {
        //_waveIsPaused = false;
        _maxPool = _enemyAsset.Count * 10; //Note: EnemySpawnManager Enemy count hard coded to 10*Count
        _enemies = new(_maxPool);
        _enemyCount = _enemyAsset.Count;
        PopulatePool();
    }
    void Start()
    {
        GameManager.NewDifficulty += (x) => SetNewDifficulty(x);

        GameManager.MasterDifficulty += (x) => SetDifficulty(x);

        BossFightManager.ActiveMother += (x) => { _isMotherShipActive = x; };
        BossFightManager.ContinueWaves += (x) => SpawnWaveState(false);
        BossFightManager.PauseWaves += (x) => SpawnWaveState(true);

        EnemyCollisons.EnemyPointsEvent += EnemiesKilled;
        BombEplode.BombExplosionEvent += () => BPMPauseSpawn();
        BackGroundMusic_Events.BGM_Events += () => { _beatEnemySpawner = true; };
        StartGameAsteroids.GameStarted += GameStarted;


        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;

        Player.Game_Over += PlayerIsDead;
        SpawnRandomEnemiesWave();
        _enemiesSpawnCount = _waveSize;
    }
    private void PopulatePool()
    {
        // return an error if _enemyAset.count == 0;
        int eAssetCount = _enemyAsset.Count;
        Transform ePlaceHolder = null;
        if(eAssetCount == 0)
        {
            Debug.Log("PopulatePool Failed no Assests found or cleaned before population");
            return;
        }
        for (int i = 0; i < eAssetCount; i++)
        {
            for (int j = 0; j < _maxPool / eAssetCount; j++)
            {
                _enemies.Add(Instantiate(_enemyAsset[i], new Vector3(0, 0, 0), Quaternion.identity, transform));
                ePlaceHolder = _enemies[^1];
                Vector3 playerBounds = ePlaceHolder.GetComponentInChildren<Renderer>().bounds.size;
                SetShipSize(playerBounds.y);
                //Debug.Log("XYBounds " + playerBounds, transform);
                ePlaceHolder.gameObject.SetActive(false);
                _isPoolMaxed = true;
            }
        }
    }
    private void SetShipSize(float size)
    {
        if(size > _ESizeH)
        _ESizeH = size;
    }
    private void SetDifficulty(float setDifficulty)
    {
        //Master difficulty at game start
        _difficulty = setDifficulty;
        _multiSpawnProbability = MathFunctionsHelper.Map(_difficulty, 0, 4, 0, 100);
        Debug.Log("Difficulty was set to " + setDifficulty);
        _maxPool = Mathf.RoundToInt(_maxPool * _difficulty);
        CalculateSpawnCount();
    }
    private int _multiSpawnProbability = 0;
    private void SetNewDifficulty(float newDifficulty)
    {
        _difficulty = newDifficulty;
        _multiSpawnProbability = MathFunctionsHelper.Map(_difficulty, 0, 4, 0, 100);
        // Debug.Log("percentage " + _multiSpawnProbability);
        CalculateSpawnCount();
        //must make equations to set a spawn difficulty using a float where hard (3) is 1 spawn per beat
        //Adjust spawns per measure here _spawnsPerMeasure spawn rate where hard is equal to 4.
        //Sputter some groups here adjust fixed update line 129
        //current difficulty and test if we should increase dificulty. if so by how mucho
        //Level 0 is 1 spawn per 8 beats and level 4 is 1 spawn each beat.
        // only adjust selected diffictulty +- 1
        //Eq. 8 - (newDifficulty * 2) When zero spawn more groups at the same time.
    }
    [SerializeField]
    private float _ESizeHBufferSize = 0.0f;
    private float _ESizeH = 0.0f;
    [Range(0.0f, 1.0f)]
    public float _onScreenSpawnMultiplier = 0.1f;
    private void CalculateSpawnCount()
    {
        // (Screen size height / (biggest EShip height + (buffer * 2) ) * current difficulty.
        // Test for Divide by zero.
        //Consider: Adding all the sizes and averaging them to get screen enemy screen count.
        Vector2 screenSize = GameManager.Instance.XYBounds;
        int eYCount = 0;
        if (_ESizeH != 0)
            eYCount = Mathf.RoundToInt(screenSize.y / _ESizeH);
        else
        {
            Debug.Log("Devide by Zero", transform);
            return;
        }
        // level 0 - 4 range. 1 spawn count to full _maxPool
        float ECount = MathFunctionsHelper.Map(_difficulty, 0.0f, 4.0f, 1, eYCount);
        _enemiesSpawnCount = Mathf.RoundToInt(ECount * _difficulty);
        _enemiesSpawnCount = _enemiesSpawnCount <= 0 ? 1 : _enemiesSpawnCount;
        // Debug.Log(_enemiesSpawnCount + " Enemies spawned with difficulty " + _difficulty);
    }
    private void SpawnWaveState(bool isWavesPaused = false)
    {
        _gameStarted = !isWavesPaused;
        _isWaveComplete = isWavesPaused;
    }
    private void BPMPauseSpawn()
    {
        _canSpawnTime = Time.time;
    }
    private void GameStarted()
    {
        _gameStarted = true;
    }
    private bool _canSpawn = false;
    private int _BPMeasure = 0;
    [SerializeField]
    private int _spawnsPerMeasure = 4;
    void FixedUpdate()
    {
        if (_gameOver || !_gameStarted) return;
        //Note: _isWavesPaused could cause a glitch with current enemy count or point system.
        //Temp: Timmer might not have to spawn enemies with time 
        //Temp: Timmer if (_canSpawn + _spawnRate > Time.time) return;
        //Note: Spawn manager was stopped during game play for a brif moment
        if(_beatEnemySpawner)
        {
            _beatEnemySpawner = false;
            _BPMeasure++;
        }
        if (!_isWaveComplete && _BPMeasure >= _spawnsPerMeasure && _spawnDelay + _canSpawnTime < Time.time)
        {
            //return all active enemies int the list in hierarchy
            _spawnRateFeedback++;
            _BPMeasure = 0;
            int activeEnemies = _enemies.FindAll(x => x.gameObject.activeSelf).Count;
            if (activeEnemies >= _waveSize - _enemiesKilled) return;
            SpawnSystem();
            if (_difficulty <= 1.0f) return;
            if (_difficulty > 1.0f && Random.Range(0, 100) <= _multiSpawnProbability) SpawnSystem();
            if (_difficulty > 2.0f && Random.Range(0, 100) <= (float) _multiSpawnProbability / 2) SpawnSystem();
        }
    }
    //Feedback for caclulating spawn rate as you progress threw the game.
    private int _spawnRateFeedback = 0;
    private void EnemiesKilled(int notUsed, string enemyName)
    {
        EnemiesKilledFeedBack?.Invoke(_waveSize);
        _enemiesKilled++;
        _enemyKilledIncreaseDiff++;
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
            if(_isMotherShipActive)
            _isWaveComplete = true;
        }
    }
    private void SpawnRandomEnemiesWave()
    {
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
    private int _enemyKilledIncreaseDiff = 0;
    private int _enemySpawnIncreaseDiff = 0;
    private void SpawnSystem()
    {
        // keep track where the enemy was last spawned and alter adjustments based on that.
        // Send GameManager the count of Enemy spawns Difficulty curve.
        if (_isPoolMaxed)
        {
            SpawnFeedbackCount?.Invoke(_waveSize);
            _enemySpawnIncreaseDiff++;
            // Debug.Log("BPM " + _enemies[0].name + " CheckLoop");
            int sifiDroneCount = _enemies.FindAll(x => x.name == Types.Enemy.Scifi_Drone_04.ToString() + "(Clone)" && x.gameObject.activeSelf).Count;
            bool hasSifiDrons = _enemies.FindIndex(x => x.name == Types.Enemy.Scifi_Drone_04.ToString() + "(Clone)" && !x.gameObject.activeSelf) != -1;
            int alienShipCount = _enemies.FindAll(x => x.name == Types.Enemy.Alien_Ship_001.ToString() + "(Clone)" && x.gameObject.activeSelf).Count;
            bool hasAlienShip = _enemies.FindIndex(x => x.name == Types.Enemy.Alien_Ship_001.ToString() + "(Clone)" && !x.gameObject.activeSelf) != -1;
            int defaultESCount = _enemies.FindAll(x => x.name == Types.Enemy.Enemy000.ToString() + "(Clone)" && x.gameObject.activeSelf).Count;
            bool HasdefaultES = _enemies.FindIndex(x => x.name == Types.Enemy.Enemy000.ToString() + "(Clone)" && !x.gameObject.activeSelf) != -1;
            
            int count = sifiDroneCount + alienShipCount + defaultESCount;
            Debug.Log("Enemies on screen is greater then _enemiesSpawned " + count + " then " + _enemiesSpawnCount);
            int spawnCountLimit = CalcIncreaseDifficulty(_enemiesSpawnCount);
            if (sifiDroneCount + alienShipCount + defaultESCount >= _enemiesSpawnCount)
            {
                //This Difficulty curve might be too fast rate of change
                if(_enemyKilledIncreaseDiff >= spawnCountLimit || _enemySpawnIncreaseDiff >= spawnCountLimit)
                {
                    IncreaseDifficultyCurve?.Invoke(spawnCountLimit);
                    _enemyKilledIncreaseDiff = 0;
                    _enemySpawnIncreaseDiff = 0;
                    Debug.Log("Increased Difficulty " + _difficulty + " " + count + " count of " + spawnCountLimit);
                }
                return;
            }
            else
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
            {
                //Debug: This triggered 6 times in one play threw to mother ship 2
                Debug.Log("Could not find Enemy of type XXX", transform);
            }
        }
    }
    private int CalcIncreaseDifficulty(int value)
    {
        int newCount = Mathf.RoundToInt(value * _difficulty);
        return newCount == 0 ? value : newCount;
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
        {
            //Debug: This triggered 6 times in one play threw to mother ship 2
            Debug.Log("Could not find Enemy of type XXX", transform);
        }
    }

    private Vector3 CalcOffset(Transform enemyAsset)
    {
        float enemyBounds = enemyAsset.localScale.x * 0.5f;
        return new Vector3(enemyBounds, 0, 0);
    }

    private Vector3 RandomEnemySpawn() // DebugIt Move this script to EnemySpawnManager
    {
        //Todo: if eneymy already occupies space dont spawn there
        // a better float random calculation
        float _randomRandx = Random.Range(-(_xyBounds.x) * 1000, _xyBounds.x * 1000);
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
        GameManager.NewDifficulty -= (x) => SetNewDifficulty(x);
        GameManager.MasterDifficulty -= (x) => SetDifficulty(x);

        BossFightManager.ActiveMother -= (x) => { _isMotherShipActive = x; };
        BossFightManager.ContinueWaves -= (x) => SpawnWaveState(false);
        BossFightManager.PauseWaves -= (x) => SpawnWaveState(true);

        EnemyCollisons.EnemyPointsEvent -= EnemiesKilled;
        BombEplode.BombExplosionEvent -= () => BPMPauseSpawn();
        Player.Game_Over -= PlayerIsDead;
        StartGameAsteroids.GameStarted -= GameStarted;
        BackGroundMusic_Events.BGM_Events -= () => { _beatEnemySpawner = true; };
    }
}
