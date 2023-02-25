using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    //LeftOff: Get the instance and Start boss fight when wave 5 is complete.
    public static BossFightManager Instance;

    public delegate void BossFightStage();
    public static event BossFightStage OnBossFightStage;

    //LeftOff: Automate the MotherShip movement when stage 1 threw 3
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

    //Delete: Test bool _nextStage
    public bool _nextStage = false;

    private bool _hasStarted = true;
    private bool _beacon = false;
    private bool _body = false;
    private bool _leftEngine = false;
    private bool _leftWing = false;
    private bool _sM_Thrust_01 = false;
    private bool _sM_Thrust_02 = false;
    private bool _stage01 = false;
    private bool _stage02 = false;
    private bool _stage03 = false;

    private void Awake()
    {
        _positionSequence = _positions.GetComponentsInChildren<Transform>(false).Skip(1).ToArray();
        BossExplosions.OnDisablePart += RegisterPartDestroid;
    }
    void Start()
    {
        //Delete: TestCode for StartingBossFight
        StartBossFight();

        //ToDo: Listen for an event to start the boss fight.
        //Enable colliders for stage 1
        //enable the boss and position him from position 0 to position 1.
        //Listen for events that will trigger the boss to move to the next position. (thruster damage)
        //Enable colliders for stage 2
        //Listen for events that will trigger the boss to move to the next position. (thruster damage)
        //Enable colliders for stage 3
        //listen for events that will trigger the boss to die and display game over.
        //roll the credits. is any.
    }
    private void StartBossFight()
    {
        if(!_hasStarted)
        {
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
            MoveBossToNextPosition();
        }
        if(_body && _leftWing && !_stage02)
        {
            _stage02 = true;
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
        
        //Delete: if block for _nextStage
        if (_nextStage)
        {
            MoveBossToNextPosition();
            _nextStage = false;
        }
    }
}