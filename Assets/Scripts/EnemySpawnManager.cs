using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> EnemyAsset;
    private List<Transform> enemies;
    [Space]
    private float randomRandx = 0;
    private float boundsOffset = 0;
    [Space]
    private Camera _camera;
    private float cameraAspecRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;
    [Space]
    [SerializeField] private int maxPool = 10;
    [SerializeField] private float spawnRate = 0.5f;
    private bool isPoolMaxed = false;
    private float canSpawn = 0;
    private float Offset = 0;
    private int iterateEnemy = 0;
    private bool _canSpawn = false;

    // Start is called before the first frame update
    private void Awake()
    {
        maxPool = EnemyAsset.Count * 10;
        enemies = new(maxPool);
    }
    void Start()
    {
        _camera = Camera.main;
        yBounds = _camera.orthographicSize;
        xBounds = yBounds * cameraAspecRatio;
        StartCoroutine("SpawnEnemyRate");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (canSpawn + spawnRate > Time.time) return;
        //canSpawn = Time.time;
        if(_canSpawn)
        {
            SpawnSystem();
            _canSpawn = false;
        }
    }

    private void SpawnSystem()
    {
            if (enemies.Count < maxPool && !isPoolMaxed)
            {
            enemies.Add(Instantiate(EnemyAsset[0], RandomEnemySpawn() + CalcOffset(EnemyAsset[0]), Quaternion.identity, transform));
                iterateEnemy++;
                if (iterateEnemy == maxPool)
                {
                iterateEnemy = 0;
                    isPoolMaxed = true;
                }
            }
            else if (isPoolMaxed)
            {
                //fire rate must not surpass laser pool check if object is disabled before using.
                //Lock rotations add recochet later
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (!enemies[i].gameObject.activeSelf)
                    {
                        Debug.Log("this code is running");
                        enemies[i].gameObject.SetActive(true);
                        enemies[i].position = RandomEnemySpawn() + CalcOffset(enemies[i]);
                        break;
                    }
                }
            }
        
    }
    IEnumerator SpawnEnemyRate()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnRate);
            _canSpawn = true;
        }
    }

    private Vector3 CalcOffset(Transform enemyAsset)
    {
        float enemyBounds = enemyAsset.localScale.x * 0.5f;
        return new Vector3(enemyBounds, 0, 0);
    }

    private Vector3 RandomEnemySpawn() // DebugIt Move this script to EnemySpawnManager
    {
        randomRandx = Random.Range(-(xBounds - boundsOffset) * 1000, xBounds * 1000);
        randomRandx *= 0.001f;
        return new Vector3(randomRandx, yBounds, 0);
    }
}
