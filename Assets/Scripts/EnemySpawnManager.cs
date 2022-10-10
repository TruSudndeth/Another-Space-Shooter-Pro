using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _enemyAsset;
    private List<Transform> _enemies;
    [Space]
    private float _randomRandx = 0;
    private float _boundsOffset = 0;
    [Space]
    private Camera _camera;
    private float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    [Space]
    [SerializeField] private int _maxPool = 10;
    [SerializeField] private float _spawnRate = 0.5f;
    private bool _isPoolMaxed = false;
    private float _canSpawn = 0;
    private float _offset = 0;
    private int _iterateEnemy = 0;
    private bool _gameOver = false;


    private void Awake()
    {
        _maxPool = _enemyAsset.Count * 10;
        _enemies = new(_maxPool);
    }

    void Start()
    {
        _camera = Camera.main;
        _yBounds = _camera.orthographicSize;
        _xBounds = _yBounds * _cameraAspecRatio;
        PlayerInput.gameOver += PlayerIsDead;
    }

    void FixedUpdate()
    {
        if (_gameOver) return;
        if (_canSpawn + _spawnRate > Time.time) return;
        _canSpawn = Time.time;
        SpawnSystem();
    }

    private void SpawnSystem()
    {
            if (_enemies.Count < _maxPool && !_isPoolMaxed)
            {
            _enemies.Add(Instantiate(_enemyAsset[0], RandomEnemySpawn() + CalcOffset(_enemyAsset[0]), Quaternion.identity, transform));
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
        _randomRandx = Random.Range(-(_xBounds - _boundsOffset) * 1000, _xBounds * 1000);
        _randomRandx *= 0.001f;
        return new Vector3(_randomRandx, _yBounds, transform.position.z);
    }
    private void PlayerIsDead()
    {
        Debug.Log("GameOver :)");
        _gameOver = true;
    }
    private void OnDisable()
    {
        PlayerInput.gameOver -= PlayerIsDead;
    }
}
