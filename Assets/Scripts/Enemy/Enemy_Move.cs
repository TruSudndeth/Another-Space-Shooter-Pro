using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Todo: Change Enemy laser Transform tags to EnemyLaser and player transform laser to PlayerLaser
public class Enemy_Move : MonoBehaviour
{
    private float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Camera _camera;
    private Vector2 _xyBounds;
    [SerializeField] private float _speed = 4.0f;
    private bool _move = false;


    private void Awake()
    {
        _camera = Camera.main;
        _yBounds = _camera.orthographicSize;
        _xBounds = _yBounds * _cameraAspecRatio;
        _xyBounds = new Vector2(_xBounds, _yBounds);
    }
    void FixedUpdate()
    {
        if (_move)
        {
            Vector3 movePlayer = _speed * Vector3.down * Time.fixedDeltaTime;
            movePlayer = OutOfBounds.CalculateMove(transform, movePlayer, _xyBounds);
            if (movePlayer == Vector3.zero) gameObject.SetActive(false);
            transform.Translate(movePlayer);
        }
    }
    private void OnEnable()
    {
        _move = true;
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        _move = false;
    }
}
