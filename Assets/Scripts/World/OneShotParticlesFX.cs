using UnityEngine;

public class OneShotParticlesFX : MonoBehaviour
{
    //particle should play once then disable its self.
    [SerializeField]
    private ParticleSystem _particleSystems;
    private bool _canDissableTransform = false;
    private void OnEnable()
    {
        if(_particleSystems == null)
        {
            _particleSystems = GetComponent<ParticleSystem>();
            Debug.Log("particle system was set");
            _canDissableTransform = true;
        }
    }
    void FixedUpdate()
    {
        if(_canDissableTransform) 
        {
            if(!_particleSystems.isPlaying)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
