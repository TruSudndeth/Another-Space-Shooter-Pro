using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    private float cameraAspecRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;
    private Camera _camera;
    private Vector2 xyBounds;
    [SerializeField] private float speed = 4.0f;
    private bool move = false;


    private void Awake()
    {
        _camera = Camera.main;
        yBounds = _camera.orthographicSize;
        xBounds = yBounds * cameraAspecRatio;
        xyBounds = new Vector2(xBounds, yBounds);
    }
    void FixedUpdate()
    {
        if (move)
        {
            Vector3 movePlayer = speed * Vector3.down * Time.fixedDeltaTime;
            movePlayer = OutOfBounds.CalculateMove(transform, movePlayer, xyBounds);
            if (movePlayer == Vector3.zero) gameObject.SetActive(false);
            transform.Translate(movePlayer);
        }
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
}
