using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _enemyAsset;
    private List<Transform> _enemies;
    private int _enemyCount = 0;
    [Space]
    private float _boundsOffset = 0;
    [Space]
    private Camera _camera;
    private float _cameraAspecRatio = 1.7777778f;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private int _maxPool = 10;
    [SerializeField] private float _spawnRate = 0.5f;
    private bool _isPoolMaxed = false;
    private float _canSpawn = 0;
    private float _offset = 0;
    private int _iterateEnemy = 0;
    private bool _gameOver = false;
    private bool _gameStarted = false;


    private void Awake()
    {
        //_maxPool = _enemyAsset.Count * 10;
        _enemies = new(_maxPool);
        _enemyCount = _enemyAsset.Count;
    }

    void Start()
    {
        StartGameAsteroids._startGame += GameStarted;
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;

        PlayerInput.gameOver += PlayerIsDead;
    }
    
    private void GameStarted()
    {
        _gameStarted = true;
    }

    void FixedUpdate()
    {
        if (_gameOver || !_gameStarted) return;
        if (_canSpawn + _spawnRate > Time.time) return;
        _canSpawn = Time.time;
        SpawnSystem();
    }

    private void SpawnSystem()
    {
            if (_enemies.Count < _maxPool && !_isPoolMaxed)
            {
                int enemyIndex = Random.Range(0, _enemyCount);
                _enemies.Add(Instantiate(_enemyAsset[enemyIndex], RandomEnemySpawn() + CalcOffset(_enemyAsset[enemyIndex]), Quaternion.identity, transform));
                _iterateEnemy++;
                if (_iterateEnemy == _maxPool)
                {
                    _iterateEnemy = 0;
                    _isPoolMaxed = true;
                }
            }
            else if (_isPoolMaxed)
            {
                //fire rate must not surpass laser pool check if object is disabled before using.
                //Lock rotations add recochet later
                for (int i = 0; i < _enemies.Count; i++)
                {
                    if (!_enemies[i].gameObject.activeSelf)
                    {
                        _enemies[i].gameObject.SetActive(true);
                        _enemies[i].position = RandomEnemySpawn() + CalcOffset(_enemies[i]);
                        break;
                    }
                }
            }
    }

    private Vector3 CalcOffset(Transform enemyAsset)
    {
        float enemyBounds = enemyAsset.localScale.x * 0.5f;
        return new Vector3(enemyBounds, 0, 0);
    }

    private Vector3 RandomEnemySpawn() // DebugIt Move this script to EnemySpawnManager
    {
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
        PlayerInput.gameOver -= PlayerIsDead;
        StartGameAsteroids._startGame -= GameStarted;
    }
}
