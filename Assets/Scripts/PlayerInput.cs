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
        Vector2 movement = WSAD.ReadValue <Vector2>();
        movement = CalculateMove(movement); // with bounds

        movePlayer = movement;
    }

    private Vector2 CalculateMove(Vector2 _movement)
    {
        _movement *= speed * Time.fixedDeltaTime;
        if (Mathf.Abs(transform.position.x + _movement.x) > xBounds)
        {
            _movement.x = (xBounds * Mathf.Sign(_movement.x)) - transform.position.x;
        }
        if (Mathf.Abs(transform.position.y + _movement.y) > yBounds)
        {
            _movement.y = (yBounds * Mathf.Sign(_movement.y)) - transform.position.y;
        }
        return _movement;
    }

    private void FixedUpdate()
    {
        // Might not need to set translate if there is no input hmmmm
        transform.Translate(movePlayer, Space.World); //Moves the transfomr in the direction and distance of translation
    }
}
