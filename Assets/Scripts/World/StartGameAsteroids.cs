using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameAsteroids : MonoBehaviour
{
    public delegate void StartGame();
    public static StartGame _startGame;
    public delegate void Dificulty();
    public static Dificulty _difficulty;

    [SerializeField] private float _rotationSpeedMax = 1.5f;
    private Rigidbody _rb;
    private float _rotationSpeed;
    private void Awake()
    {
        _rotationSpeed = Random.Range(0.1f, 0.3f);
    }
    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Rigidbody rb))
            {
                _rb = rb;
                _rb.AddTorque(RandomAngle(), RandomAngle(), RandomAngle());
            }
            else
                Debug.LogError("No Rigidbody found on " + gameObject.name);
        }
    }
    private void Update()
    {
        transform.Rotate(0, 0, _rotationSpeed);
        if (transform.childCount <= 0)
        {
            Debug.Log("Started Game");
            _startGame?.Invoke();
            Destroy(gameObject);
        }
    }
    private float RandomAngle()
    {
        _rotationSpeedMax = Random.Range(0.5f, _rotationSpeedMax);
        return Random.Range(1.0f, 100.0f * _rotationSpeedMax);
    }
    public void SetDificulty()
    {
        _difficulty?.Invoke();
    }
}
