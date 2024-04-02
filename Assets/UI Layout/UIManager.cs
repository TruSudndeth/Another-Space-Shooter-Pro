using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Text;
using UnityEngine;
using System;
using System.Linq;

public class UIManager : DontDestroyHelper<UIManager>
{
    //Todo: Add Difficulty visual feedback. Test, Easy, Mid, Hard (Add text for each setting, update UI)
    //LeftOff: _mainMenue is fucking things up. when in the main menu starting game we need to unsubscribe from UIDocument
    //When in game space we need to subscribe to UIDocument
    //when space is hit in game the game restarts.
    //Player inputs might be working but must double check.
    public delegate void Reset();
    public static event Reset ResetLevel;
    public delegate void LoadScene(Types.GameState index);
    public static event LoadScene Load_Scene;
    public delegate void ExitApp();
    public static event ExitApp ExitApplication;

    public delegate void UISetsDifficulty(int difficulty);
    public static event UISetsDifficulty SetDifficultyUI;

    [Space]
    private UIDocument _UIDocGame;
    private UIDocument _UIDocMenu;

    [Space(20)]
    [SerializeField] private Transform _mainMenuUI;
    [SerializeField] private Transform _gamePlayUI;

    //Todo: if pool is too small grow the list
    [SerializeField]
    private Transform _pointPoolParent;
    [SerializeField]
    private Transform _pointTextFX;
    private List<Transform> _pointPoolFX;
    private int _pointCount = 10;

    private Types.GameState _gameState;
    private bool _isMainMenu = true;

    [Space]
    private Button _startBTN;
    private Button _optionsBTN;
    private Button _quitBTN;
    private Slider _musicSlider;
    private Slider _soundSlider;
    private SliderInt _difficultySlider;
    private float _musicVolume = 0.5f;
    private float _currentMusicVolume = 0;
    private float _soundVolume = 0.5f;
    private float _currentSoundVolume = 0;

    [Space]
    private VisualElement _rootMenu;
    private VisualElement _rootGame;
    private VisualElement _healthBar;
    private int _score = 0;
    private Label _scoreLabel;
    private Label _ammo;
    private Label _title;

    [Space]
    [SerializeField] private float _gameOverFlashTime = 1.0f;
    private Label _gameOver;
    private float _gameOverTime = 0.0f;
    private bool _isGameOver = false;
    private Label _restartText;

    [Space]
    [SerializeField] private List<Sprite> _healthStatus;
    private List<StyleBackground> _healthStatusStyle;

    [Space]
    private bool _hasRestarted = false;

    [Space]
    [SerializeField] private float _thrusterCoolDownTime = 1.0f;
    private float _thrusterTime = 0.0f;
    private bool _canThrust = false;

    private ProgressBar _thrusterCoolDown;
    private VisualElement _thrusterColor;
    private bool _thrustBPM = false;

