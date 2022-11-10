using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Todo: Change Enemy laser Transform tags to EnemyLaser and player transform laser to PlayerLaser
public class Enemy_Move : MonoBehaviour
{
    private readonly float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Camera _camera;
    private Vector2 _xyBounds;
    [SerializeField] private float _speed = 4.0f;
    private bool _move = false;

    [Space]
    [SerializeField] private float _shiftSpeedMin = 0.5f;
    [SerializeField] private float _ShiftSpeedMax = 2.0f;
    private float _shiftSpeed = 0.5f;
    private float _shiftProbability = 0.25f;
    private bool _isShifting = false;
    private float _randomShiftLocation;

    //Todo: Eplayer behaviour to move towards player
    //Todo: Eplayers All move Right all move left
    //Todo: Eplayers all move towards player
    private void Awake()
    {
        _camera = Camera.main;
        _yBounds = _camera.orthographicSize;
        _xBounds = _yBounds * _cameraAspecRatio;
        _xyBounds = new Vector2(_xBounds, _yBounds);
    }
    private void Start()
    {
        BackGroundMusic_Events.BGM_Events += () => _isShifting = !_isShifting;
    }
    void FixedUpdate()
    {
        if (_move)
        {
            float fixedTime = Time.fixedDeltaTime;
            Vector3 movePlayer = _speed * fixedTime * Vector3.down;

            Vector3 shiftPlayer = ShiftWithBPM(fixedTime);
            
            movePlayer = OutOfBounds.CalculateMove(transform, movePlayer + shiftPlayer, _xyBounds);
            if (movePlayer.y == 0) gameObject.SetActive(false);
            transform.position += movePlayer;
        }
    }
    private Vector3 ShiftWithBPM(float fixedTime)
    {
        if (_isShifting)
        {
            Vector3 shiftPlayer = _shiftSpeed * fixedTime * Mathf.Sign(_randomShiftLocation) * (Vector3.right);
            return shiftPlayer;
        }
        else
            return Vector3.zero;
    }
    private void OnEnable()
    {
        _shiftProbability = Random.Range(0.0f, 1.0f);
        _shiftSpeed = Random.Range(_shiftSpeedMin, _ShiftSpeedMax);
        //Todo: Add a zero probability. (straight)
        _randomShiftLocation = Random.Range(0.0f, 1.0f) <= _shiftProbability ? Random.Range(-_xBounds, _xBounds) :
                _randomShiftLocation;
        _randomShiftLocation = transform.position.x;
        _move = true;
    }

    private void OnDisable()
    {
        BackGroundMusic_Events.BGM_Events -= () => _isShifting = !_isShifting;
        transform.position = Vector3.zero;
        _move = false;
    }
}
