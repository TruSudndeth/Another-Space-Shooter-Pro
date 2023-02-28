using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticlesVFXManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _explosionsAssets;
    private List<ExplosionType> _particleExplode;

    private void Start()
    {
        _particleExplode = new();
        ParticlesVFX.OneShotExplosion += PlayOneShotExplosion;
        BossExplosions.OneShotExplosion += PlayOneShotExplosion;
    }

    private void PlayOneShotExplosion(Transform unitsTransform, Types.VFX unitVFXType)
    {
        if (_particleExplode.Count > 0) //Initial
        {
            for (int i = 0; i < _particleExplode.Count; i++) //loop
            {
                if (_particleExplode[i].Type == unitVFXType) //check if explosion type exist Player, Enemy, Astroid
                    if (!_particleExplode[i].Explosion.activeSelf) //only use disabled explosions
                    {
                        //Set Pool Explosion Position and Active (Animation runs when active auto)
                        _particleExplode[i].Explosion.transform.position = unitsTransform.position;
                        _particleExplode[i].Explosion.gameObject.SetActive(true);
                        return; // only return if we found a disabled explosion in pool
                    }
            }
            //this will only run if we did not find a disabled explosion in pool because of the return above
            GameObject thisExplode = Instantiate(CheckExplosionList(unitVFXType), unitsTransform.position, Quaternion.identity, transform);
            _particleExplode.Add(new ExplosionType(thisExplode, unitVFXType));
        }
        else
        {
            //Initial Explosion (index 0)
            GameObject thisExplode = Instantiate(CheckExplosionList(unitVFXType), unitsTransform.position, Quaternion.identity, transform);
            _particleExplode.Add(new ExplosionType(thisExplode, unitVFXType));
        }
    }

    private GameObject CheckExplosionList(Types.VFX _vfx)
    {
        int explosionType;
        switch (_vfx)
        {
            case Types.VFX.PlayerDeath:
                explosionType = 0;
                break;
            case Types.VFX.EnemyDeath:
                explosionType = 1;
                break;
            case Types.VFX.MiniBossDeath:
                explosionType = 2;
                break;
            case Types.VFX.BossDeath:
                explosionType = 3;
                break;
            default:
                explosionType = 0;
                break;
        }
        if (explosionType > _explosionsAssets.Count - 1 && _explosionsAssets.Count -1 != -1)
            return _explosionsAssets[0];
        else
            return _explosionsAssets[explosionType];
    }
    private void OnDisable()
    {
        ParticlesVFX.OneShotExplosion -= PlayOneShotExplosion;
        BossExplosions.OneShotExplosion -= PlayOneShotExplosion;
    }
}
