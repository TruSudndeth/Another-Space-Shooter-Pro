using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : DontDestroyHelper<InputManager>
{   //Debug: Fix Access to this script to be more secure
    //Found a bug that doesn't let me shoot when i move diagnal (-1, 1) (-1, -1) (1, -1)
    private MyBaseInputs _playerInputs;
    
    [HideInInspector] public InputAction Restart { get; private set; }
    [HideInInspector] public InputAction Thrust { get; private set; }
    [HideInInspector] public InputAction WSAD { get; private set; }
    [HideInInspector] public InputAction Fire { get; private set; }
    [HideInInspector] public InputAction Exit { get; private set; }

    protected override void Awake()
    {
        //Base inharetance has an instance This class
        base.Awake();
        
        _playerInputs = new();

        //Player
        Thrust = _playerInputs.Player.Thrust;
        WSAD = _playerInputs.Player.Move;
        Fire = _playerInputs.Player.Fire; 
        //UI
        Exit = _playerInputs.UI.Exit;
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
        //if (_playerInputs != null) _playerInputs.Disable();
        if (Restart != null)    Restart.Disable();
        if (Thrust != null)     Thrust.Disable();
        if (WSAD != null)       WSAD.Disable();
        if (Fire != null)       Fire.Disable();
    }
}
