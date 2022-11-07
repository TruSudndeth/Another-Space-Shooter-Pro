using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombEplode : MonoBehaviour
{
    public delegate void BombExplosion();
    public static BombExplosion BombExplosionEvent;

    [SerializeField] private Types.SFX _explodeSFX;
    [SerializeField] private Types.VFX _vfxType;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionForce = 700f;
    [SerializeField] private float _explosionUpward = 0.4f;
    [SerializeField] private float _explosionDamage = 100f;
    [SerializeField] private float _explosionDelay = 0.25f; //Todo: delet line
    private Animator _animator;
    private bool _hasAnimator = false;
    private AudioSource _audioSource;
    [Space]
    private bool _hasExploded = false;
    private bool _oneShot = false;

    private void Start()
    {
        if (TryGetComponent(out AudioSource audioSource))
        {
            _audioSource = audioSource;
        }
        if (TryGetComponent(out Animator animator))
        {
            _hasAnimator = true;
            _animator = animator;
        }
        else
            Debug.Log("No Animator Found" + transform);
    }
    private void OnEnable()
    {
        _oneShot = true;
        _hasExploded = false;
    }
    private void Update()
    {
        if (_hasAnimator && _oneShot)
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Explode"))
            {
                _audioSource.Play();
                Invoke("Explode", _audioSource.clip.length);
                _oneShot = false;
            }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            if (!nearbyObject.transform.CompareTag("Collectible"))
            {
                if (nearbyObject.TryGetComponent(out Rigidbody rb))
                {
                    if (rb != null)
                    {
                        rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _explosionUpward, ForceMode.Impulse);
                    }
                }
                else if (nearbyObject.TryGetComponent(out EnemyCollisons enemyCollision))
                {
                    enemyCollision.DestroyEnemy();
                }
                else if (nearbyObject.transform.parent.TryGetComponent(out EnemyCollisons enemyCollisionParent))
                {
                    enemyCollisionParent.DestroyEnemy();
                }
            }
        }
        ParticlesVFX.OneShotExplosion?.Invoke(transform, _vfxType);
        AudioManager.Instance.PlayAudioOneShot(_explodeSFX);
        BombExplosionEvent?.Invoke();
        gameObject.SetActive(false);
    }
}
