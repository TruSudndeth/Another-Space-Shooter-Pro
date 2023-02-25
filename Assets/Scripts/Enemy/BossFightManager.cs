using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
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

    private void Awake()
    {
        _positionSequence = _positions.GetComponentsInChildren<Transform>(false).Skip(1).ToArray();
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
        _positionIndex = 0;
        if(!_MotherShip.gameObject.activeSelf)
        {
            _MotherShip.gameObject.SetActive(true);
        }
        _nextPosition = _positionSequence[_positionIndex].position;
        _MotherShip.position = _nextPosition;
        //MoveBossToNextPosition();
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
        
        if (_nextStage)
        {
            MoveBossToNextPosition();
            _nextStage = false;
        }
    }
}