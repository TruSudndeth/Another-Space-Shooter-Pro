using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public delegate void GameOver();
    public static GameOver Game_Over;
    public delegate void Points(int points);
    public static Points Score;
    public delegate void PlayerHealth(int health);
    public static PlayerHealth UpdateHealth;
    public delegate void PlayerAmmo(int ammo, int maxAmmo);
    public static PlayerAmmo UpdateAmmo;
    public delegate void PlayerThruster(float Duration);
    public static PlayerThruster Thruster;
    public delegate void PlayerDamage(float Duration);
    public static PlayerDamage OnPlayerDamage;
    public int Health { get { return _health; } set { Damage(value); } }

    [Space]
    [SerializeField] private Types.SFX _playerDeath;
    private int _playerScore = 0;
    [SerializeField] private float _interPoMoveSpeed = 2.0f;
    [SerializeField] private float _bankSpeed = 10.0f;
    [SerializeField] private float _maxBank = 45.0f;
    [SerializeField] private float _antiSpeed = 2.0f;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _speedBoostTimeout = 5.0f;
    [SerializeField] private float _thrustDistance = 3.0f;
    [SerializeField] private float _thrustCoolDown = 10.0f;
    private bool _isAntiSpeed = false;
    private float _thrustTimer = 0.0f;
    private bool _actuateThrust = false;
    private float _speedBoostTime = 0.0f;
    private bool _isSpeedBoostActive = false;
    private Vector2 _movePlayerFixed;
    private Vector2 _movePlayer = Vector2.zero;
    private float _bank = 0;
    [Space]
    [SerializeField] private bool _fired = false;
    [Space]
    private Transform _myCamera;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.8f, 0); //DeleteLine: Not used
    [SerializeField] private float _singeFireRate = 0.25f;
    [SerializeField] private float _trippleFireRate = 0.5f;
    [SerializeField] private bool _isTrippleShot = false;
    [SerializeField] private float _powerUpTimeout = 5.0f;
    [SerializeField] private int _ammoBank = 15;
    private int _ammoBankMax;
    private float _powerUpTime = 0.0f;
    private List<Transform> _tripleShot;
    private float _canFire = 0;
    private float _cameraOrthoSize = 5;
    private readonly float _cameraAspectRatio = 1.7777778f;
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
    private bool _resetShield = false;
    private bool _defectiveShield = false;
    [Space]
    [SerializeField] private bool _updateScore = false;
    private bool _gameStarted = false;
    private Rigidbody _rigidbody;
    [Space]
    [SerializeField] private float _bombTimeout = 5.0f;
    private bool _useBomb = false;
    private float _bombTime = 0.0f;
    [Space]
    [SerializeField] private float _playerDamageShake = 0.5f;
    [Space]
    private bool _antiTrippleShot = false;

    //Toto: Input bug holding Down and left doesn't allow to shoot
    private void Awake()
    {
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            _rigidbody = rigidbody;
        }
        else
        {
            Debug.LogError("Rigidbody not found on Player");
        }
        _ammoBankMax = _ammoBank;
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
            UpdateScore(111, transform.name); //todo: test transform.name for bugs
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
        float antiSpeed = _isAntiSpeed ? _antiSpeed : 1;
        _movePlayerFixed = _speed * antiSpeed * Time.fixedDeltaTime * _movePlayer;
        //Todo: Thrust Add a speed boost
        bool canThrust = false;
        if (_thrustCoolDown + _thrustTimer < Time.time && _actuateThrust) canThrust = true;
        else if (_actuateThrust) _actuateThrust = false;
        _movePlayerFixed = canThrust ? AddThrust() * _movePlayerFixed.normalized + _movePlayerFixed : _movePlayerFixed;
        _movePlayerFixed = OutOfBounds.CalculateMove(transform, _movePlayerFixed, _xyBounds);
        if (_movePlayerFixed == Vector2.zero) _rigidbody.velocity = Vector3.zero;
    }
    private void FixedUpdate()
    {
        //GDHQ: New Projectile 5 Second Time out.
        if(_useBomb && Time.time > _bombTime + _bombTimeout)
        {
            _useBomb = false;
            AudioManager.Instance.PlayAudioOneShot(Types.SFX.ErrorSound);
        }
        if (_fired) UseLaserPool();
        if(_actuateThrust)
        {
            Thruster?.Invoke(_thrustCoolDown);
            _actuateThrust = false;
            _thrustTimer = Time.time;
        }
        transform.Translate(_movePlayerFixed, Space.World);

        if (_powerUpTime + _powerUpTimeout <= Time.time && _isTrippleShot)
            _isTrippleShot = false;
        if (_speedBoostTime + _speedBoostTimeout <= Time.time && _isSpeedBoostActive)
        {
            _isAntiSpeed = false;
            _isSpeedBoostActive = false;
        }
        if (_shieldTime + _shieldTimeout <= Time.time && _isShieldActive)
            Damage(0);
    }
    public void AddHealth(bool Anti_Powerup)
    {
        //Todo: Add a negative Override to health
        //50% chance to remove a health else add a health. This can kill the player.
        //Todo: Add Health Sound FX
        bool antiProbablility = Anti_Powerup ? Random.Range(1, 100) > 50 : false;
        if (_health == 3 && !antiProbablility) return;
        if (antiProbablility) Damage(1); else _health++;
        if (_health == 3)
        {
            _damageLeftENG.SetActive(false);
            _damageRighENG.SetActive(false);
        }
        else if (_health == 2)
        {
            _damageRighENG.SetActive(false);
        }
        UpdateHealth?.Invoke(_health);
    }
    public void AddAmmo(bool Anti_Powerup)
    {
        //Todo: Add a negative overide to Ammo
        //Override will ony have a 50% change to give full ammo else give 3
        //Todo: Ammo Reload Sound effect
        bool antiProbablility = Anti_Powerup ? Random.Range(1, 100) > 50 : false;
        if (_ammoBank == _ammoBankMax && antiProbablility) return;
        if(Anti_Powerup)
        _ammoBank = antiProbablility ? _ammoBankMax : Mathf.Clamp(_ammoBank + 3, 0, _ammoBankMax);
        else
            _ammoBank = Mathf.Clamp(_ammoBank + 15, 0, _ammoBankMax);
        UpdateAmmo(_ammoBank, _ammoBankMax);
    }
    public void SpeedBoost(bool Anti_Powerup)
    {
        //Todo: Add a negative override to speed boost
        //will give the player an Ugly speed boost for a short time
        _isAntiSpeed = Anti_Powerup;
        _isSpeedBoostActive = true;
        _speedBoostTime = Time.time;
    }
    public void TripleShotActive(bool Anti_Powerup)
    {
        //Todo: Add a negative override to tripple shot
        //Override Tripple shot will take 3 x ammo
        _antiTrippleShot = Anti_Powerup;
        _isTrippleShot = true;
        _powerUpTime = Time.time;
    }
    public void ShieldActive(bool Anti_Powerup)
    {
        //Todo: Add a negative override to Shield
        //Override Shild has a 50% change to protect player from damage
        //Todo: add Defective Shield visual and sound
        if (_isShieldActive)
        {
            _defectiveShield = Anti_Powerup;
            _resetShield = true;
            Damage(0);
        }
        AudioManager.Instance.PlayAudioOneShot(Types.SFX.ShieldOn);
        _shieldTime = Time.time;
        _shield.gameObject.SetActive(true);
        _isShieldActive = true;
    }
    public void UseBomb()
    {
        //GDHQ: New Projectile Prt 2 for a 5 Second Time out.
        _useBomb = true;
        _bombTime = Time.time;
    }
    private void Damage(int damage)
    {
        //Todo: Make Shield Camera shake Damage less than player damage
        if(damage > 0)
        OnPlayerDamage?.Invoke(_playerDamageShake);
        bool defectiveShield = _defectiveShield && Random.Range(1, 100) > 50;
        if (_isShieldActive && !defectiveShield) //Shield Damage
        {
            if (_shield.gameObject.TryGetComponent(out ShieldBehavior shieldB))
            {
                if (_resetShield)
                {
                    _resetShield = false;
                    shieldB.ResetShield();
                }
                shieldB.Damage(damage);
                if (!shieldB.isActiveAndEnabled) _isShieldActive = false;
                return;
            }
            else
            {
                AudioManager.Instance.PlayAudioOneShot(Types.SFX.ShieldOff);
                _isShieldActive = false;
                _shield.gameObject.SetActive(false);
                return;
            }
        }
        //Player Damage
        
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
    private float AddThrust()
    {
        return _thrustDistance;
    }
    private void UseLaserPool()
    {
        if (_gameStarted)
        {
            if (_useBomb) //GDHQ: New Projectile UsingBomb
            {
                _fired = false;
                _useBomb = false;
                LaserManager.Instance.CallBombPool(transform);
                return;
            }
            if (_ammoBank <= 0)
            {
                //Todo: Sound Invoke Out of ammo sound FX
                _ammoBank = 0;
                _fired = false;
                return;
            }
            _ammoBank = _antiTrippleShot ? _ammoBank - 3 : _ammoBank - 1;
            UpdateAmmo?.Invoke(_ammoBank, _ammoBankMax);
        }
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
    private void GameStarted()
    {
        _gameStarted = true;
        _ammoBank = _ammoBankMax;
        UpdateAmmo?.Invoke(_ammoBank, _ammoBankMax);
    }
    public void UpdateScore(int points, string enemyName)
    {
        _playerScore += points;
        Score?.Invoke(_playerScore);
    }
    private void SubscribeToInputs()
    {
        StartGameAsteroids.GameStarted += () => GameStarted();
        InputManager.Instance.Fire.performed += _ => _fired = true;
        InputManager.Instance.Thrust.started += _ => _actuateThrust = true;
    }
    private void OnDisable()
    {
        StartGameAsteroids.GameStarted -= () => GameStarted();
        InputManager.Instance.Thrust.started -= _ => _actuateThrust = true;
        EnemyCollisons.EnemyPointsEvent -= UpdateScore;
        InputManager.Instance.Fire.performed -= _ => _fired = true; //??? Look into this unsubscribe
        Game_Over?.Invoke();
        InputManager.Instance.EnablePlayerIO(false);
    }
    
}
