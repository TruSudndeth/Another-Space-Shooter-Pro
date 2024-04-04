using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossFightManager : DontDestroyHelper<BossFightManager>
{
    //Todo: Fix Boss Battle Beacken Shield Function

    //Bug: Boss moves to next wave before the wave is even complete. move boss when wave is complete only. 
    //Todo: Fix Boss Parts Explotion sound FX
    //public static BossFightManager Instance;

    //Delete: event this is not used ????
    //public delegate void BossFightStage();
    //public static event BossFightStage OnBossFightStage;
    public delegate void SpawnWaves(bool states);
    public static event SpawnWaves PauseWaves;
    public static event SpawnWaves ContinueWaves;
    public static event SpawnWaves ActiveMother;

    public delegate void EnableStageCollider(BossParts part);
    public static event EnableStageCollider EnableStageColliderPart;

    public delegate void ResetBoss();
    public static event ResetBoss ResetBossEvent;

    public delegate void MotherShipDestroyed(int adjustBy);
    public static event MotherShipDestroyed SetDifficulty;

    [Space(20)]
    [Header("Boss Stage Collider Setup")]
    [SerializeField]
    private List<BossParts> _stage1Parts;
    [SerializeField]
    private List<BossParts> _stage2Parts;
    [SerializeField]
    private List<BossParts> _stage3Parts;
    private List<List<BossParts>> _stages;
    

    [SerializeField]
    private int _damagePerPart = 3;
    public int DamagePerPart {  get { return _damagePerPart; } private set {  _damagePerPart = value; } }
    [SerializeField]
    private int _motherShipsDestroyed = 0;

    [Space(20)]
    [Header("Prefab Setup")]
    [SerializeField]
    private Transform _MotherShip;
    [SerializeField]
    private Transform _positions;

    [Space(20)]
    [Header("Boss Setup")]
    [SerializeField]
    private float _bossSpeed = 1.0f;

    [Space]
    private int _positionIndex = 0;
    private Vector3 _nextPosition;
    private Transform[] _positionSequence;

    private bool _hasStarted = false;
    private bool _beacon = false;
    private bool _body = false;
    private bool _leftEngine = false;
    private bool _leftWing = false;
    private bool _sM_Thrust_01 = false;
    private bool _sM_Thrust_02 = false;
    private bool _stage01 = false;
    private bool _stage02 = false;
    private bool _stage03 = false;

    [Header("Boss Fight Intervals")]
    [Space(20)]
    [SerializeField]
    private int _bossFightInterval = 5;
    private int _waveCount = 1;
    private int _maxIntValue;
    private bool _moveComplete = false;
    private bool _BossDestroyed = false;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        _maxIntValue = int.MaxValue;
        _stages = new List<List<BossParts>> { _stage1Parts, _stage2Parts, _stage3Parts };
        _positionSequence = _positions.GetComponentsInChildren<Transform>(false).Skip(1).ToArray();
        BossSetup();
    }
    void Start()
    {
        BossExplosions.OnDisablePart += RegisterPartDestroid;
        EnemySpawnManager.NewWaveEvent += BossFightIntervals;
        UIManager.ResetLevel += ResetGame;

        GameManager.MasterDifficulty += (x) => { _currentDifficulty = x; };
        GameManager.NewDifficulty += (x) => { _currentDifficulty = x; };
    }
    public void StartBossFight()
    {
        if (!_hasStarted)
        {
            SetDifficultyCurve();
            //Pause Waves when ship rolls in
            PauseWaves?.Invoke(true);
            //Other stuff
            EnableStageColliders(_stage1Parts);
            _hasStarted = true;
            ActiveMother?.Invoke(true);
            _positionIndex = 0;
            if (!_MotherShip.gameObject.activeSelf)
            {
                _MotherShip.gameObject.SetActive(true);
            }
            _nextPosition = _positionSequence[_positionIndex].position;
            _MotherShip.position = _nextPosition;
            MoveBossToNextPosition();
        }
    }
    private void BossSetup()
    {
        _nextPosition = _positionSequence[_positionIndex].position;
        _MotherShip.position = _nextPosition;
    }
    private void BossFightIntervals()
    {
        _bossFightInterval = _bossFightInterval == 0 ? 1 : _bossFightInterval;
        // Debug.Log("Somehow entered here at reset " + _waveCount + " INTERVAL " + _bossFightInterval + " " + _waveCount % _bossFightInterval);
        if (_waveCount % _bossFightInterval == 0)
        {
            // Debug.Log("inside remainder " + _waveCount + " INTERVAL " + _bossFightInterval + " " + _waveCount % _bossFightInterval);
            if (_waveCount >= _maxIntValue)
            {
                _waveCount = 1;
            }
            StartBossFight();
        }
        _waveCount++;
    }
    private void EnableStageColliders(List<BossParts> parts)
    {
        foreach (BossParts part in parts)
        {
            EnableStageColliderPart?.Invoke(part);
        }
    }
    private void ResetGame()
    {
        _waveCount = 1;
        _motherShipsDestroyed = 0;
        ResetBossFight();
    }
    private void ResetBossFight()
    {
        _positionIndex = 0;
        _BossDestroyed = false;
        _hasStarted = false;
        ActiveMother?.Invoke(false);
        _beacon = false;
        _body = false;
        _leftEngine = false;
        _leftWing = false;
        _sM_Thrust_01 = false;
        _sM_Thrust_02 = false;
        _stage01 = false;
        _stage02 = false;
        _stage03 = false;
        //StartBossFight();
        ResetBossEvent?.Invoke();
        BossSetup();
        //Debug.Log("Boss was reset and invoked");
    }
    private void RegisterPartDestroid(BossParts parts)
    {
        if (parts == BossParts.Beacon)
            _beacon = true;
        if (parts == BossParts.Body)
            _body = true;
        if (parts == BossParts.LeftEngine)
            _leftEngine = true;
        if (parts == BossParts.LeftWing)
            _leftWing = true;
        if (parts == BossParts.SM_Thruster_01)
            _sM_Thrust_01 = true;
        if (parts == BossParts.SM_Thruster_02)
            _sM_Thrust_02 = true;
        DeterminStage();
    }
    private void DeterminStage()
    {
        //Note: Any missed ID will glitch this out and skip stages
        if (_sM_Thrust_01 && _sM_Thrust_02 && _leftEngine && !_stage01)
        {
            _stage01 = true;
            EnableStageColliders(_stage2Parts);
            MoveBossToNextPosition();
        }
        if (_body && _leftWing && !_stage02)
        {
            _stage02 = true;
            EnableStageColliders(_stage3Parts);
            MoveBossToNextPosition();
        }
        if (_beacon && !_stage03)
        {
            ActiveMother?.Invoke(false);
            _hasStarted = false;
            _stage03 = true;
            _BossDestroyed = true;
            _motherShipsDestroyed++;
            MoveBossToNextPosition();
        }
    }
    private float _currentDifficulty = -1;
    private void SetDifficultyCurve()
    {
        //Difficulty calculation current RandomRange(round-int(difficulty), round-int(difficulty^2)) * current boss fight.

        if(_currentDifficulty < 0)
        {
            Debug.Log("Difficulty was never set by GameManager " + _currentDifficulty);
            _currentDifficulty = 1;
        }
        //Variables used
        int difficultySqr = Mathf.RoundToInt(_currentDifficulty * _currentDifficulty);
        int mothershipDifficulty = 0;
        //Todo: Mothership fight will get boring with no enemies to fight when _mothershipsDestroyed gets too large
        //Limit _mothershipsDestroyed or add exception difficulty when high.
        //Todo: Mother Ship, when a wave is complete and a part is destroyed spawn new wave.
        //Todo: Mother ship can attack when _motherShipsDestryed is > then ?
        mothershipDifficulty = Random.Range(Mathf.RoundToInt(_currentDifficulty), difficultySqr) * _motherShipsDestroyed;

        Debug.Log("Mothership difficulty was set to " + mothershipDifficulty);
        mothershipDifficulty = mothershipDifficulty <= 0 ? 1 : mothershipDifficulty;
        SetDifficulty(mothershipDifficulty);
    }
    private void MoveBossToNextPosition()
    {
        //Move the boss to the next position.
        _positionIndex++;
        if (_positionIndex < _positionSequence.Length)
        {
            //do something
            _nextPosition = _positionSequence[_positionIndex].position;
            if (_positionIndex > 1) ContinueWaves?.Invoke(false);
        }
    }
    private float _sqrDistanceThreshold = 0.01f;
    private float _squareDistanceToNextPosition = 0;
    void FixedUpdate()
    {
        _squareDistanceToNextPosition = (_MotherShip.position - _nextPosition).sqrMagnitude;
        //move the boss to the next position smoothly
        if (_MotherShip.position != _nextPosition || _squareDistanceToNextPosition > _sqrDistanceThreshold)
        {
            _MotherShip.position = Vector3.MoveTowards(_MotherShip.position, _nextPosition, _bossSpeed * Time.fixedDeltaTime);
        }
        else
        {
            
            if(_BossDestroyed && _squareDistanceToNextPosition < _sqrDistanceThreshold)
            {
                ResetBossFight();
            }
        }
    }
    private void OnDisable()
    {
        if (Instance != this) return;
        BossExplosions.OnDisablePart -= RegisterPartDestroid;
        EnemySpawnManager.NewWaveEvent -= BossFightIntervals;
        UIManager.ResetLevel -= ResetGame;

        GameManager.MasterDifficulty -= (x) => { _currentDifficulty = x; };
        GameManager.NewDifficulty -= (x) => { _currentDifficulty = x; };
    }
}