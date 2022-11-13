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

public class UI : MonoBehaviour
{
    public delegate void Reset();
    public static Reset ResetLevel;
    public delegate void LoadScene(Types.GameState index);
    public static LoadScene Load_Scene;
        
    [Space]
    private UIDocument _docUI;
    [SerializeField] private VisualTreeAsset _mainMenu_UI;
    [SerializeField] private VisualTreeAsset _gamePlay_UI;
    private Types.GameState _gameState; 
    private bool _isMainMenu = false;

    [Space]
    private Button _startBTN;
    private Button _optionsBTN;
    private Button _quitBTN;
    private Slider _musicSlider;
    private Slider _soundSlider;
    private float _musicVolume = 0;
    private float _soundVolume = 0;

    [Space]
    private VisualElement root;
    private VisualElement _healthBar;
    private int _score = 0;
    private Label _scoreLabel;
    private Label _ammo;
    
    [Space]
    [SerializeField] private float _gameOverFlashTime = 1.0f;
    private Label _gameOver;
    private float _gameOverTime = 0.0f;
    private bool _isGameOver = false;
    private bool _gameStarted = false; //DeleteLine: not Used ???
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
    //Todo: Listen for player thruster event

    private void Start()
    {
        if(!_isMainMenu)
        {
            InputManager.Instance.Restart.started += _ => _hasRestarted = true;
            InputManager.Instance.EnablePlayerIO(true);

            BackGroundMusic_Events.BGM_Events += () => _thrustBPM = true;
            Player.Thruster += ThrusterCoolDown;
            Player.UpdateAmmo += UpdateAmmo;
            Player.Score += UpdateScore;
            Player.UpdateHealth += UpdateHealth;
            Player.Game_Over += GameOver;
            StartGameAsteroids.GameStarted += GameStarted;

            UpdateScore(0);
        }else if (_isMainMenu)
        {
            //Do Basic Setup here.
            _startBTN = root.Q<Button>("Start");
            _optionsBTN = root.Q<Button>("Options");
            _quitBTN = root.Q<Button>("Quit");
            _musicSlider = root.Q<Slider>("Music");
            _soundSlider = root.Q<Slider>("Sound");

            _quitBTN.clicked += Application.Quit;
            _optionsBTN.clicked += AudioEnableDisable;
            _startBTN.clicked += LoadLevelOne;

            _startBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
            _optionsBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
            _quitBTN.RegisterCallback<MouseOverEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Hover));
            
            _startBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
            _optionsBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
            _quitBTN.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlayAudioOneShot(Types.SFX.UI_Click));
            
            //Todo: Add Audio Settings
            //_musicSlider.RegisterValueChangedCallback((evt) => _musicVolume = evt.newValue);
            //_soundSlider.RegisterValueChangedCallback((evt) => _soundVolume = evt.newValue);
        }
        else
        {
            Debug.Log("Fix UI Settings");
        }
    }
    private void GameStarted()
    {
        _gameStarted = true; //DeleteLine: Variable not used ??
        _scoreLabel.visible = true;
    }
    private void LoadLevelOne()
    {
        //Play start button Audio
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
            ResetLevel?.Invoke();
            _hasRestarted = false;
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
        if(!_isMainMenu)
        _healthBar.style.backgroundImage = _healthStatusStyle[health];
    }
    
    private void OnEnable()
    {
        if (TryGetComponent(out UIDocument uiDoc))
        {
            _docUI = uiDoc;
        }
        else Debug.Log("No UI Document Component" + transform);
        
        _isMainMenu = _docUI.visualTreeAsset == _mainMenu_UI ? true : false;
        root = _docUI.rootVisualElement;
        

        if (!_isMainMenu)
        {
            //Debug: Delete 2 comments
            //_UIbaseInputs = new();
            //_restartInput = _UIbaseInputs.UI.Restart;
            _healthStatusStyle = new(4);
            for (int i = 0; i < _healthStatus.Count; i++)
            {
                _healthStatusStyle.Add(new StyleBackground(_healthStatus[i]));
            }
        }
        if (!_isMainMenu)
        {
            //Reff
            _docUI.visualTreeAsset = _gamePlay_UI;
            root = _docUI.rootVisualElement;

            _thrusterCoolDown = root.Q<ProgressBar>("ThrusterCoolDown");
            _healthBar = root.Q<VisualElement>("HealthBar");
            _scoreLabel = root.Q<Label>("Score");
            _gameOver = root.Q<Label>("GameOver");
            _restartText = root.Q<Label>("Restart_Text");
            _ammo = root.Q<Label>("Ammo");

            //SetUp
            _healthBar.style.backgroundImage = _healthStatusStyle[_healthStatus.Count - 1];
            _gameOver.visible = false;
            _restartText.visible = false;
            _scoreLabel.visible = false;

            _thrusterCoolDown.highValue = 1;
            _thrusterCoolDown.lowValue = 0;
            _thrusterCoolDown.value = 0;
            _thrusterColor = root.Q(className: "unity-progress-bar__progress");


            //unity-progress-bar__progress
            //unity-progress-bar__container
            //unity-progress-bar__background
            //unity-progress-bar__title-container


            
        }
    }
    private void OnDisable()
    {
        if (!_isMainMenu)
        {
            BackGroundMusic_Events.BGM_Events -= () => _thrustBPM = true;
            Player.Thruster -= ThrusterCoolDown;
            InputManager.Instance.Restart.started -= _ => _hasRestarted = true;
            InputManager.Instance.EnableRestart(false);
            Player.Score -= UpdateScore;
            Player.UpdateHealth -= UpdateHealth;
            Player.Game_Over -= GameOver;
            Player.UpdateAmmo -= UpdateAmmo;
        }
        else if(_isMainMenu)
        {
            StartGameAsteroids.GameStarted -= GameStarted;
            _optionsBTN.clicked -= AudioEnableDisable;
            _startBTN.clicked -= LoadLevelOne;
            _quitBTN.clicked -= Application.Quit;
        }
    }
}