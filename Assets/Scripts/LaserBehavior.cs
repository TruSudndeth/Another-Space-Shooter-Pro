using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    private Camera _camera;
    private float cameraAspecRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;
    private Vector2 xyBounds;
    [Space]
    [SerializeField] private float speed = 10;
    private bool move = false;

    private void Awake()
    {
        _camera = Camera.main;
        yBounds = _camera.orthographicSize;
        xBounds = yBounds * cameraAspecRatio;
        xyBounds = new Vector2(xBounds, yBounds);
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
    void FixedUpdate()
    {
        if (move)
        {
            Vector3 moveLaser = speed * Time.fixedDeltaTime * transform.up;
            moveLaser = OutOfBounds.CalculateMove(transform, moveLaser, xyBounds);
            if(moveLaser == Vector3.zero)
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
