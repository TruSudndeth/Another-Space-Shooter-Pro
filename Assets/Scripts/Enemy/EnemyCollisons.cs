using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisons : MonoBehaviour
{
    //create a delegate for the event of enemyPoints
    public delegate void EnemyPoints(int points, string enemyName);
    public static event EnemyPoints EnemyPointsEvent;
    
    [SerializeField] private Types.SFX _sfxType;
    [SerializeField] private Types.Points _enemyPointValue;
    [SerializeField] private Transform _shield;
    [SerializeField] [Range(1,100)] private int _shieldProbability = 25;
    private bool _hasChildren = false;
    private int _life = 1;

    private void Awake()
    {
        //Todo: dirty and a temp fix
        _hasChildren = transform.parent.childCount > 0 && transform.parent.name != "EnemySpwanManager";
        _life = Random.Range(0.0f, 100.0f) < _shieldProbability ? 2 : 1;
        if (_life > 1) _shield.gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(Types.LaserTag.PlayerLaser.ToString()))
        {
            other.gameObject.SetActive(false);
            if (_life > 1)
            {
                DisableShield();
                return;
            }
            EnemyPointsEvent?.Invoke((int)_enemyPointValue, transform.name);
            DisableParent();
        }
        else if(other.CompareTag(Types.Tag.Player.ToString()))
        {
            if(other.TryGetComponent(out Player _input))
            {
                _input.Health = 1;
                if (_life > 1)
                {
                    DisableShield();
                    return;
                }
                EnemyPointsEvent?.Invoke(0, transform.name);
                DisableParent();
            }
        }
    }
    private void DisableShield()
    {
        _life--;
        _shield.gameObject.SetActive(false);
    }
    public void DestroyEnemy()
    {
        EnemyPointsEvent?.Invoke((int)_enemyPointValue, transform.name);
        DisableParent();
    }
    private void DisableParent()
    {
        AudioManager.Instance.PlayAudioOneShot(_sfxType);
        if (TryGetComponent(out ParticlesVFX _vfx))
        {
            _vfx.PlayVFX();
        }
        if (!_hasChildren)
            gameObject.SetActive(false);
        else if (_hasChildren)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
