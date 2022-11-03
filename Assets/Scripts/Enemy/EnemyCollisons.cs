using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisons : MonoBehaviour
{
    [SerializeField] private Types.SFX _sfxType;
    [SerializeField] private Types.Points _enemyPointValue;
    private bool _hasChildren = false;
    //create a delegate for the event of enemyPoints
    public delegate void EnemyPoints(int points);
    public static event EnemyPoints EnemyPointsEvent;

    private void Awake()
    {
        //This is dirty and a temp fix
        _hasChildren = transform.parent.childCount > 0 && transform.parent.name != "EnemySpwanManager";
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag(Types.LaserTag.PlayerLaser.ToString()))
        {
            EnemyPointsEvent?.Invoke((int)_enemyPointValue);
            other.gameObject.SetActive(false);
            DisableParent();
        }
        else if(other.CompareTag(Types.Tag.Player.ToString()))
        {
            if(other.TryGetComponent(out Player _input))
            {
                _input.Health = 1;
                DisableParent();
            }
        }
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
