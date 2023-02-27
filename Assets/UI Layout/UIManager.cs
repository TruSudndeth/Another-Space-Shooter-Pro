using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using NUnit.Framework.Internal.Filters;
using System.Diagnostics.CodeAnalysis;

public class UIManager : DontDestroyHelper<UIManager>
{
    //LeftOff: _mainMenue is fucking things up. when in the main menu starting game we need to unsubscribe from UIDocument
    //When in game space we need to subscribe to UIDocument
    //when space is hit in game the game restarts.
    //Player inputs might be working but must double check.
    public delegate void Reset();
    public static event Reset ResetLevel;
    public delegate void LoadScene(Types.GameState index);
    public static event LoadScene Load_Scene;
        
    [Space]
    private UIDocument _UIDocGame;
    private UIDocument _UIDocMenu;

    [Space(20)]
    [SerializeField] private Transform _mainMenuUI;
    [SerializeField] private Transform _gamePlayUI;
    
    private Types.GameState _gameState; 
    private bool _isMainMenu = true;

    [Space]
    private Button _startBTN;
    private Button _optionsBTN;
    private Button _quitBTN;
    private Slider _musicSlider;
    private Slider _soundSlider;
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

    //Delete: ResetUIVisualss bool test code
    public bool ResetUIVisualss = false;
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
        SubscribeToEvents();
        
