using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossExplosions : MonoBehaviour
{
    //LeftOff: Explosion FX
    //Add more explosions to Main engine, wing, body (maybe one multiple explosion at once at the end)
    //only do one explosion of the small thrusters.
    public delegate void Explosion(Transform _transform, Types.VFX _vfx);
    public static Explosion OneShotExplosion;
    [SerializeField]
    private Types.VFX vfxType;
    [SerializeField]
    private MotherShipParts _motherShipPart;
    [SerializeField]
    private Transform[] _locations;
    [SerializeField]
    private float _randomTimeDelay = 0.75f;
    [SerializeField]
    private float _randomTimeDelayRange = 0.5f;

    private bool _hasExploded = false;
    private void Awake()
    {
        _locations = transform.GetComponentsInChildren<Transform>(false).Skip(1).ToArray();
        
    }
    private void Start()
    {
        DamageMaterialFX.onObjectDestroyed += ExplodeObject;
    }
    void Update()
    {
        
    }
    public void ExplodeObject(MotherShipParts part)
    {
        if(!_hasExploded && part == _motherShipPart)
        {
            Debug.Log(part.ToString() + " has exploded", transform);
            float timeDelay = 0;
            _hasExploded = true;
            timeDelay = TimeDelayRange();
            StartCoroutine(ExplodeIteration(timeDelay));
        }
    }
    
    IEnumerator ExplodeIteration(float timeDelay)
    {
        int iterate = 0;
        while (iterate < _locations.Length)
        {
            OneShotExplosion?.Invoke(_locations[iterate], vfxType);
            yield return new WaitForSeconds(timeDelay);
            //play sound as well
            iterate++;
        }
    }

    private float TimeDelayRange()
    {
        return Random.Range(_randomTimeDelay - _randomTimeDelayRange, _randomTimeDelay + _randomTimeDelayRange);
    }
    private void OnDisable()
    {
        DamageMaterialFX.onObjectDestroyed -= ExplodeObject;
    }
}
