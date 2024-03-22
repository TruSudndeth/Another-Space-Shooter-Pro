using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

//Todo: Change Enemy laser Transform tags to EnemyLaser and player transform laser to PlayerLaser
//Debug: Enemy ship Bug, some how speed is reduced to a crawl, not able to find reason.
public class Enemy_Move : MonoBehaviour
{
    private readonly float _cameraAspecRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Camera _camera;
    private Vector2 _xyBounds;
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _avoidSpeed = 10.0f;
    private bool _move = false;

    [Space]
    [SerializeField] private float _shiftSpeedMin = 0.5f;
    [SerializeField] private float _ShiftSpeedMax = 2.0f;
    private float _shiftSpeed = 0.5f;
    private float _shiftProbability = 0.25f;
    private bool _isShifting = false;
    private float _randomShiftLocation;

    [Space]
    [SerializeField] private Types.Enemy _enemyType = Types.Enemy.Default;
    private Transform _trackPlayer;

    [Space]
    [SerializeField] private EnemyShoots _Eshoots;

    [Space(25)]
    [SerializeField] private float _aggressiveCrash = 5.0f;
    [SerializeField] private bool _isAggressive = false;
    [SerializeField][Range(0.0f, 1.0f)] private float _aggressiveProbability = 0.25f;
    [SerializeField][Range(0.0f, 1.0f)] private float _trackerSpeedReduction = 0.25f;
    [SerializeField] private Transform _laserManager;
    private bool _aggressiveTracking = false;
    private bool _avoidShots = false;
    private bool _homingTagged = false;
    public bool HomingTagged { get { return _homingTagged; } private set {} }
    private void Awake()
    {
        _camera = Camera.main;
        _yBounds = _camera.orthographicSize;
        _xBounds = _yBounds * _cameraAspecRatio;
        _xyBounds = new Vector2(_xBounds, _yBounds);
    }
    private float _masterDifficulty = 0;
    private void Start()
    {
        GameManager.MasterDifficulty += (x) => { _masterDifficulty = x; };
        GameManager.NewDifficulty += (x) => AdjustDifficulty(x);
        BackGroundMusic_Events.BGM_Events += () => _isShifting = !_isShifting;
        if (_masterDifficulty == 0)
        {
            Debug.Log("Throw event error. difficulty", transform);
            _masterDifficulty = (float) GameManager.Instance.SetMainDifficulty;
        }
        _currentDifficulty = _masterDifficulty;
        if (!_laserManager)
        {
            _laserManager = GameObject.Find("LaserManager").transform;
        }
        if (transform.TryGetComponent(out EnemyShoots enemyShoots))
            _Eshoots = enemyShoots;
    }
    //Complete: add spawn anticipation Delay Move() function for a range of human reaction 0.125f - 0.5f
    private float _spawnAnticipation_MS = 0;
    private float _currentDifficulty = 0;
    private float _anticipationTime = 0;
    //Complete: Slow down enemies with 4 being current and 0 being a % slower.
    private float _enemySpeedAdjustment = 0.50f;
    //Todo: adjust laser attack probability to player but not to collectables.
    //Todo: difficulty curve adjust Eplayers speed all move towards player.
    //Todo: limit number of enemies on screen on easy and slow.
    void FixedUpdate()
    {
        if (_move)
        {
            if(_anticipationTime + _spawnAnticipation_MS < Time.time)
            {
                Move();
                if (_enemyType == Types.Enemy.Scifi_Drone_04)
                    TrackPlayer();
            }
        }
    }
    private int _maxDifficulty = GameConstants.World.MaxDifficulty;
    private float _humanReactMax = GameConstants.Player.HumanReactionMax;
    private float _humanReactMin = GameConstants.Player.HumanReactionMin;
    private float SpawnAnticipation(float setDifficulty)
    {
        _currentDifficulty = setDifficulty;
        float invertDifficulty = _maxDifficulty - _currentDifficulty;
        _spawnAnticipation_MS = MathFunctionsHelper.Map(invertDifficulty, 0, _maxDifficulty, _humanReactMax, _humanReactMin);
        //Debug.Log("Anticipation = " + _spawnAnticipation_MS + " and difficulty is " + _currentDifficulty);
        return _spawnAnticipation_MS;
    }
    private void MoveDifficulty(float difficulty)
    {
        difficulty = _maxDifficulty - difficulty;
        _enemySpeedAdjustment = MathFunctionsHelper.Map(difficulty, 0, 4, 50, 100);
        _enemySpeedAdjustment *= 0.01f;
        //Debug.Log("enemy Adjustment = " + _enemySpeedAdjustment);
    }
    private void AdjustDifficulty(float difficulty)
    {
        _currentDifficulty = difficulty;
        SpawnAnticipation(difficulty);
        MoveDifficulty(difficulty);
    }
    private void TrackPlayer()
    {
        if (!_trackPlayer)
        {
            _trackPlayer = GameObject.FindGameObjectWithTag(Types.Tag.Player.ToString()).transform;
        }
        else
        {
            if (_enemyType == Types.Enemy.Scifi_Drone_04)
            {
                _randomShiftLocation = Vector2.Dot(Vector2.left, transform.position - _trackPlayer.position);
            }
        }
    }
    private void Move()
    {
        float fixedTime = Time.fixedDeltaTime;
        if (DistanceFromPlayer())
        {
            float adjustSpeed = _speed;
            transform.position = Vector3.MoveTowards(transform.position, _trackPlayer.position, adjustSpeed * fixedTime);
            return;
        }
        Vector3 movePlayer = (_speed * fixedTime) * Vector3.down;
        Vector3 shiftPlayer = ShiftWithBPM(fixedTime);
        if (_isAggressive)
        {
            movePlayer *= _trackerSpeedReduction;
            movePlayer.x = 0;
        }
        if (!_isShifting && _enemyType == Types.Enemy.Scifi_Drone_04 && !_isAggressive) movePlayer.y *= 0.01f;
        //Todo: should _avoidShots be added to a difficulty curve?
        if (_avoidShots)
        {
            movePlayer = AvoidShots(movePlayer);
        }
        movePlayer = OutOfBounds.CalculateMove(transform, movePlayer + shiftPlayer, _xyBounds);

        if (movePlayer.y == 0) gameObject.SetActive(false);
        transform.position += movePlayer;
    }
    private bool DistanceFromPlayer()
    {
        if (_trackPlayer && _isAggressive)
        {
            if ((transform.position - _trackPlayer.position).sqrMagnitude < _aggressiveCrash)
            {
                _aggressiveTracking = true;
                return true;
            }
            else
                return false;
        } else
            return false;
    }
    //complete: only allow one laser avoid or a probability of 2
    private Transform _avoidClosestLaser = null;
    private Vector2 AvoidShots(Vector2 movement)
    {
        if(_avoidClosestLaser != null)
        {
            if(_avoidClosestLaser.gameObject.activeSelf == false)
            {
                _avoidClosestLaser = null;
                if (Random.Range(0, 100) < 75) _avoidShots = false;
                return movement;
            }
        }
        Transform[] activelasers = _laserManager.GetComponentsInChildren<Transform>().Where(x => x.CompareTag(Types.LaserTag.PlayerLaser.ToString())).Where(x => x.gameObject.activeSelf).ToArray();
        if (activelasers.Count() > 0)
        {
            _avoidClosestLaser = GetClosestLaser();
            if (Mathf.Abs(_avoidClosestLaser.position.y - transform.position.y) < 1)
            {
                movement.x = Mathf.Sign(_avoidClosestLaser.position.x - transform.position.x) * -1;
                movement.x *= _avoidSpeed * Time.fixedDeltaTime;
            }
            return movement;
        }
        else
            return movement;
    }
    private Transform GetClosestLaser()
    {
        Transform closestLaser = null;
        float closestDistance = 0;
            foreach (Transform shot in _laserManager)
            {
                if (shot.gameObject.activeSelf)
                {
                    if (shot.CompareTag(Types.LaserTag.PlayerLaser.ToString()))
                    {
                        float distance = (transform.position - shot.position).sqrMagnitude;
                        if (closestLaser == null)
                        {
                            closestLaser = shot;
                            closestDistance = distance;
                        }
                        else
                        {
                            if (distance < closestDistance)
                            {
                                closestLaser = shot;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }
        return closestLaser;
    }
    private Vector3 ShiftWithBPM(float fixedTime)
    {
        if (_isShifting || _aggressiveTracking)
        {
            Vector3 shiftPlayer = _shiftSpeed * fixedTime * Mathf.Sign(_randomShiftLocation) * (Vector3.right);
            return shiftPlayer;
        }
        else
            return Vector3.zero;
    }
    
    private void NextWave()
    {
        //LeftOff: Next wave to reset all used asssets
        if (_enemyType != Types.Enemy.Scifi_Drone_04)
            return;
        float aggressiveProbability = Random.Range(0.0f, 1.0f);
        if(aggressiveProbability <= _aggressiveProbability)
        {
            _isAggressive = true;
            _aggressiveTracking = true;
        }
        else
        {
            _isAggressive = false;
            _aggressiveTracking = false;
        }
    }
    public void HasBeenTaggedByLaser()
    {
        _homingTagged = true;
    }
    
    private void OnEnable()
    {
        //Debug: might want to move even listeners to on enable rather than start.
        //Debug: performance, might be tax heavy with reg and unreg
        AdjustDifficulty(_currentDifficulty);
        
        _anticipationTime = Time.time;
        _avoidShots = Random.Range(0, 101) < 35;
        _shiftProbability = Random.Range(0.0f, 1.0f);
        _shiftSpeed = Random.Range(_shiftSpeedMin, _ShiftSpeedMax);// * _enemySpeedAdjustment;
        //Todo: Add a zero probability. (straight)
        _randomShiftLocation = Random.Range(0.0f, 1.0f) <= _shiftProbability ? Random.Range(-_xBounds, _xBounds) :
                _randomShiftLocation;
        _randomShiftLocation = transform.position.x;
        _move = true;
        NextWave();
    }

    private void OnDisable()
    {
        GameManager.MasterDifficulty -= (x) => { _masterDifficulty = x; };
        GameManager.NewDifficulty -= (x) => AdjustDifficulty(x);
        BackGroundMusic_Events.BGM_Events -= () => _isShifting = !_isShifting;
        transform.position = Vector3.zero;
        _move = false;
        _homingTagged = false;
    }
}
