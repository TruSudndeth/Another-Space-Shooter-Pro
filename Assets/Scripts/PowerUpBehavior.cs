using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    //Todo: PowerUp Shield energy depleating Timmer warning. Sound (OutOfTimeShield)
    [SerializeField] private Types.PowerUps _powerUpType;
    [Space]
    [SerializeField] private float _speed = 3.0f;
    [Space]
    private float _cameraAspecRatio = 1.7777778f;
    private Vector3 _move = Vector3.down;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [Tooltip("float Random Range is less than Range float from 0 to 100%")]
    [SerializeField][Range(0, 100)] private float _antiProbablity = 0.5f;
    [SerializeField] Transform _negativePowerupVisual;
    private bool _antiPowerups = false;
    [SerializeField] [Space(25)]
    private Types.VFX _vfxType = Types.VFX.PlayerDeath;

    private void Awake()
    {
        _antiPowerups = Random.Range(0.0f, 100.0f) < _antiProbablity; //Fix: Move to enabled for pooling
        //Todo: Add a negative visual to the powerup
        CheckIfVisualExistAndSet(_antiPowerups);
        _xyBounds.y = Camera.main.orthographicSize;
        _xyBounds.x = _xyBounds.y * _cameraAspecRatio;
    }
    
    private void CheckIfVisualExistAndSet(bool isActive)
    {
        if (_negativePowerupVisual != null)
        {
            _negativePowerupVisual.gameObject.SetActive(isActive);
        }
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
        if (other.CompareTag(Types.Tag.Player.ToString()))
        {
            if(other.TryGetComponent(out Player playerPowerups))
            {
                switch(_powerUpType)
                {
                    case Types.PowerUps.TripleShotPowerup:
                        playerPowerups.TripleShotActive(_antiPowerups);
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.ShieldPowerup:
                        playerPowerups.ShieldActive(_antiPowerups);
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.SpeedPowerup:
                        playerPowerups.SpeedBoost(_antiPowerups);
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.AmmoPickup:
                        playerPowerups.AddAmmo(_antiPowerups);
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.HealthPackRed:
                        //Todo: Add Health to Player
                        playerPowerups.AddHealth(_antiPowerups);
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.BombPickup:
                        //Todo: Add Bombs to Player
                        playerPowerups.UseBomb();
                        gameObject.SetActive(false);
                        break;
                    case Types.PowerUps.HomingMissle:
                        playerPowerups.HomingMissleActive(_antiPowerups);
                        gameObject.SetActive(false);
                        break;
                    default:
                        Debug.Log(transform + "Power Up type not set");
                        break;
                }
                if(_antiPowerups)
                    AudioManager.Instance.PlayAudioOneShot(Types.SFX.ErrorSound);
                else
                    AudioManager.Instance.PlayAudioOneShot(Types.SFX.PickUp);
            }
        }
        if (other.CompareTag(Types.LaserTag.EnemyLaser.ToString()))
        {
            ParticlesVFX.OneShotExplosion?.Invoke(transform, _vfxType);
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }
    }
}
