using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public static LaserManager Instance { get; private set; }
    [SerializeField] private Material _enemyLaser;
    [SerializeField] private Material _playerLaser;
    [Space]
    [SerializeField] private Transform _laserAsset;
    [SerializeField] private int _maxPool = 15;
    [SerializeField] private int _iterateLaser = 0; //Debugit: remove serialized field
    
    private bool _isPoolMaxed = false;
    private List<Transform> _lasers;

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
        _lasers = new List<Transform>(_maxPool);
    }

    public void LaserPool(Transform laserTransform)
    {
        Vector3 position = laserTransform.position;
        if (_lasers.Count < _maxPool && !_isPoolMaxed)
        {
            _lasers.Add(Instantiate(_laserAsset, position, laserTransform.rotation, transform));
            SetAll(laserTransform, _lasers.LastOrDefault());

            _iterateLaser++;
            if (_iterateLaser == _maxPool)
            {
                _iterateLaser = 0;
                _isPoolMaxed = true;
            }
        }
        else if (_isPoolMaxed)
        {
            //fire rate must not surpass laser pool check if object is disabled before using.
            //Lock rotations add recochet later
            for (int i = 0; i < _lasers.Count; i++)
            {
                if (!_lasers[i].gameObject.activeSelf)
                {
                    SetAll(laserTransform, _lasers[i]);
                    return;
                }
            }
            _lasers.Add(Instantiate(_laserAsset, position, laserTransform.rotation, transform));
            SetAll(laserTransform, _lasers.LastOrDefault());
        }
    }
    
    private void SetAll(Transform caller, Transform laserInPool)
    {
        laserInPool.gameObject.SetActive(true);
        if (laserInPool.TryGetComponent(out LaserBehavior laserBehavior))
        {
            laserBehavior.transform.position = caller.position;
            laserBehavior.transform.rotation = caller.rotation;
            laserBehavior.SetMaterial(SetMaterial(caller));
            laserBehavior.SetTag(SetTag(caller));
        }
    }

    private Material SetMaterial(Transform laserMaterial)
    {
        Material material = _playerLaser;
        if (laserMaterial.CompareTag(Types.Tag.Enemies.ToString()))
            material = _enemyLaser;
        return material;
    }
    private Types.Tag SetTag(Transform laserTag)
    {
        Types.Tag tag = Types.Tag.Player;
        if (laserTag.CompareTag(Types.Tag.Enemies.ToString()))
            tag = Types.Tag.Enemies;
        return tag;
    }
}
