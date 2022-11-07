using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    //Todo: PowerUp Shield energy depleating Timmer warning. Sound (OutOfTimeShield)
    [SerializeField] private Types.PowerUps _powerUpType;
    [Space]
    [SerializeField] private float _speed = 3.0f;
    private float _cameraAspecRatio = 1.7777778f;
    private Vector3 _move = Vector3.down;
    private Vector2 _xyBounds = Vector2.zero;

    private void Awake()
    {
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;
    }

    void FixedUpdate()
    {
        _move = Vector3.down * _speed * Time.fixedDeltaTime;
        _move = OutOfBounds.CalculateMove(transform, _move, _xyBounds);
        if (_move == Vector3.zero) gameObject.SetActive(false);
        transform.Translate(_move);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Types.Tag.Player.ToString()))
        {
            if(other.TryGetComponent(out Player playerPowerups))
            {
                switch(_powerUpType)
                {
                    case Types.PowerUps.Tripple:
                        playerPowerups.TripleShotActive();
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.Shield:
                        playerPowerups.ShieldActive();
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.Speed:
                        playerPowerups.SpeedBoost();
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.Ammo:
                        playerPowerups.AddAmmo();
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.Health:
                        //Todo: Add Health to Player playerPowerups.AddHealth();
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.Rocket:
                        //Todo: Add Rockets to playerPowerups.AddRocket();
                        gameObject.SetActive(false);
                        break;
                    default:
                        Debug.Log(transform + "Power Up type not set");
                        break;
                }
                AudioManager.Instance.PlayAudioOneShot(Types.SFX.PickUp);
            }
        }
    }
}
