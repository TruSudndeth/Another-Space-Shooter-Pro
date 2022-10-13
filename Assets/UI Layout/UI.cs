using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        Button buttonStart = root.Q<Button>("ButtonStart");
        Button buttonRestart = root.Q<Button>("ButtonRestart");
        Button buttonQuit = root.Q<Button>("ButtonQuit");

        buttonStart.clicked += () => Debug.Log("Start was Clicked");
        buttonRestart.clicked += () => Debug.Log("Restart was Clicked");
        buttonQuit.clicked += () => Debug.Log("Quit was Clicked");
    }
}
