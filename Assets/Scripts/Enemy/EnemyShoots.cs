using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoots : MonoBehaviour
{
    [SerializeField] private Types.SFX _sfx;
    [SerializeField] private List<Transform> _laserSpawnPoints;
    [Space]
    [Tooltip("If random 01 float is greater than this value")]
    [SerializeField] private float _shouldFire = 0.25f;
    private bool _fired = false;
    private void OnEnable()
    {
        //_laserSpawnPoints = new(3);
        _fired = false;
        BackGroundMusic_Events.BGM_Events += Shoot;
    }

    private void Shoot()
    {
        if (_shouldFire < ShouldFire() && !_fired)
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
    
    private float ShouldFire()
    {
        return Random.Range(0.0f, 1.0f);
    }

    private void OnDisable()
    {
        BackGroundMusic_Events.BGM_Events -= Shoot;
    }
}
