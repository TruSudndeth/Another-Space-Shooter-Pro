using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{ //Debug: Fix Access to this script to be more secure
    public static InputManager Instance;
    private MyBaseInputs _playerInputs;
    
    [HideInInspector] public InputAction WSAD;
    [HideInInspector] public InputAction Fire;
    [HideInInspector] public InputAction Restart;

    [SerializeField] private bool enableInputs = false;

    private void Awake()
    {
        _playerInputs = new();
        WSAD = _playerInputs.Player.Move;
        Fire = _playerInputs.Player.Fire;
        Restart = _playerInputs.UI.Restart;
        if (Instance)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Update()
    {
        if(enableInputs)
        {
            enableInputs = false;
            EnablePlayerIO();
        }
    }
    public void EnableRestart()
    {
        DisablePlayerIO();
        Restart.Enable();
    }
    public void DisableRestart()
    {
        Restart.Disable();
    }
    public void EnablePlayerIO()
    {
        DisableRestart();
        WSAD.Enable();
        Fire.Enable();
    }
    public void DisablePlayerIO()
    {
        WSAD.Disable();
        Fire.Disable();
    }
    private void OnDisable()
    {
        Restart.Disable();
        WSAD.Disable();
        Fire.Disable();
    }
}
