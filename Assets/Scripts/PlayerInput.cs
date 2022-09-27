using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public delegate void GameOver();
    public static GameOver gameOver;

    private MyBaseInputs playerInputs;
    private Vector2 movePlayer;
    [SerializeField] private float speed = 1;
    private InputAction WSAD;
    private InputAction fire;
    [SerializeField] private bool fired = false;
    [Space]
    [SerializeField] private Transform myCamera; //set in the inspector.
    [SerializeField] private Vector3 Offset = new Vector3(0, 0.8f, 0);
    [SerializeField] private float fireRate = 0.25f;
    private float canFire = 0;
    private float cameraOrthoSize = 5;
    private float cameraAspectRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;
    private Vector2 xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private Transform LaserAsset;
    [SerializeField] private List<Transform> lasers;
    [SerializeField] private int maxPool = 10;
    [SerializeField] private int iterateLaser = 0; //Debugit remove serialized field
    private bool isPoolMaxed = false;
    [SerializeField] private int health = 3;

    private void Awake()
    {
        playerInputs = new(); //Create a new instance of MyBaseInputs
        lasers = new(10); // create a fixed size for performance reasons on Awake
    }

    // Add Some movement interperlation Plz (Mooth out)
    void Start()
    {
        cameraOrthoSize = myCamera.GetComponent<Camera>().orthographicSize;
        xBounds = cameraOrthoSize * cameraAspectRatio;
        yBounds = cameraOrthoSize;
        xyBounds = new Vector2(xBounds, yBounds);
        EnableInputs();
        SubscribeToInputs();
    }

    // Update is called once per frame
    void Update()
    {
        //Bounds set
        //Fix bound clipping due to frame rate.
        Vector2 movement = WSAD.ReadValue<Vector2>();
        movement *= Time.fixedDeltaTime * speed;
        movement = OutOfBounds.CalculateMove(transform, movement, xyBounds); // with bounds

        movePlayer = movement;
    }

    private void FixedUpdate()
    {
        if (fired) LaserPool();

        if(health <= 0) gameObject.SetActive(false); // DebugIt look for Disable Bugs
        // Might not need to set translate if there is no input hmmmm
        transform.Translate(movePlayer, Space.World); //Moves the transfomr in the direction and distance of translation
    }

    private void LaserPool()
    {
        fired = false;
        if (canFire + fireRate > Time.time) return;
        canFire = Time.time;
        if (lasers.Count < maxPool && !isPoolMaxed)
        {
            lasers.Add(Instantiate(LaserAsset, transform.position + Offset, Quaternion.identity, transform.parent));
            iterateLaser++;
            if (iterateLaser == maxPool)
            {
                iterateLaser = 0;
                isPoolMaxed = true;
            }
        }
        else if (isPoolMaxed)
        {
            //fire rate must not surpass laser pool check if object is disabled before using.
            //Lock rotations add recochet later
            for (int i = 0; i < lasers.Count; i++)
            {
                if (!lasers[i].gameObject.activeSelf)
                {
                    Debug.Log("this code is running");
                    lasers[i].gameObject.SetActive(true);
                    lasers[i].position = transform.position + Offset;
                    break;
                }
            }
        }
    }

    private void EnableInputs()
    {
        WSAD = playerInputs.Player.Move;
        WSAD.Enable();
        fire = playerInputs.Player.Fire;
        fire.Enable();
    }

    private void SubscribeToInputs()
    {
        fire.performed += _ => fired = true;
    }
    private void OnDisable()
    {
        gameOver(); //Null Reference exception?
        WSAD.Disable();
        fire.Disable();
    }

    public int Health { private get { return health; } set { health -= value; } }
}
