using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 4.0f;
    private Camera _camera;
    private float cameraAspecRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;
    private Vector2 xyBounds;
    [Space]
    private bool move = false;
    [Space]
    private float randomRandx = 0;
    private float boundsOffset = 0;

    private void Awake()
    {
        _camera = Camera.main;
        yBounds = _camera.orthographicSize;
        xBounds = yBounds * cameraAspecRatio;
        xyBounds = new Vector2(xBounds, yBounds);

        boundsOffset = transform.localScale.x * 0.5f;
    }
    void FixedUpdate()
    {
        Vector3 movePlayer = speed * Vector3.down * Time.fixedDeltaTime;
        movePlayer = OutOfBounds.CalculateMove(transform, movePlayer, xyBounds);
        if (movePlayer == Vector3.zero) RandomEnemySpawn();// gameObject.SetActive(false);
        transform.Translate(movePlayer); 
    }
    private void OnEnable()
    {
        move = true;
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        move = false;
    }

    private void RandomEnemySpawn() // DebugIt Move this script to EnemySpawnManager
    {
        randomRandx = Random.Range(-(xBounds - boundsOffset) * 1000, xBounds * 1000);
        randomRandx *= 0.001f;
        transform.position = new Vector3(randomRandx, yBounds, 0);
        //return new Vector3(randomRandx, yBounds, 0);
    }
}
