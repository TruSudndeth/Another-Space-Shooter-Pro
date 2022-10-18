using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionType
{
    private Type.VFX _type;
    private GameObject _explosion;
    public Type.VFX Type { get => _type; set => _type = value; }
    public GameObject Explosion { get => _explosion; set => _explosion = value; }

    public ExplosionType(GameObject _transform, Type.VFX _vfx)
    {
        _type = _vfx;
        _explosion = _transform.gameObject;
    }
}
