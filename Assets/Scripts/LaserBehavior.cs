using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LaserBehavior : MonoBehaviour
{
    private Renderer _renderer;
    private Camera _camera;
    private float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Vector2 _xyBounds;
    private bool _homingLaser = false;
    private Transform target;
    [Space]
    [SerializeField] private float _speed = 10;
    private bool _move = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
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
        if (tag == Types.Tag.Player)
            gameObject.tag = Types.LaserTag.PlayerLaser.ToString();
        else if (tag == Types.Tag.Enemies)
            gameObject.tag = Types.LaserTag.EnemyLaser.ToString(); //Debug: Enemy is not defined
    }
    public void SetHoming(bool SetHoming)
    {
        _homingLaser = SetHoming;
    }
    private void OnDisable()
    {
        transform.position = Vector3.zero;
        _move = false;
        _homingLaser = false;
    }
    void FixedUpdate()
    {
        if (_move && !_homingLaser)
        {
            Vector3 checkForBounds;
            if (_homingLaser && target == null)
            {
                target = TargetClosestEnemy();
                if (target.TryGetComponent(out Enemy_Move enemy))
                    enemy.HasBeenTaggedByLaser();
                if (!target)
                {
                    Debug.Log("Laser cant find Target");
                    return;
                }
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 180 * Time.fixedDeltaTime);
            }
            Vector3 moveLaser = _speed * Time.fixedDeltaTime * transform.up;
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
        Transform[] EnemyManager = GameObject.Find("EnemySpwanManager").GetComponentsInChildren<Transform>();
        Transform[] Enemies = EnemyManager.Where(x => x.tag == Types.Tag.Enemies.ToString()).Where(x => x.gameObject.activeSelf).Where(x => x.GetComponent<Enemy_Move>().HomingTagged == false).ToArray();
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
