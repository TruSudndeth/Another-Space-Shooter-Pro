using UnityEngine;
using System.Linq;

//Colmplete: Change Enemy laser Transform tags to EnemyLaser and player transform laser to PlayerLaser
//Debug: Enemy ship Bug, some how speed is reduced to a crawl, not able to find reason.
//Complete: Enemy Easy mode ships don't move towards player if anything only a probability of ships do.
//Complete: Enemy Move, dodge shots reduce probability
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
    private float _randomShiftDirection;

    [Space]
    [SerializeField] private Types.Enemy _enemyType = Types.Enemy.Default;
    private Transform _trackPlayer;

    [Space]
    [SerializeField] private EnemyShoots _Eshoots;

    [Space(25)]
    [SerializeField] private float _aggressiveCrash = 5.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float _aggressiveProbability = 0.25f;
    [SerializeField][Range(0.0f, 1.0f)] private float _trackerSpeedReduction = 0.25f;
    [SerializeField] private Transform _laserManager;

    [Space]
    [SerializeField] private bool _isAggressive = false;
    [SerializeField] private bool _avoidShots = false;
    [SerializeField] private bool _isBPMove = false;
    [SerializeField] private bool _canTrackPlayer = false;
    [SerializeField] private bool _isShifiting = false;


    //Fix: isShifingBPM is pulsed
    [SerializeField] private bool _moveWithBPM = false;

    private bool _aggressiveTracking = false;
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
    private Quaternion _masterRotation;
    private void Start()
    {
        _masterRotation = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();
        if ( _rigidbody == null )
            Debug.Log("Failed to get Rigidbody component!");

        GameManager.MasterDifficulty += (x) => { _masterDifficulty = x; };
        GameManager.NewDifficulty += (x) => AdjustDifficulty(x);
        BackGroundMusic_Events.BGM_Events += () => _moveWithBPM = !_moveWithBPM;
        if (_masterDifficulty == 0)
        {
            //Debug.Log("Throw event error. difficulty", transform);
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
    //Complete: adjust laser attack probability to player but not to collectables.
    
    //Todo: difficulty curve adjust Eplayers speed all move towards player.
    void FixedUpdate()
    {
        if (_move)
        {
            if(_anticipationTime + _spawnAnticipation_MS < Time.time)
            {
                Move();
                if (_canTrackPlayer && _enemyType == Types.Enemy.Scifi_Drone_04)
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
                _randomShiftDirection = Vector2.Dot(Vector2.left, transform.position - _trackPlayer.position);
            }
        }
    }
    private void Move()
    {   //Todo: Enemy Move, Turn moves into bools add a straight move. and add a difficulty curve
        //Enemy_Move: Aggressive (Target locked)
        float fixedTime = Time.fixedDeltaTime;
        if (DistanceFromPlayer())
        {
            float adjustSpeed = _speed; //Todo: EMove Adjust speed based on difficulty
            transform.position = Vector3.MoveTowards(transform.position, _trackPlayer.position, adjustSpeed * fixedTime);
            return;
        }
        //Enemy_Move: Aggressive (Target not locked)
        Vector3 movePlayer = (_speed * fixedTime) * Vector3.down; //Todo: do this only when on easy
        //Enemy_Move: is shifting
        Vector3 shiftPlayer = MoveWithBPM(fixedTime);
        if (_isAggressive)
        {
            movePlayer *= _trackerSpeedReduction;
            movePlayer.x = 0;
        }
        //Enemy_Move: Shifiting
        if (_isBPMove && !_moveWithBPM && _enemyType == Types.Enemy.Scifi_Drone_04 && !_isAggressive) movePlayer.y *= 0.01f;
        //Todo: should _avoidShots be added to a difficulty curve?
        //Enemy_Move: Avoid Shots
        if (_avoidShots)
        {
            movePlayer = AvoidShots(movePlayer);
        }
        //EnemyMove: Move Straight

        //Enemy_Move: Finish move
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
    //Todo: add a Range for lasers, note: avoid might be to slow with small ranges
    //Todo: add a Range for aggressive drones, when far just drop down slow.
    //Todo: Enemy_Move Fix avoid shot probability.
    //Todo: Enemy_Move Avoid Shots VFX and SFX
    private Transform _avoidClosestLaser = null;
    private Vector2 AvoidShots(Vector2 movement)
    {
        if(_avoidClosestLaser != null)
        {
            if(_avoidClosestLaser.gameObject.activeSelf == false)
            {
                _avoidClosestLaser = null;
                if (Random.Range(0, 100) < 90 - Mathf.RoundToInt(10 * _currentDifficulty)) 
                    _avoidShots = false;
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
    private Vector3 MoveWithBPM(float fixedTime)
    {
        if (_canTrackPlayer || (_isShifiting && (_moveWithBPM || _aggressiveTracking)))
        {
            Vector3 shiftPlayer = _shiftSpeed * fixedTime * Mathf.Sign(_randomShiftDirection) * (Vector3.right);
            return shiftPlayer;
        }
        else
            return Vector3.zero;
    }
    private float _currentAggressiveProbability = 0;
    private bool NextWave()
    {
        //LeftOff: Next wave to reset all used asssets
        if (_enemyType != Types.Enemy.Scifi_Drone_04)
            return false;
        float aggressiveProbability = Random.Range(0.0f, 1.0f);
        if(aggressiveProbability <= _currentAggressiveProbability)
        {
            _aggressiveTracking = true;
            _canTrackPlayer = true;
            _isAggressive = true;
        }
        else
        {
            _aggressiveTracking = false;
            _canTrackPlayer = false;
            _isAggressive = false;
        }
        return _isAggressive;
    }
    public void HasBeenTaggedByLaser()
    {
        _homingTagged = true;
    }
    private Rigidbody _rigidbody;
    private void OnEnable()
    {
        //Debug: might want to move even listeners to on enable rather than start.
        //Debug: performance, might be tax heavy with reg and unreg
        AdjustDifficulty(_currentDifficulty);

        //Difficulty settings _isAggressive, _avoidShots, _isBPMove, _canTrckPlayer, _isShifting
        //00000 = straight, 00001 = Random direction, 
        //masterDifficulty + 1 == current difficulty
        //LeftOff: Setting up Difficulty from easy to hard.
        //Bug: Enemy_Move is _aggresive is too fast
        int masterDifficulty = Mathf.RoundToInt(_masterDifficulty) == 0 ? 1 : Mathf.RoundToInt(_masterDifficulty);
        _currentAggressiveProbability = Random.Range(0.0f, _aggressiveProbability);
        _isAggressive = NextWave(); //Todo: Balance Aggressive probability Skip if one already exist easy - hard
        _canTrackPlayer = _canTrackPlayer || Random.Range(0, 101) < 10 * masterDifficulty;
        _avoidShots = Random.Range(0, 101) < 10 * masterDifficulty;
        _isBPMove = Random.Range(0,101) < 10 * masterDifficulty; //Todo: Set range based on difficulty
        _isShifiting = Random.Range(0, 101) < 10 * masterDifficulty; ;

        if(_currentDifficulty >= masterDifficulty + 0.5f) //HardCode: hard coded 0.5f
        {
            int mDifficultSquared = Mathf.RoundToInt(_currentDifficulty * _currentDifficulty);
            _isShifiting = _isShifiting || Random.Range(0, 101) < 10 * (mDifficultSquared);
            _randomShiftDirection = Random.Range(-_xBounds, _xBounds);
        }

        _anticipationTime = Time.time;
        _shiftSpeed = Random.Range(_shiftSpeedMin, _ShiftSpeedMax);// * _enemySpeedAdjustment;
        //_randomShiftLocation = transform.position.x;
        _move = true;
        if (_rigidbody != null) DoRigidBodyStuff();
    }
    private void DoRigidBodyStuff()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        GameManager.MasterDifficulty -= (x) => { _masterDifficulty = x; };
        GameManager.NewDifficulty -= (x) => AdjustDifficulty(x);
        BackGroundMusic_Events.BGM_Events -= () => _moveWithBPM = !_moveWithBPM;
        transform.position = Vector3.zero;
        _move = false;
        _homingTagged = false;
    }
}
