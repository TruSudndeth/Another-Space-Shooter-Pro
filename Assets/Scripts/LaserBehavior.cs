using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Melanchall.DryWetMidi.Core;

public class LaserBehavior : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10;
    private Renderer _renderer;
    private Camera _camera;
    private float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Vector2 _xyBounds;
    private bool _homingLaser = false;
    private Transform target;
    private bool tagged = false;
    [Space]
    [SerializeField] private float _speed = 10;
    private bool _move = false;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _camera = Camera.main;
        _yBounds = _camera.orthographicSize;
        _xBounds = _yBounds * _cameraAspecRatio;
        _xyBounds = new Vector2(_xBounds, _yBounds);
    }
    private void OnEnable()
    {
        _move = true;
    }
    public void SetMaterial(Material material)
    {
        _renderer.material = material;
    }
    public void SetTag(Types.Tag tag)
    {
        Types.LaserTag laserTagType;
        if (tag == Types.Tag.Player)
        {
            laserTagType = Types.LaserTag.PlayerLaser;
        }
        else if (tag == Types.Tag.Enemies)
            laserTagType = Types.LaserTag.EnemyLaser; //Debug: Enemy is not defined
        else
        {
            laserTagType = Types.LaserTag.PlayerLaser;
            Debug.Log("laser was not to a Unit, default is player", transform);
        }
        GameObject[] setTreeToTags = gameObject.GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToArray();
        foreach (GameObject child in setTreeToTags)
        {
            child.tag = laserTagType.ToString();
        }
    }
    public void SetHoming(bool SetHoming)
    {
        _homingLaser = SetHoming;
    }
    private void OnDisable()
    {
        tagged = false;
        target = null;
        transform.position = Vector3.zero;
        _move = false;
        _homingLaser = false;
    }
    void FixedUpdate()
    {
        if (_move)
        {
            Vector3 checkForBounds;
            if(target == null)
            target = TargetClosestEnemy();
            if (_homingLaser && target != null)
            {
                Debug.Break();
                if (target)
                {
                    if (!tagged && target.TryGetComponent(out Enemy_Move enemy))
                    {
                        enemy.HasBeenTaggedByLaser();
                        tagged = true;
                    }
                    transform.LookAt(target, transform.right);
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 360 * _rotationSpeed * Time.fixedDeltaTime);
                    Debug.Log("Rotation");
                }
                else
                {
                    Debug.Log("Laser cant find Target", transform);
                    return;
                }
            }
            Vector3 moveLaser = _speed * Time.fixedDeltaTime * transform.forward;
            checkForBounds = OutOfBounds.CalculateMove(transform, moveLaser, _xyBounds);
            if(moveLaser == Vector3.zero || checkForBounds.magnitude > moveLaser.magnitude * 2)
            {
                KillLaser();
                return;
            }
            transform.position += moveLaser;
        }
        else
        {
            
        }
        //Look at target and reset rotation On Disable.
        //Check if laser is already homing enemy
        //and Check if enemy is infront/above the players y position
    }
    private Transform TargetClosestEnemy()
    {
        //if Enemymanager is empty skip everyting that can result in an error
        Transform[] EnemyManager = GameObject.Find("EnemySpwanManager").GetComponentsInChildren<Enemy_Move>().Select(x => x.transform).Where(x => x.gameObject.activeSelf).ToArray();
        if (EnemyManager.Length == 0) return null;
        Transform[] Enemies = EnemyManager.Where(x => x.CompareTag(Types.Tag.Enemies.ToString())).Where(x => x.GetComponent<Enemy_Move>().HomingTagged == false).ToArray();
        if (Enemies.Length == 0) return null;
        Transform closestEnemy = null;
        float distance = Mathf.Infinity;
        if (Enemies.Length == 0)
        {
            return null;
        }else
        foreach (Transform Closest in Enemies)
        {
                float distance2 = Vector3.Distance(transform.position, Closest.position);
                if (distance2 < distance)
                {
                    distance = distance2;
                    closestEnemy = Closest;
                }
        }
        return closestEnemy;
    }
    private void KillLaser()
    {
        transform.gameObject.SetActive(false);
    }

}
