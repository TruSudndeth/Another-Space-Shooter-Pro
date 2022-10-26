using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    private Camera _camera;
    private float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Vector2 _xyBounds;
    [Space]
    [SerializeField] private float _speed = 10;
    private bool _move = false;

    private void Awake()
    {
        _camera = Camera.main;
        _yBounds = _camera.orthographicSize;
        _xBounds = _yBounds * _cameraAspecRatio;
        _xyBounds = new Vector2(_xBounds, _yBounds);
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
    void FixedUpdate()
    {
        if (_move)
        {
            Vector3 checkForBounds = Vector3.zero;
            Vector3 moveLaser = _speed * Time.fixedDeltaTime * transform.up;
            checkForBounds = OutOfBounds.CalculateMove(transform, moveLaser, _xyBounds);
            if(moveLaser == Vector3.zero || checkForBounds.magnitude > moveLaser.magnitude * 2)
            {
                KillLaser();
                return;
            }
            transform.position += moveLaser;
        }
    }

    private void KillLaser()
    {
        transform.gameObject.SetActive(false);
    }

}
