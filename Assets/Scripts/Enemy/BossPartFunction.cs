using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
    
public class BossPartFunction : MonoBehaviour
{
    [SerializeField]
    private GameObject _particleGameObject;
    [SerializeField]
    private float _timeLapsIsPlaying = 5.0f;
    private ParticleSystem _particleSystem;
    void Start()
    {
        BossFightManager.ResetBossEvent += ResetAll;
        _particleSystem = _particleGameObject.TryGetComponent(out ParticleSystem particleSystem) ? particleSystem : null;
    }
    private void ResetAll()
    {
        EnablePartFunction();
    }
    IEnumerator StopParticleSystemIsPlaying()
    {
        float time = 0;
        _particleSystem.Stop(true);
        while (_particleSystem.isPlaying)
        {
            time += Time.fixedDeltaTime;
            if (time > _timeLapsIsPlaying)
            {
                yield break;
            }
            if (_particleSystem.isPlaying)
                yield return null;
        }
        //Debug: Disabling this object is ineffective when we can just play and stop
        _particleGameObject.SetActive(false);
    }
    public void DisablePartFunction()
    {
        if (_particleSystem)
            StartCoroutine(StopParticleSystemIsPlaying());
        else
            Debug.Log("No Particle System Found", transform);
    }
    public void EnablePartFunction()
    {
        if (_particleSystem)
        {
            _particleGameObject.SetActive(true);
            _particleSystem.Play(true);
        }
        else
            Debug.Log("No Particle System Found", transform);
    }
    private void OnDisable()
    {
        BossFightManager.ResetBossEvent -= ResetAll;
    }
}
