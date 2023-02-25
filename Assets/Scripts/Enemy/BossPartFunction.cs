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
        _particleSystem = _particleGameObject.TryGetComponent(out ParticleSystem particleSystem) ? particleSystem : null;
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
        _particleGameObject.SetActive(false);
    }
    public void DisablePartFunction()
    {
        if (_particleSystem)
            StartCoroutine(StopParticleSystemIsPlaying());
        else
            Debug.Log("No Particle System Found", transform);
    }
}
