using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public delegate void Reset();
    public static Reset _resetLevel;
    public delegate void LoadScene(Type.GameState index);
    public static LoadScene _loadScene;
        
    [Space]
    private UIDocument _UIDoc;
    [SerializeField] private VisualTreeAsset _mainMenu_UI;
    [SerializeField] private VisualTreeAsset _gamePlay_UI;
    private Type.GameState _gameState; 
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
    private MyBaseInputs _UIbaseInputs;
    private InputAction _restartInput;
    private bool _hasRestarted = false;

    private void Start()
    {
        if(!_isMainMenu)
        {
            _restartInput.started += _ => _hasRestarted = true;

            PlayerInput.Score += UpdateScore;
            PlayerInput.UpdateHealth += UpdateHealth;
            PlayerInput.gameOver += GameOver;

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
        }
        else
        {
            Debug.Log("Fix UI Settings");
        }
    }
    private void LoadLevelOne()
    {
        _gameState = Type.GameState.Level1;
        _loadScene?.Invoke(_gameState);
    }
    private void AudioEnableDisable()
    {
        DisplayStyle isVisible = _musicSlider.style.display.value == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        _musicSlider.style.display = isVisible;
        _soundSlider.style.display = isVisible;
    }
    private void OnDisable()
    {
        if(!_isMainMenu)
        {
            _restartInput.started -= _ => _hasRestarted = false;
            _restartInput.Disable();
            PlayerInput.Score -= UpdateScore;
            PlayerInput.UpdateHealth -= UpdateHealth;
            PlayerInput.gameOver -= GameOver;
        }else if(_isMainMenu)
        {
            _optionsBTN.clicked -= AudioEnableDisable;
            _startBTN.clicked -= LoadLevelOne;
            _quitBTN.clicked -= Application.Quit;
        }
    }
    private void Update()
    {
        if(_hasRestarted)
        {
            _resetLevel?.Invoke();
            _hasRestarted = false;
            _restartInput.Disable();
        }
    }
    private void FixedUpdate()
    {
        if (_gameOverTime + _gameOverFlashTime < Time.time && _isGameOver && !_isMainMenu)
        {
            _gameOverTime = Time.time;
            _gameOver.style.visibility = _gameOver.style.visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
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
    private void GameOver()
    {
        if(!_isMainMenu)
        {
            //create a on off logic over time
            _isGameOver = true;
            _restartText.visible = true;
            _restartInput.Enable();
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
            _UIDoc = uiDoc;
        }
        else Debug.Log("No UI Document Component" + transform);
        
        _isMainMenu = _UIDoc.visualTreeAsset == _mainMenu_UI ? true : false;
        root = _UIDoc.rootVisualElement;
        

        if (!_isMainMenu)
        {
            
            _UIbaseInputs = new();
            _restartInput = _UIbaseInputs.UI.Restart;
            _healthStatusStyle = new(4);
            for (int i = 0; i < _healthStatus.Count; i++)
            {
                _healthStatusStyle.Add(new StyleBackground(_healthStatus[i]));
            }
        }
        if (!_isMainMenu)
        {
            //Reff
            _UIDoc.visualTreeAsset = _gamePlay_UI;
            root = _UIDoc.rootVisualElement;
            _healthBar = root.Q<VisualElement>("HealthBar");
            _scoreLabel = root.Q<Label>("Score");
            _gameOver = root.Q<Label>("GameOver");
            _restartText = root.Q<Label>("Restart_Text");

            //Set
            _healthBar.style.backgroundImage = _healthStatusStyle[_healthStatus.Count - 1];
            _gameOver.visible = false;
            _restartText.visible = false;
        }
    }
}