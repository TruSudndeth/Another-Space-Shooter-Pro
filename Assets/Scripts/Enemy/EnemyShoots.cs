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
    private Vector3 _fireRotation = new(180, 0 , 0);
    private float _inFront = 0;

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
                    laser.rotation = Quaternion.Euler(Vector3.zero);
                }
            }
        }
    }

    private void Shoot()
    {
        if (ShouldFire() && !_fired)
        {
            _fired = true;
            LaserManager.Instance.LaserPool(_laserSpawnPoints[RandomGun()]);
            AudioManager.Instance.PlayAudioOneShot(_sfx);
        }
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
