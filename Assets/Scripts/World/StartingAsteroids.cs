using UnityEngine;

public class StartingAsteroids : MonoBehaviour
{    
    [SerializeField] private Types.SFX _sfxType;
    [SerializeField] private Types.Tag _tag = Types.Tag.Astroids;
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
                //playerInput.DamagedByType = _tag;
                //playerInput.Health = 1;
                playerInput.Damage(_tag, 1);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag(Types.LaserTag.PlayerLaser.ToString()))
        {
            other.transform.parent.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out ParticlesVFX _vfx))
        {
            //Debug: StartingAstroids OnDestroy on editor Stop
            _vfx.PlayVFX();
            if (AudioManager.Instance == null) 
                Debug.Log("AudioManager.instance is null ");
            else
                AudioManager.Instance.PlayAudioOneShot(_sfxType);
        }
    }
}
