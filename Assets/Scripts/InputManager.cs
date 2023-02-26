using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : DontDestroyHelper<InputManager>
{ //Debug: Fix Access to this script to be more secure
    private MyBaseInputs _playerInputs;
    
    [HideInInspector] public InputAction WSAD { get; private set; }
    [HideInInspector] public InputAction Fire { get; private set; }
    [HideInInspector] public InputAction Restart { get; private set; }
    [HideInInspector] public InputAction Exit { get; private set; }
    [HideInInspector] public InputAction Thrust { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        _playerInputs = new();

        Thrust = _playerInputs.Player.Thrust;
        Exit = _playerInputs.UI.Exit;
        WSAD = _playerInputs.Player.Move;
        Fire = _playerInputs.Player.Fire;
        Restart = _playerInputs.UI.Restart;

        Exit.Enable();
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
    public void UIDisabled(bool enabled = false)
    {
        if (enabled)
        {
            EnablePlayerIO(true);
        }
        else
        {
            DisableAllPlayerInputs();
        }
    }
    public void EnablePlayerIO(bool enable)
    {
        if (enable)
        {
            EnableRestart(false);
            Thrust.Enable();
            WSAD.Enable();
            Fire.Enable();
        }
        else
        {
            Thrust.Disable();
            WSAD.Disable();
            Fire.Disable();
        }
    }
    private void OnDestroy()
    {
        DisableAllPlayerInputs();
        Exit.Disable();
    }
    private void DisableAllPlayerInputs()
    {
        Thrust.Disable();
        Restart.Disable();
        WSAD.Disable();
        Fire.Disable();
    }
}
