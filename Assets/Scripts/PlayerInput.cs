using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private MyBaseInputs playerInputs;
    private Vector2 movePlayer;
    [SerializeField] private float speed = 1;
    [SerializeField] private InputAction WSAD; //Debugit unSerialize private fild

    // Add Some movement interperlation Plz (Mooth out)
    void Start()
    {
        playerInputs = new(); //Create a new instance of MyBaseInputs
        WSAD = playerInputs.Player.Move;
        WSAD.Enable();
    }
    private void OnDestroy()
    {
        WSAD.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = WSAD.ReadValue < Vector2> ();
        if (movement == Vector2.zero) // a bit redundent :)
        {
            movement = Vector2.zero;
        }
        movePlayer = movement;
    }

    private void FixedUpdate()
    {
        // Might not need to set translate if there is no input hmmmm
        transform.Translate(movePlayer * speed * Time.fixedDeltaTime, Space.World); //Moves the transfomr in the direction and distance of translation
    }
}
