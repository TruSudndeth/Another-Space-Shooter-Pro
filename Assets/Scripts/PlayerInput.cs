using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public delegate void GameOver();
    public static GameOver gameOver;
    public int Health { private get { return health; } set { health -= value; } }

    private MyBaseInputs playerInputs;
    private Vector2 movePlayer;
    [SerializeField] private float speed = 1;
    private InputAction WSAD;
    private InputAction fire;
    [SerializeField] private bool fired = false;
    [Space]
    [SerializeField] private Transform myCamera; //set in the inspector.
    [SerializeField] private Vector3 Offset = new Vector3(0, 0.8f, 0);
    [SerializeField] private float _singeFireRate = 0.25f;
    [SerializeField] private float _trippleFireRate = 0.5f;
    [SerializeField] private bool _isTrippleShot = false;
    [SerializeField] private float _powerUpTimeout = 5.0f;
    private float _powerUpTime = 0.0f;
    private List<Transform> _tripleShot;
    private float canFire = 0;
    private float cameraOrthoSize = 5;
    private float cameraAspectRatio = 1.7777778f;
    private float xBounds = 0;
    private float yBounds = 0;
    private Vector2 xyBounds = Vector2.zero;
    [Space]
    [SerializeField] private Transform _laserPool;
    [SerializeField] private Transform _laserAsset;
    [SerializeField] private Transform _primaryLaserSpawn;
    [SerializeField] private Transform _dualLaser_LSpawn;
    [SerializeField] private Transform _dualLaser_RSpawn;
    [SerializeField] private List<Transform> lasers;
    [SerializeField] private int maxPool = 15;
    [SerializeField] private int iterateLaser = 0; //Debugit remove serialized field
    private bool isPoolMaxed = false;
    [SerializeField] private int health = 3;
    [Space]
    [SerializeField] private float maxBank = 45.0f;
    [SerializeField] private float bankSpeed = 10.0f;
    private float bank = 0;

    private void Awake()
    {
        playerInputs = new(); //Create a new instance of MyBaseInputs
        lasers = new(maxPool); // create a fixed size for performance reasons on Awake
        _tripleShot = new(){_primaryLaserSpawn, _dualLaser_LSpawn, _dualLaser_RSpawn};
    }

    // Add Some movement interperlation Plz (Smooth out)
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
        Vector2 movement = WSAD.ReadValue<Vector2>();
        //Bank left and right
        float _bankSpeed = Time.deltaTime * bankSpeed;
        if(movement.x < 0)
        bank = bank >= 1 ? 1 : bank + _bankSpeed; //bank left
        if(movement.x > 0)
        bank = bank <= -1 ? -1 : bank - _bankSpeed; //bank right
        if(movement.x == 0)
        if(Mathf.Abs(bank) > _bankSpeed * 2)
        bank = bank > 0 ? bank - _bankSpeed: bank + _bankSpeed; //Zero
        transform.rotation = Quaternion.Euler(-90, 0, maxBank * bank);

        //Access Z value from transform             check
        //apply rotation to Z with clamp -          check
        //zero out bank when no input (smooth) -    check
        //smooth out banks -                        Lets Incorperate

        //Bounds set
        //Fix bound clipping due to frame rate.
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

        if (_powerUpTime + _powerUpTimeout <= Time.time && _isTrippleShot)
            _isTrippleShot = false;
    }
    public void TripleShotActive()
    {
        _isTrippleShot = true;
        _powerUpTime = Time.time;
    }
    private void LaserPool()
    {
        int tripleShotIndex = 0;
        fired = false;
        if (canFire + _singeFireRate > Time.time) return;
        canFire = Time.time;
        if (lasers.Count < maxPool && !isPoolMaxed)
        {
            for (int i = 0; i < maxPool; i++)
            {
                if (tripleShotIndex > 2) break; //Logic Breaks the max pool (Must Fix)
                lasers.Add(Instantiate(_laserAsset, _tripleShot[tripleShotIndex].position + Offset, Quaternion.identity, _laserPool));

                iterateLaser++;
                if (iterateLaser == maxPool)
                {
                    iterateLaser = 0;
                    isPoolMaxed = true;
                }

                if(_isTrippleShot)
                {
                    tripleShotIndex++;
                    continue;
                }
                break;
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
                    if (tripleShotIndex > 2) break;
                    lasers[i].gameObject.SetActive(true);
                    lasers[i].position = _tripleShot[tripleShotIndex].position + Offset;
                    
                    if (_isTrippleShot)
                    {
                        tripleShotIndex++;
                        continue;
                    }
                    break;
                }
            }
        }
    }

    private void PopulateShots(List<Transform> totalShots)
    {

        //lasers[i].gameObject.SetActive(true);
        //lasers[i].position = _primaryLaserSpawn.position + Offset;
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
        gameOver?.Invoke(); //Null Reference exception?
        WSAD.Disable();
        fire.Disable();
    }


}
