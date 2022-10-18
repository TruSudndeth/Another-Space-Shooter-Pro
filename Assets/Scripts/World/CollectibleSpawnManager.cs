using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _powerUpAssets;
    private List<Transform> _powerUps;
    [Space]
    private float _cameraAspecRatio = 1.7777778f;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private float _spawnRate = 7.0f;
    private float _currentSpawnTime = 0;
    [Space]
    [SerializeField] private bool _spawnPowerUps = false;

    private bool _gameStarted = false;

    private void Start()
    {
        StartGameAsteroids._startGame += GameStarted;
    }
    
    void GameStarted()
    {
        _gameStarted = true;
    }
    void Awake()
    {
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;

        _powerUps = new(_powerUpAssets.Count); //currently 3 power ups
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Add if statment to spawn every 7 seconds Time.time
        if (_powerUpAssets.Count == 0 || !_gameStarted) return;
        if(_currentSpawnTime + _spawnRate <= Time.time)
        {
            _currentSpawnTime = Time.time;
            int powerUpCount = _powerUpAssets.Count;
            _powerUps.Add(Instantiate(_powerUpAssets[RandomInt(powerUpCount)], RandomXSpawn(), Quaternion.identity, transform));
        }
    }

    private Vector3 RandomXSpawn()
    {
        float randomRandx = Random.Range(-(_xyBounds.x) * 1000, _xyBounds.x * 1000);
        randomRandx *= 0.001f;
        return new Vector3(randomRandx, _xyBounds.y, transform.position.z);
    }

    private int RandomInt(int myInt)
    {
        return Random.Range(0, myInt);
    }
    private void OnDestroy()
    {
        StartGameAsteroids._startGame -= GameStarted;
    }
}
