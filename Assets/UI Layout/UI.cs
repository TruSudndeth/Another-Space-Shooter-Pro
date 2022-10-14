using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private VisualElement _healthBar;
    private int _score = 0;
    private Label _scoreLabel;
    [Space]
    [SerializeField] private float _gameOverFlashTime = 1.0f;
    private Label _gameOver;
    private float _gameOverTime = 0.0f;
    private bool _isGameOver = false;
    [Space]
    [SerializeField] private List<Sprite> _healthStatus;
    private List<StyleBackground> _healthStatusStyle;

    private void Awake()
    {
        _healthStatusStyle = new(4);
        for (int i = 0; i < _healthStatus.Count; i++)
        {
            _healthStatusStyle.Add(new StyleBackground(_healthStatus[i]));
        }
    }
    private void Start()
    {
        PlayerInput.Score += UpdateScore;
        PlayerInput.UpdateHealth += UpdateHealth;
        PlayerInput.gameOver += GameOver;

        UpdateScore(0);
    }
    private void OnDisable()
    {
        PlayerInput.Score -= UpdateScore;
        PlayerInput.UpdateHealth -= UpdateHealth;
        PlayerInput.gameOver -= GameOver;
    }

    private void FixedUpdate()
    {
        if (_gameOverTime + _gameOverFlashTime < Time.time && _isGameOver)
        {
            _gameOverTime = Time.time;
            _gameOver.style.visibility = _gameOver.style.visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private void UpdateScore(int points)
    {
        _score = points;
        //Might Not Update current score from player at start
        //label is not being found (Script Execution order)
        _scoreLabel.text = "Score: " + _score;
    }
    private void GameOver()
    {
        //create a on off logic over time
        _isGameOver = true;
    }

    private void UpdateHealth(int health)
    {
        _healthBar.style.backgroundImage = _healthStatusStyle[health];
    }
    
    private void OnEnable()
    {
        //Reff
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _healthBar = root.Q<VisualElement>("HealthBar");
        _scoreLabel = root.Q<Label>("Score");
        _gameOver = root.Q<Label>("GameOver");

        //Set
        _healthBar.style.backgroundImage = _healthStatusStyle[_healthStatus.Count - 1];
        _gameOver.visible = false;
    }
}