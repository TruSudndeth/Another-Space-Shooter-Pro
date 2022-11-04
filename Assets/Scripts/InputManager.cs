using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{ //Debug: Fix Access to this script to be more secure
    public static InputManager Instance;
    private MyBaseInputs _playerInputs;
    
    [HideInInspector] public InputAction WSAD { get; private set; }
    [HideInInspector] public InputAction Fire { get; private set; }
    [HideInInspector] public InputAction Restart { get; private set; }
    [HideInInspector] public InputAction Exit { get; private set; }

    private void Awake()
    {
        _playerInputs = new();
        Exit = _playerInputs.UI.Exit;
        WSAD = _playerInputs.Player.Move;
        Fire = _playerInputs.Player.Fire;
        Restart = _playerInputs.UI.Restart;

        Exit.Enable();
        
        if (Instance)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void EnableExit(bool enable)
    {
        if (enable)
            Exit.Enable();
        else
            Exit.Disable();
    }
    public void EnableRestart(bool enable)
    {
        if (enable)
        {
            EnablePlayerIO(false);
            Restart.Enable();
        }
        else
            Restart.Disable();
    }
    public void EnablePlayerIO(bool enable)
    {
        if (enable)
        {
            EnableRestart(false);
            WSAD.Enable();
            Fire.Enable();
        }
        else
        {
            WSAD.Disable();
            Fire.Disable();
        }
    }
    private void OnDisable()
    {
        Exit.Disable();
        Restart.Disable();
        WSAD.Disable();
        Fire.Disable();
    }
}
