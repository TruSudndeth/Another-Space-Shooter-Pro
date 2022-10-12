using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    [SerializeField] private Type.PowerUps _powerUpType;
    [Space]
    [SerializeField] private float _speed = 3.0f;
    private float cameraAspecRatio = 1.7777778f;
    private Vector3 move = Vector3.down;
    private Vector2 _xyBounds = Vector2.zero;

    private void Awake()
    {
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * cameraAspecRatio;
    }

    void FixedUpdate()
    {
        move = Vector3.down * _speed * Time.fixedDeltaTime;
        move = OutOfBounds.CalculateMove(transform, move, _xyBounds);
        if (move == Vector3.zero) gameObject.SetActive(false);
        transform.Translate(move);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Type.Tags.Player.ToString()))
        {
            if(other.TryGetComponent(out PlayerInput playerIO))
            {
                switch(_powerUpType)
                {
                    case Type.PowerUps.Tripple:
                        playerIO.TripleShotActive();
                        gameObject.SetActive(false);
                        break;
                    case Type.PowerUps.Shield:
                        Debug.Log("Shield Stuff");
                        gameObject.SetActive(false);
                        break;
                    case Type.PowerUps.Speed:
                        playerIO.SpeedBoost();
                        gameObject.SetActive(false);
                        break;
                    default:
                        Debug.Log(transform + "Power Up type not set");
                        break;
                }
            }
        }
    }
}
