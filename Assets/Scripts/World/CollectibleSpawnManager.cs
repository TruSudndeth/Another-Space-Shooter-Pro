using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class CollectibleSpawnManager : MonoBehaviour
{
    //LeftOff: 1. Create an instance to this class
    public static CollectibleSpawnManager Instance;

    [SerializeField] private List<Transform> _powerupAssets;
    private List<Transform> _collectableList;
    private List<Transform> _returnPowerups;
    [Space]
    private float _cameraAspecRatio = 1.7777778f;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private float _spawnRate = 7.0f;
    private float _currentSpawnTime = 0;
    [Space]
    private bool _spawningBPM = false;

    private bool _gameStarted = false;

    private void Start()
    {
        if(!Instance)
            Instance = this;
        else
            Destroy(this);
        StartGameAsteroids.GameStarted += GameStarted;
        BackGroundMusic_Events.BGM_Events += () => _spawningBPM = true;
    }
    
    void GameStarted()
    {
        _gameStarted = true;
    }
    void Awake()
    {
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;
        _returnPowerups = new List<Transform>();
        _collectableList = new(_powerupAssets.Count); //currently 3 power ups
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Add if statment to spawn every 7 seconds Time.time
        if (_powerupAssets.Count == 0 || !_gameStarted) return;
        if (_currentSpawnTime + _spawnRate <= Time.time && _spawningBPM)
        {
            _spawningBPM = false;
            _currentSpawnTime = Time.time;
            int randomPrefab = RandomInt();
            Debug.Log(randomPrefab);
            _collectableList.Add(Instantiate(_powerupAssets[randomPrefab], RandomXSpawn(), Quaternion.identity, transform)); //Debugit: IndexOut of Range -1, < collection
        }
        else
            _spawningBPM = false;
    }

    private Vector3 RandomXSpawn()
    {
        float randomRandx = Random.Range(-(_xyBounds.x) * 1000, _xyBounds.x * 1000);
        randomRandx *= 0.001f;
        return new Vector3(randomRandx, _xyBounds.y, transform.position.z);
    }
    
    public List<Transform> GetActivePowerupPool()
    {
        
        _returnPowerups = _collectableList.Where(x => x.gameObject.activeSelf).ToList();
        return _returnPowerups;
    }
    private int RandomInt()
    {
        //GDHQ: New Projectile RareSpawn
        int randomProbability = Random.Range(0, 101);
        if (randomProbability <= 10)
            if (Random.Range(0, 101) <= 50)
                return _powerupAssets.FindIndex(b => b.name == Types.PowerUps.BombPickup.ToString());
            else
                return _powerupAssets.FindIndex(b => b.name == Types.PowerUps.HealthPackRed.ToString());
        else if (randomProbability <= 20)
            return _powerupAssets.FindIndex(b => b.name == Types.PowerUps.ShieldPowerup.ToString());
        else if (randomProbability <= 25)
            return _powerupAssets.FindIndex(b => b.name == Types.PowerUps.SpeedPowerup.ToString());
        else if (randomProbability <= 35)
            return _powerupAssets.FindIndex(b => b.name == Types.PowerUps.TripleShotPowerup.ToString());
        else
            return _powerupAssets.FindIndex(b => b.name == Types.PowerUps.AmmoPickup.ToString());
    }
    private void OnDestroy()
    {
        StartGameAsteroids.GameStarted -= GameStarted;
        BackGroundMusic_Events.BGM_Events -= () => _spawningBPM = true;
    }
}
