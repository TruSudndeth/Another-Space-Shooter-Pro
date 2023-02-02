using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyShoots : MonoBehaviour
{
    [SerializeField] private Types.SFX _sfx;
    [SerializeField] private List<Transform> _laserSpawnPoints;
    [Space]
    [Tooltip("If random 01 float is greater than this value")]
    [SerializeField] private float _shouldFire = 0.25f;
    private bool _fired = false;
    public bool Fired { get => _fired; }

    private Transform _player;
    private bool _reverseFire = false;
    private Vector3 _fireRotation = new(90, 0 , 0);
    private Vector3 _reverseFireRotation = new(-90, 0, 0);
    private float _inFront = 0;
    private float _powerupInfront = 0;
    [SerializeField][Range(0.0f, 1.0f)] float _inFrontRange = 0.1f;
    private Transform _powerupTransfrom;

    private void Start()
    {
        if (!_player)
            _player = GameObject.FindGameObjectWithTag(Types.Tag.Player.ToString()).transform;
    }
    private void OnEnable()
    {
        

        //_laserSpawnPoints = new(3);
        _fired = false;
        BackGroundMusic_Events.BGM_Events += Shoot;
    }

    private void FixedUpdate()
    {
        //if player is behind Enemy rotate laser
        _inFront = Vector3.Dot((_player.position - transform.position).normalized, Vector3.down);
        if (_laserSpawnPoints.Count > 0)
        {
            if(_inFront > 0)
            {
                foreach (Transform laser in _laserSpawnPoints)
                {
                    laser.rotation = Quaternion.Euler(_fireRotation);
                }
            }
            else
            {
                foreach (Transform laser in _laserSpawnPoints)
                {
                    laser.rotation = Quaternion.Euler(_reverseFireRotation);
                }
            }
        }
    }

    private void RequestPowerupLineUp()
    {
        //request _Enemy_Move to lineup enemy shot in the x axes
    }

    private void Shoot()
    {
        foreach(Transform powerup in CollectibleSpawnManager.Instance.GetActivePowerupPool())
        {
            if(IsInfront(powerup))
            {
                _fired = false;
            }
        }
        if (ShouldFire() && !_fired)
        {
            _fired = true;
            LaserManager.Instance.LaserPool(_laserSpawnPoints[RandomGun()], false);
            AudioManager.Instance.PlayAudioOneShot(_sfx);
        }
    }

    private bool IsInfront(Transform powerup)
    {
        //LeftOff: Object not set to an instance PowerUp
        float inFront = Vector3.Dot((powerup.position - transform.position).normalized, Vector3.down);
        if (1 - _inFrontRange < inFront) 
            return true;
        else
            return false;
    }
    
    private int RandomGun()
    {
        return Random.Range(0, _laserSpawnPoints.Count);
    }
    
    private bool ShouldFire()
    {
        return Random.Range(0.0f, 1.0f) < _shouldFire;
    }

    private void OnDisable()
    {
        BackGroundMusic_Events.BGM_Events -= Shoot;
    }
}
