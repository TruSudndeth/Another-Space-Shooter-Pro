using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionType
{
    private Types.VFX _type;
    private GameObject _explosion;
    public Types.VFX Type { get => _type; set => _type = value; }
    public GameObject Explosion { get => _explosion; set => _explosion = value; }

    public ExplosionType(GameObject _transform, Types.VFX _vfx)
    {
        _type = _vfx;
        _explosion = _transform.gameObject;
    }
}