        UpdateUIVisuals();
        SetSceneInputs(); //Delete: Unknown if needed here
    }
    private void SubscribeToEvents()
    {
        // Main Menu events
        _quitBTN.clicked += Application.Quit;
        _optionsBTN.clicked += AudioEnableDisable;
        _startBTN.clicked += LoadLevelOne;

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
    private void SetSceneInputs()
    {
        if (!_isMainMenu)
        {
            UpdateScore(0);
            //InputManager.Instance.EnableRestart(false);
            InputManager.Instance.EnablePlayerIO(true);
            //Delete: Debuglog (GamePlay Inputs Eneabled)
            Debug.Log("Game Play Inputs enabled");
            RegisterAllCallbacks();
            UnregisterAllCallBacks();
        }
        else if (_isMainMenu)
        {
            //Delete: Debug.Log("Main Menu Inputs enabled");
            Debug.Log("Main Menu Inputs enabled");
            //Do Basic Setup here.
            InputManager.Instance.UIDisabled(false);
            
            RegisterAllCallbacks();
        }
    }
    private void RegisterAllCallbacks()
    {
        //Delete: Debug Log, testing only
        Debug.Log("Registering all callbacks");
        _startBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _optionsBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _quitBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));

        _startBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _optionsBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _quitBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));

        //Todo: Add Audio Settings
        _musicSlider.RegisterValueChangedCallback((evt) => _musicVolume = evt.newValue * 0.01f);
        _soundSlider.RegisterValueChangedCallback((evt) => _soundVolume = evt.newValue * 0.01f);
    }
    private void UnregisterAllCallBacks()
    {
        //Delete: Debug Log, testing only
        Debug.Log("Unregistering all callbacks");
        _startBTN.UnregisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _optionsBTN.UnregisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
        _quitBTN.UnregisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));

        _startBTN.UnregisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _optionsBTN.UnregisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
        _quitBTN.UnregisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));

        //Todo: Add Audio Settings
        _musicSlider.UnregisterValueChangedCallback((evt) => _musicVolume = evt.newValue * 0.01f);
        _soundSlider.UnregisterValueChangedCallback((evt) => _soundVolume = evt.newValue * 0.01f);
    }
    private void GameStarted()
    {
        _scoreLabel.visible = true;
    }
    private void LoadLevelOne()
    {        
        _isMainMenu = false;
        ResetGame();

        InputManager.Instance.EnablePlayerIO(true);
        _gameState = Types.GameState.Level1;
        Load_Scene?.Invoke(_gameState);
    }
    private void AudioEnableDisable()
    {
        DisplayStyle isVisible = _musicSlider.style.display.value == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        _musicSlider.style.display = isVisible;
        _soundSlider.style.display = isVisible;
    }
    private void Update()
    {
        if(_hasRestarted)
        {
            _hasRestarted = false;
            ResetGame();
            InputManager.Instance.EnableRestart(false);
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
            _currentSoundVolume = _soundVolume;
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
    private void UpdateUIVisuals()
    {        
        if(_isMainMenu)
        {
            //Delete: Debug Log, testing only
            Debug.Log("Updating UI Visuals");
            //Update Visual UI Setup for Main Menu
            _gamePlayUI.gameObject.SetActive(false);
            _mainMenuUI.gameObject.SetActive(true);
        }
        if (!_isMainMenu)
        {
            _gamePlayUI.gameObject.SetActive(true);
            _mainMenuUI.gameObject.SetActive(false);
            //Delete: Debug Log, testing only
            Debug.Log("ResetUIVisuals for GamePlay");
            //Visual UI SetUp Game Play
            _healthBar.style.backgroundImage = _healthStatusStyle[_healthStatus.Count - 1];
            _gameOver.visible = false;
            _restartText.visible = false;
            _scoreLabel.visible = false;
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
        _startBTN = _rootMenu.Q<Button>("Start");
        _optionsBTN = _rootMenu.Q<Button>("Options");
        _quitBTN = _rootMenu.Q<Button>("Quit");
        _musicSlider = _rootMenu.Q<Slider>("Music");
        _soundSlider = _rootMenu.Q<Slider>("Sound");
    }
    private void ResetGame()
    {
        _isGameOver = false;
        UpdateUIVisuals();
        //_isGameOver = false;
        //_restartText.visible = false;
        InputManager.Instance.EnableRestart(false);
        _score = 0;
        SetSceneInputs();
        ResetLevel?.Invoke();
        //Enable player inputs on restart
        //disable UI inputs
    }
    private void OnDisable()
    {
        if (Instance != this) return;
        if (_UIDocGame || _UIDocMenu)
        {
            UpdateDisabled();
            UnregisterAllCallBacks();
        }
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
        _optionsBTN.clicked -= AudioEnableDisable;
        _startBTN.clicked -= LoadLevelOne;
        _quitBTN.clicked -= Application.Quit;
    }
    
}

public class EventManager
{
    private static readonly Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    public static void AddListener(string eventName, Delegate handler)
    {
        if (!eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] = null;
        }

        eventTable[eventName] = Delegate.Combine(eventTable[eventName], handler);
    }

    public static void RemoveListener(string eventName, Delegate handler)
    {
        if (eventTable.TryGetValue(eventName, out var existingHandler))
        {
            var newHandler = Delegate.Remove(existingHandler, handler);

            if (newHandler == null)
            {
                eventTable.Remove(eventName);
            }
            else
            {
                eventTable[eventName] = newHandler;
            }
        }
    }

    public static void RaiseEvent(string eventName, params object[] args)
    {
        if (eventTable.TryGetValue(eventName, out var handler))
        {
            handler?.DynamicInvoke(args);
        }
    }
}

public class EventManager2<T>
{
    private event Action<T> handlers;

    private HashSet<Action<T>> handlerSet = new HashSet<Action<T>>();

    public void AddHandler(Action<T> handler)
    {
        if (handler == null)
        {
            return;
        }

        if (handlerSet.Contains(handler))
        {
            return;
        }

        handlers += handler;
        handlerSet.Add(handler);
    }

    public void RemoveHandler(Action<T> handler)
    {
        if (handler == null)
        {
            return;
        }

        if (!handlerSet.Contains(handler))
        {
            return;
        }

        handlers -= handler;
        handlerSet.Remove(handler);
    }

    public void Invoke(T arg)
    {
        handlers?.Invoke(arg);
    }
}

