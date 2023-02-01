using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private List<Types.SFX> _sfx;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Types.LaserTag.EnemyLaser.ToString()))
        {
            other.transform.parent.gameObject.SetActive(false);
            GetComponent<Player>().Health = 1;
            if(_sfx.Count > 0)
            AudioManager.Instance.PlayAudioOneShot(RandomDamageSFX());
        }
    }

    private Types.SFX RandomDamageSFX()
    {
        int selectSound = Random.Range(0, _sfx.Count);
        return _sfx[selectSound];
    }
        
}
