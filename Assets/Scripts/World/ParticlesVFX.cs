using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesVFX : MonoBehaviour
{
    public delegate void Explosion(Transform _transform, Types.VFX _vfx);
    public static Explosion OneShotExplosion;
    
    [SerializeField] private Types.VFX vfxType;
    public void PlayVFX()
    {
        OneShotExplosion?.Invoke(transform, vfxType);
    }
}
