using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] [Range(1.0f,5.0f)] private float _shakeAmplitude = 1f;
    private float _shakeTimer = 0;
    private CinemachineVirtualCamera _cameraCMV;
    void Start()
    {
        _cameraCMV = GetComponent<CinemachineVirtualCamera>();
        Player.OnPlayerDamage += ShakeCamera;
    }
    
    void FixedUpdate()
    {
        //shake camera if shake timer is greater than 0 with shakeTime duration
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime / shakeTime;
            if(_shakeTimer >= 1)
            {
                _shakeTimer = 0;
                _cameraCMV.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            }
            float lerpShake = Mathf.Lerp(0, 1, _shakeTimer);
            _cameraCMV.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = lerpShake * _shakeAmplitude;
        }
    }
    private void ShakeCamera(float damageDurationSec)
    {
        _shakeTimer = damageDurationSec;
    }

    private void OnDisable()
    {
        Player.OnPlayerDamage -= ShakeCamera;
    }
}
