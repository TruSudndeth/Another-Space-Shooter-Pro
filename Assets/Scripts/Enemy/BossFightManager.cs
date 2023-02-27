using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossFightManager : DontDestroyHelper<BossFightManager>
{
    //public static BossFightManager Instance;

    //Delete: event this is not used ????
    //public delegate void BossFightStage();
    //public static event BossFightStage OnBossFightStage;

    public delegate void EnableStageCollider(BossParts part);
    public static event EnableStageCollider EnableStageColliderPart;

    public delegate void ResetBoss();
    public static event ResetBoss ResetBossEvent;
    
    [Space(20)]
    [Header("Boss Stage Collider Setup")]
    [SerializeField]
    private List<BossParts> _stage1Parts;
    [SerializeField]
    private List<BossParts> _stage2Parts;
    [SerializeField]
    private List<BossParts> _stage3Parts;
    private List<List<BossParts>> _stages;

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
    [Space (20)]
    [SerializeField]
    private int _bossFightInterval = 5;
    private int _waveCount = 0;
    private int _maxIntValue;

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
    }
    public void StartBossFight()
    {
        if(!_hasStarted)
        {
            EnableStageColliders(_stage1Parts);
            _hasStarted = true;
            _positionIndex = 0;
            if(!_MotherShip.gameObject.activeSelf)
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
        _waveCount++;
        if (_waveCount % _bossFightInterval == 0)
        {
            if (_waveCount >= _maxIntValue)
            {
                _waveCount = 0;
            }
            StartBossFight();
        }
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
        _waveCount = 0;
        ResetBossFight();
    }
    private void ResetBossFight()
    {
        _positionIndex = 0;
        _hasStarted = false;
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
        if (_sM_Thrust_01 && _sM_Thrust_02 && _leftEngine && !_stage01)
        {
            _stage01 = true;
            EnableStageColliders(_stage2Parts);
            MoveBossToNextPosition();
        }
        if(_body && _leftWing && !_stage02)
        {
            _stage02 = true;
            EnableStageColliders(_stage3Parts);
            MoveBossToNextPosition();
        }
        if(_beacon && !_stage03)
        {
            _hasStarted = false;
            _stage03 = true;
            MoveBossToNextPosition();
        }
    }
    private void MoveBossToNextPosition()
    {
        //Move the boss to the next position.
        _positionIndex++;
        if (_positionIndex < _positionSequence.Length)
        {
            //do something
            _nextPosition = _positionSequence[_positionIndex].position;
        }
    }
    void FixedUpdate()
    {
        //move the boss to the next position smoothly
        if (_MotherShip.position != _nextPosition)
        {
            _MotherShip.position = Vector3.MoveTowards(_MotherShip.position, _nextPosition, _bossSpeed * Time.fixedDeltaTime);
        }
    }
    private void OnDisable()
    {
        if (Instance != this) return;
        BossExplosions.OnDisablePart -= RegisterPartDestroid;
        EnemySpawnManager.NewWaveEvent -= BossFightIntervals;
        UIManager.ResetLevel -= ResetBossFight;
    }
}