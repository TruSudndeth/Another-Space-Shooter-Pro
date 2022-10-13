using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private int _score = 0;
    private Label _scoreLabel;
    private void Start()
    {
        PlayerInput.Score += UpdateScore;
        UpdateScore(0);
    }
    private void OnDisable()
    {
        PlayerInput.Score -= UpdateScore;
    }

    private void UpdateScore(int points)
    {
        _score = points;
        //Might Not Update current score from player at start
        //label is not being found (Script Execution order)
        _scoreLabel.text = "Score: " + _score;
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _scoreLabel = root.Q<Label>("Score");
    }
}