using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesVFX : MonoBehaviour
{
    //create an explosion delegate 
    public delegate void Explosion(Transform _transform, Type.VFX _vfx);
    public static Explosion OneShotExplosion;

    [SerializeField] private Type.VFX vfxType;
    public void PlayVFX()
    {
        OneShotExplosion?.Invoke(transform, vfxType);
    }
}