    //Delete: ResetUIVisualss public bools, test code
    public bool LoadMainMenu = false;
    //Todo: Listen for player thruster event
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        if (!_mainMenuUI.gameObject.activeSelf || !_gamePlayUI.gameObject.activeSelf)
        {
            _isMainMenu = !_gamePlayUI.gameObject.activeSelf;
            _mainMenuUI.gameObject.SetActive(true);
            _gamePlayUI.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        SetUIRef();
        //Debug: UIManager sub to events for Menu
        SubscribeToEvents();

        UpdateUIVisuals();
        SetSceneInputs(); //Delete: Unknown if needed here
        _pointPoolFX = new(_pointCount);
        PopulateTextPool();
    }
    private void PopulateTextPool()
    {
        Vector2 worldPositions = new Vector2 (GameConstants.World.DefaultLocation.X, GameConstants.World.DefaultLocation.Y);
        Vector3 currentPosition = new Vector3();
        for (int i = 0; i < _pointCount; i++)
        {
            _pointPoolFX.Add(Instantiate(_pointTextFX, Vector2.zero, Quaternion.identity, _pointPoolParent));
            currentPosition = _pointPoolFX[^1].position;
            currentPosition = new Vector3(worldPositions.x, worldPositions.y, currentPosition.z);
            _pointPoolFX[^1].position = currentPosition;
            _pointPoolFX[^1].gameObject.SetActive(false);
        }
    }
    private Transform _currentTextTransform = null;
    public void RequestText(string text, Vector3 position)
    {
        CollectablePointsFX requestText = null;
        _currentTextTransform = null;
        _currentTextTransform = _pointPoolFX.FirstOrDefault(x => !x.gameObject.activeSelf);
        requestText = _currentTextTransform.GetComponent<CollectablePointsFX>();
        if(!_currentTextTransform || !requestText)
        {
            Debug.Log("Text Request was not found or all requests are being used");
        }else
        {
            _currentTextTransform.gameObject.SetActive(true);
            _currentTextTransform.position = position;
            requestText.PreviewTextRequest(text);
        }
    }
    private void SubscribeToEvents()
    {
        // Main Menu events
        InputManager.Instance.Exit.performed += _ => ExitGame();
        // Game Play Events
        InputManager.Instance.Restart.performed += _ => _hasRestarted = true;
        BackGroundMusic_Events.BGM_Events += () => _thrustBPM = true;
        Player.Thruster += ThrusterCoolDown;
        Player.UpdateAmmo += UpdateAmmo;
        Player.Score += UpdateScore;
        Player.UpdateHealth += UpdateHealth;
        Player.Game_Over += GameOver;
        StartGameAsteroids.GameStarted += GameStarted;
    }
    private void ExitGame()
    {
        if (_isMainMenu)
        {
            Debug.Log("Application closed in Main Menu");
            ExitApplication?.Invoke();
        }
        if (!_isMainMenu)
        {
            _isMainMenu = true;
            ResetGame();
            _gameState = Types.GameState.MainNenu;
            Load_Scene?.Invoke(_gameState);
        }
    }
    private void SetSceneInputs()
    {
        if (!_isMainMenu)
        {
            UpdateScore(0);
            //InputManager.Instance.EnableRestart(false);
            InputManager.Instance.EnablePlayerIO(true);
            //Delete: Debuglog (GamePlay Inputs Eneabled)
            //Debug.Log("Game Play Inputs enabled");
            //Debug: UIManager uncomment code bellow
            //RegisterAllCallbacks();
            UnregisterAllCallBacks();
        }
        else if (_isMainMenu)
        {
            //Delete: Debug.Log("Main Menu Inputs enabled");
            Debug.Log("Main Menu Inputs enabled");
            //Do Basic Setup here.
            InputManager.Instance.UIDisabled(false);

            //Debug: UIManager Uncoment code bellow
            RegisterAllCallbacks();
            SetSlidersMaxMin();
            UpdateListeners();
        }
    }
    [SerializeField]
    private int _difficulty = 1;
    private void SetSlidersMaxMin()
    {
        _musicSlider.lowValue = 0.0f;
        _musicSlider.highValue = 1.0f;
        _soundSlider.lowValue = 0.0f;
        _soundSlider.highValue = 1.0f;
        _difficultySlider.lowValue = 0;
        _difficultySlider.highValue = 3;
        //Difficulty set to easy
        _difficultySlider.value = _difficulty;
        _soundSlider.value = _soundVolume;
        _musicSlider.value = _musicVolume;
        //Set Visuals
        _difficultySlider.style.display = _optionsIsVisible;
        _soundSlider.style.display = _optionsIsVisible;
        _musicSlider.style.display = _optionsIsVisible;
    }
    private void UpdateListeners()
    {
        SetDifficultyUI?.Invoke(_difficulty);
        AudioManager.Instance.UpdateMusicVolume(_currentMusicVolume);
        AudioManager.Instance.UpdateSFXVolume(_currentSoundVolume);
    }
    private void RegisterAllCallbacks()
    {
        _quitBTN.clicked += Application.Quit;
        _optionsBTN.clicked += OptionsEnableDisable;
        _startBTN.clicked += LoadLevelOne;
        //Delete: Debug Log, testing only
        Debug.Log("Registering all callbacks");
        _title.RegisterCallback<ClickEvent>(LoadLevelZero);
        _startBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _optionsBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _quitBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));

        _startBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _optionsBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _quitBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));

        //Todo: Add Audio Settings
        _musicSlider.RegisterValueChangedCallback((evt) => _musicVolume = evt.newValue);
        _soundSlider.RegisterValueChangedCallback((evt) => _soundVolume = evt.newValue);
        _difficultySlider.RegisterValueChangedCallback((evt) => { SetDifficultyUI?.Invoke(evt.newValue); });
    }
    private void UnregisterAllCallBacks()
    {
        _quitBTN.clicked -= Application.Quit;
        _optionsBTN.clicked -= OptionsEnableDisable;
        _startBTN.clicked -= LoadLevelOne;
        //Delete: Debug Log, testing only
        // Debug.Log("Unregistering all callbacks");
        _startBTN.UnregisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _optionsBTN.UnregisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _quitBTN.UnregisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));

        _startBTN.UnregisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _optionsBTN.UnregisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _quitBTN.UnregisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));

        //Todo: Add Audio Settings
        _musicSlider.UnregisterValueChangedCallback((evt) => _musicVolume = evt.newValue * 0.01f);
        _soundSlider.UnregisterValueChangedCallback((evt) => _soundVolume = evt.newValue * 0.01f);
        _difficultySlider.UnregisterValueChangedCallback((evt) => { SetDifficultyUI?.Invoke(evt.newValue); });
    }
    private void GameStarted()
    {
        _scoreLabel.visible = true;
    }
    //when LoadLevelOne ran for some reason the level properly
    private void LoadLevelOne()
    {
        _isMainMenu = false;
        ResetGame();
        _gameState = Types.GameState.Level1;
        Load_Scene?.Invoke(_gameState);
    }
    private void LoadLevelZero(ClickEvent evt)
    {
        _isMainMenu = false;
        Debug.Log("Loaded Level zero");
        ResetGame();
        _gameState = Types.GameState.Level0;
        Load_Scene?.Invoke(_gameState);
    }
    private void ResetGame()
    {
        _isGameOver = false;
        _score = 0;
        //Debug: ResetLevel should only be called when already in game
        InputManager.Instance.EnablePlayerIO(true);
        InputManager.Instance.EnableRestart(false);
        UpdateUIVisuals();
        SetSceneInputs();
        if (!_isMainMenu)
        ResetLevel?.Invoke();
    }
    private DisplayStyle _optionsIsVisible = DisplayStyle.None;
    private void OptionsEnableDisable()
    {
        _optionsIsVisible = _musicSlider.style.display.value == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        _musicSlider.style.display = _optionsIsVisible;
        _soundSlider.style.display = _optionsIsVisible;
        _difficultySlider.style.display = _optionsIsVisible;
    }
    private void Update()
    {
        if(_hasRestarted)
        {
            _hasRestarted = false;
            //Debug: RestartGame() might be calling double Loads scenes.
            ResetGame();
            InputManager.Instance.EnableRestart(false);
        }
        //Delete: These Iff Condition testing bools are true bellow
        if(LoadMainMenu)
        {
            LoadMainMenu = false;
            ExitGame();
        }
}
    private void FixedUpdate()
    {
        if (_gameOverTime + _gameOverFlashTime < Time.time && _isGameOver && !_isMainMenu)
        {
            _gameOverTime = Time.time;
            _gameOver.style.visibility = _gameOver.style.visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }
        if(!_canThrust)
        {
            UpdateThrusters(_thrusterCoolDownTime);
        }
        VolumeSetup();
    }
    [SerializeField]
    private float _SFXReffTrigger = 0.1f;
    private float _currentSFXReffTrigger = 0;
    private void VolumeSetup()
    {
        //update Volume if different then current volume
        if (_musicVolume != _currentMusicVolume)
        {
            _currentMusicVolume = _musicVolume;
            AudioManager.Instance.UpdateMusicVolume(_musicVolume);
        }
        if (_soundVolume != _currentSoundVolume)
        {
            if (Mathf.Abs(_soundVolume - _currentSFXReffTrigger) > _SFXReffTrigger)
            {
                AudioManager.Instance.PlayAudioOneShot(Types.SFX.MiniBossLaser);
                _currentSoundVolume = _soundVolume;
            }
            AudioManager.Instance.UpdateSFXVolume(_soundVolume);
        }
    }
    private void ThrusterCoolDown(float time)
    {
        _thrusterTime = 0;
        _thrusterCoolDownTime = time;
        _canThrust = false;
    }
    private void UpdateThrusters(float timeDuration)
    {
        //GDHQ: Progress bar color;
        if(_thrusterTime >= 1 && _thrustBPM)
        {
            _thrustBPM = false;
            _canThrust = true;
            _thrusterTime = 1;
            _thrusterColor.style.backgroundColor = Color.green;
            _thrusterCoolDown.value = Mathf.Lerp(0, 1, _thrusterTime);
            return;
        }
        _thrusterTime += Time.fixedDeltaTime / timeDuration;
        Color color = Color.Lerp(Color.red, Color.green, _thrusterTime);
        if(_thrustBPM)
        {
            _thrustBPM = false;
            _thrusterCoolDown.value = Mathf.Lerp(0, 1, _thrusterTime);
            _thrusterColor.style.backgroundColor = color;
        }
    }
    private void UpdateScore(int points)
    {
        if(!_isMainMenu)
        {
            _score = points;
            //Might Not Update current score from player at start
            //label is not being found (Script Execution order)
            _scoreLabel.text = "Score: " + _score;
        }
    }
    private void UpdateAmmo(int _ammo, int _maxAmmo)
    {
        if (!_isMainMenu)
        {
            StringBuilder ammoText = new(String.Format("Ammo: {0}/{1}", _ammo, _maxAmmo));
            this._ammo.text = ammoText.ToString();
        }
    }
    private void GameOver()
    {
        if(!_isMainMenu)
        {
            //create a on off logic over time
            Debug.Log("GameOver");
            _isGameOver = true;
            _restartText.visible = true;
            InputManager.Instance.EnableRestart(true);
        }
    }
    private void UpdateHealth(int health)
    {
        if (!_isMainMenu)
        {
            if (health > 3) health = _healthStatusStyle.Count -1;
            _healthBar.style.backgroundImage = _healthStatusStyle[health];
        }
    }
    //The invokes are good, the same thing is heppening here is UpdateUIVisuals when this is called when LoadLevel is called and runs if !_isMainMenu condition.
    private void UpdateUIVisuals()
    {        
        if(_isMainMenu)
        {
            //Delete: Debug Log, testing only
            Debug.Log("Updating UI Visuals");
            //Update Visual UI Setup for Main Menu
            _rootMenu.visible = true;
            _rootGame.visible = false;
            //_gamePlayUI.gameObject.SetActive(false);
            //_mainMenuUI.gameObject.SetActive(true);
            //RegisterAllCallbacks();
        }
        else if (!_isMainMenu)
        {
            _rootMenu.visible = false;
            _rootGame.visible = true;
            //Delete: Debug Log, testing only
            //Debug.Log("ResetUIVisuals for GamePlay");
            //Visual UI SetUp Game Play
            _healthBar.style.backgroundImage = _healthStatusStyle[_healthStatus.Count - 1];
            _gameOver.visible = false;
            _restartText.visible = false;
            _scoreLabel.visible = false;

            //_gamePlayUI.gameObject.SetActive(true);
            //_mainMenuUI.gameObject.SetActive(false);
        }
    }
    private void SetUIRef()
    {
        if (_gamePlayUI.TryGetComponent(out UIDocument uiGame))
        {
            _UIDocGame = uiGame;
            _rootGame = _UIDocGame.rootVisualElement;
        }
        else Debug.Log("No UI Document Component" + transform);
        if (_mainMenuUI.TryGetComponent(out UIDocument uiMenu))
        {
            _UIDocMenu = uiMenu;
            _rootMenu = _UIDocMenu.rootVisualElement;
        }
        else Debug.Log("No UI Document Component" + transform);
        
        if (!_UIDocMenu || !_UIDocGame)
        {
            Debug.Log("_rootmenu or _rootGame lost reference or not set", transform);
            return;
        }

        // Game Play Reff Setup
        _thrusterColor = _rootGame.Q(className: "unity-progress-bar__progress");
        _thrusterCoolDown = _rootGame.Q<ProgressBar>("ThrusterCoolDown");
        _healthBar = _rootGame.Q<VisualElement>("HealthBar");
        _restartText = _rootGame.Q<Label>("Restart_Text");
        _gameOver = _rootGame.Q<Label>("GameOver");
        _scoreLabel = _rootGame.Q<Label>("Score");
        _ammo = _rootGame.Q<Label>("Ammo");
        _thrusterCoolDown.highValue = 1;
        _thrusterCoolDown.lowValue = 0;
        _thrusterCoolDown.value = 0;
        _healthStatusStyle = new(4);
        for (int i = 0; i < _healthStatus.Count; i++)
        {
            _healthStatusStyle.Add(new StyleBackground(_healthStatus[i]));
        }
        //unity-progress-bar__title-container
        //unity-progress-bar__background
        //unity-progress-bar__container
        //unity-progress-bar__progress

        // Main menu Reff Setup
        _title = _rootMenu.Q<Label>("GameTitle");
        _startBTN = _rootMenu.Q<Button>("Start");
        _optionsBTN = _rootMenu.Q<Button>("Options");
        _quitBTN = _rootMenu.Q<Button>("Quit");
        _musicSlider = _rootMenu.Q<Slider>("Music");
        _soundSlider = _rootMenu.Q<Slider>("Sound");
        _difficultySlider = _rootMenu.Q<SliderInt>("Difficulty");
    }
    private void OnDisable()
    {
        if (Instance != this) return;  //Bug: Something is instantiating InputManager in OnDisable()
        //Debug: redundant event un-subscription during gameplay mode
        UpdateDisabled();
        UnregisterAllCallBacks();
    }
    private void UpdateDisabled()
    {
        BackGroundMusic_Events.BGM_Events -= () => _thrustBPM = true;
        Player.Thruster -= ThrusterCoolDown;
        InputManager.Instance.Restart.started -= _ => _hasRestarted = true;

        Player.Score -= UpdateScore;
        Player.UpdateHealth -= UpdateHealth;
        Player.Game_Over -= GameOver;
        Player.UpdateAmmo -= UpdateAmmo;
        StartGameAsteroids.GameStarted -= GameStarted;
    }
}