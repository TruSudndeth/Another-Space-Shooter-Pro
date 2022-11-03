using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public delegate void GameOver();
    public static GameOver gameOver;
    public delegate void Points(int points);
    public static Points Score;
    public delegate void PlayerHealth(int health);
    public static PlayerHealth UpdateHealth;
    public int Health { get { return _health; } set { Damage(value); } }
    [Space]
    [SerializeField] private Types.SFX _playerDeath;
    private int _playerScore = 0;
    [SerializeField] private float _interPoMoveSpeed = 2.0f;
    [SerializeField] private float _bankSpeed = 10.0f;
    [SerializeField] private float _maxBank = 45.0f;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _speedBoostTimeout = 5.0f;
    private float _speedBoostTime = 0.0f;
    private bool _isSpeedBoostActive = false;
    private Vector2 _movePlayerFixed;
    private Vector2 _movePlayer = Vector2.zero;
    private float _bank = 0;
    [Space]
    [SerializeField] private bool _fired = false;
    [Space]
    private Transform _myCamera;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.8f, 0);
    [SerializeField] private float _singeFireRate = 0.25f;
    [SerializeField] private float _trippleFireRate = 0.5f;
    [SerializeField] private bool _isTrippleShot = false;
    [SerializeField] private float _powerUpTimeout = 5.0f;
    private float _powerUpTime = 0.0f;
    private List<Transform> _tripleShot;
    private float _canFire = 0;
    private float _cameraOrthoSize = 5;
    private float _cameraAspectRatio = 1.7777778f;
    private float _xBounds = 0;
    private float _yBounds = 0;
    private Vector2 _xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private Transform _primaryLaserSpawn;
    [SerializeField] private Transform _dualLaser_LSpawn;
    [SerializeField] private Transform _dualLaser_RSpawn;
    [Space]
    [SerializeField] private GameObject _damageLeftENG;
    [SerializeField] private GameObject _damageRighENG;
    [SerializeField] private Transform _shield;
    [SerializeField] private int _health = 3;
    [SerializeField] private float _shieldTimeout = 60.0f;
    private bool _isShieldActive = false;
    private float _shieldTime = 0.0f;
    
    [SerializeField] private bool _updateScore = false;
    
    private void Awake()
    {
        _myCamera = Camera.main.transform;
        _tripleShot = new(){_primaryLaserSpawn, _dualLaser_LSpawn, _dualLaser_RSpawn};
    }
    
    void Start()
    {
        _cameraOrthoSize = _myCamera.GetComponent<Camera>().orthographicSize;
        _xBounds = _cameraOrthoSize * _cameraAspectRatio;
        _yBounds = _cameraOrthoSize;
        _xyBounds = new Vector2(_xBounds, _yBounds);
        SubscribeToInputs();
        EnemyCollisons.EnemyPointsEvent += UpdateScore;
    }
    
    void Update()
    {
        if (_updateScore)
        {
            _updateScore = false;
            UpdateScore(111);
        }
        Vector2 movement = InputManager.Instance.WSAD.ReadValue<Vector2>();
        //smooth out banks -                        Lets Incorperate
        float _bankSpeed = Time.deltaTime * this._bankSpeed;
        if(movement.x < 0)
        _bank = _bank >= 1 ? 1 : _bank + _bankSpeed; //bank left
        if(movement.x > 0)
        _bank = _bank <= -1 ? -1 : _bank - _bankSpeed; //bank right
        if(movement.x == 0)
        if(Mathf.Abs(_bank) > _bankSpeed * 2)
        _bank = _bank > 0 ? _bank - _bankSpeed: _bank + _bankSpeed; //Zero out
        transform.rotation = Quaternion.Euler(-90, 0, _maxBank * _bank);

        //Interpolate Movement
        float direction = _isSpeedBoostActive ? 1 : _interPoMoveSpeed * Time.deltaTime;
        _movePlayer = Vector2.MoveTowards(_movePlayer, movement, direction);
        
        //Bounds
        _movePlayerFixed = _speed * Time.fixedDeltaTime * _movePlayer;
        _movePlayerFixed = OutOfBounds.CalculateMove(transform, _movePlayerFixed, _xyBounds);
    }

    private void FixedUpdate()
    {
        if (_fired) UseLaserPool();
        
        transform.Translate(_movePlayerFixed, Space.World);

        if (_powerUpTime + _powerUpTimeout <= Time.time && _isTrippleShot)
            _isTrippleShot = false;
        if (_speedBoostTime + _speedBoostTimeout <= Time.time && _isSpeedBoostActive)
            _isSpeedBoostActive = false;
        if (_shieldTime + _shieldTimeout <= Time.time && _isShieldActive)
            Damage(0);
    }
    public void SpeedBoost()
    {
        _isSpeedBoostActive = true;
        _speedBoostTime = Time.time;
    }
    public void TripleShotActive()
    {
        _isTrippleShot = true;
        _powerUpTime = Time.time;
    }
    public void ShieldActive()
    {
        AudioManager.Instance.PlayAudioOneShot(Types.SFX.ShieldOn);
        _shieldTime = Time.time;
        _shield.gameObject.SetActive(true);
        _isShieldActive = true;
    }
    private void Damage(int damage)
    {
        if (_isShieldActive)
        {
            AudioManager.Instance.PlayAudioOneShot(Types.SFX.ShieldOff);
            _shield.gameObject.SetActive(false);
            _isShieldActive = false;
            return;
        }
        _health -= damage;
        UpdateHealth?.Invoke(_health);
        
        if (_health == 2 && !_damageLeftENG.activeSelf)
        {
            _damageLeftENG.SetActive(true);
        }
        else if (_health == 1 && !_damageRighENG.activeSelf)
        {
            _damageRighENG.SetActive(true);
        }
        
        if (_health <= 0)
        {
            if(TryGetComponent(out ParticlesVFX _explode))
            {
                AudioManager.Instance.PlayAudioOneShot(Types.SFX.PlayerDeath);
                _explode.PlayVFX();
            }
            gameObject.SetActive(false);
        }
        
    }
    private void UseLaserPool()
    {
        int tripleShotIndex = 0;
        _fired = false;
        if (_canFire + _singeFireRate > Time.time) return;
        _canFire = Time.time;
        for (int i = 0; i < _tripleShot.Count; i++)
        {
            LaserManager.Instance.LaserPool(_tripleShot[tripleShotIndex]);
            if (_isTrippleShot && tripleShotIndex == 0)
            {
                AudioManager.Instance.PlayAudioOneShot(Types.SFX.Tripple);
            }
            else if (!_isTrippleShot)
            {
                AudioManager.Instance.PlayAudioOneShot(Types.SFX.Laser);
            }
            
            if(_isTrippleShot)
            {
                tripleShotIndex++;
                continue;
            }
            break;
        }
    }
    public void UpdateScore(int points)
    {
        _playerScore += points;
        Score?.Invoke(_playerScore);
    }

    private void SubscribeToInputs()
    {
        InputManager.Instance.Fire.performed += _ => _fired = true;
    }
    private void OnDisable()
    {
        EnemyCollisons.EnemyPointsEvent -= UpdateScore;
        InputManager.Instance.Fire.performed -= _ => _fired = true; //??? Look into this unsubscribe
        gameOver?.Invoke();
        InputManager.Instance.DisablePlayerIO();
    }
    
}
