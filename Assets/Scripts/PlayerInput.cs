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
    [SerializeField] private Transform myCamera; //set in the inspector.
    private float cameraOrthoSize = 5;
    private float cameraAspectRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;

    // Add Some movement interperlation Plz (Mooth out)
    void Start()
    {
        cameraOrthoSize = myCamera.GetComponent<Camera>().orthographicSize;
        xBounds = cameraOrthoSize * cameraAspectRatio;
        yBounds = cameraOrthoSize;
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
        //Bounds set
        //Fix bound clipping due to frame rate.
        Vector2 movement = WSAD.ReadValue < Vector2> ();
        if (movement != Vector2.zero) // Not so redundent now :)
        {
            if (transform.position.x > xBounds && movement.x > 0)
            {
                movement.x = 0;
            }else if(transform.position.x < -xBounds && movement.x < 0)
            {
                movement.x = 0;
            }

            if(transform.position.y > yBounds && movement.y > 0)
            {
                movement.y = 0;
            }
            else if(transform.position.y < -yBounds && movement.y < 0)
            {
                movement.y = 0;
            }
        }
        movePlayer = movement;
    }

    private void FixedUpdate()
    {
        // Might not need to set translate if there is no input hmmmm
        transform.Translate(movePlayer * speed * Time.fixedDeltaTime, Space.World); //Moves the transfomr in the direction and distance of translation
    }
}
