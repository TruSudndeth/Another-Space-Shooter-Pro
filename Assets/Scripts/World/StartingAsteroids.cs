using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingAsteroids : MonoBehaviour
{    
    [SerializeField] private Types.SFX _sfxType;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Types.Tag.Player.ToString()))
        {
            if(other.TryGetComponent(out Player playerInput))
            {
                if (transform.parent.TryGetComponent(out StartGameAsteroids startingGameAsteroids))
                {
                    startingGameAsteroids.SetDificulty();
                }
                playerInput.Health = 1;
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag(Types.LaserTag.PlayerLaser.ToString()))
        {
            other.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out ParticlesVFX _vfx))
        {
            _vfx.PlayVFX();
            AudioManager.Instance.PlayAudioOneShot(_sfxType);
        }
    }
}
