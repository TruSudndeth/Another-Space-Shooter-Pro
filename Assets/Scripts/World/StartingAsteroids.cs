using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingAsteroids : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Type.CollisionTags.Player.ToString()))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag(Type.CollisionTags.Laser.ToString()))
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
        }
    }
}