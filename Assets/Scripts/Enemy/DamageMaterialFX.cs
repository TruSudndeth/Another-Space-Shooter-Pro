using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMaterialFX : MonoBehaviour
{
    //Bug: MotherShip reset Failed - then reset completed - after reaching a way point (waypoint 2).
    //Complete: VFX Boss Battle
    //Disable colliders when part is destroyed
    //SetMaterial to damaged after multi expolosion
    //- when destroied Trigger Explosion FX When done continue to the next task
    //- Disable Thruster VFX if any (check for script then disable)

    /// <summary>
    /// The material to use when the object is damaged.
    /// And the Event When the object is fully destroid
    /// 
    /// </summary>
    public delegate void OnObjectDestroyed(BossParts part);
    public static event OnObjectDestroyed onObjectDestroyed;

    [Header("Setup material Paramerters")]
    [SerializeField]
    private BossParts _motherShipPart;
    [SerializeField]
    private AnimationCurve _damageCurve;
    [SerializeField]
    private float _damageDuration = 1.0f;
    //Delete: _damageAnimationSpeed is never used ??
    //[SerializeField]
    //private float _damageAnimationSpeed = 1.0f;
    [SerializeField]
    private float _noiseScale = 1000.0f;
    [SerializeField]
    private Color _damageColor = Color.red;
    [SerializeField]
    private int _health = 10;
    private int _currentHealth;

    [Header("Part Functions")]
    [SerializeField]
    private BossPartFunction _bossPartFunctions;

    [Space(25)]
    [Header("Damage Material FX")]
    [SerializeField]
    private float _destroySpeedScaler = 0.1f;
    [SerializeField]
    private float _disolveAmount = 0.25f;
    [SerializeField]
    private float _edgeWidth = 0.05f;

    [Space]
    [Header("Not Damaged Materials")]
    [SerializeField]
    private float _noDisolveAmount = 0f;
    [SerializeField]
    private float _noEdgeWidth = 1f;
    
    private Color _originalColor;
    private bool _isDestroid = false;
    private float _damageAnimationTimer = 0.0f;
    private MeshRenderer _meshRenderer;
    
    public bool TakeDamage = false; //Delete Temp variable
    private void Awake()
    {
        _currentHealth = _health; //Delete: This health set is not used
        _bossPartFunctions = GetComponentInChildren<BossPartFunction>(true);
        if (TryGetComponent(out MeshRenderer meshRend))
            _meshRenderer = meshRend;
        else
            Debug.Log("MeshRender component Doesn't exist.", transform);
    }
    void Start()
    {
        if (_motherShipPart == BossParts.Beacon) SetupBeconShield();
        SetupMaterialProperties();
        BossFightManager.EnableStageColliderPart += EnableAllCollidersInChildren;
        BossExplosions.OnDisablePart += DisablePartFunction;
        BossColliderParts.OnBossColliderParts += DamagePart;
        BossFightManager.ResetBossEvent += ResetAll;

        BossFightManager.SetDifficulty += (x) => SetCurrentHealth(x);
    }
    private void SetupBeconShield()
    {
        if (!_beconShieldFX)
        {
            Debug.Log("Failed to initiate _beconShieldFX");
            _beconShieldFX = transform.GetComponentInChildren<ParticleSystem>().transform.parent;
            if (_beconShieldFX) 
                Debug.Log("_beconShieldFX was set");
            else
            {
                Debug.Log("Failed to set _beconShieldFX");
                return;
            }
        }
        _beconShieldFX.gameObject.SetActive(true);
        _hasShield = true;
    }
    private void SetCurrentHealth(int health)
    {
        _currentHealth = health;
        _health = health;
        if (_hasShield) _shieldHealth = health * 2;
    }

    private void DamagePart(BossParts part)
    {
        if (part == _motherShipPart)
        {
            if(_hasShield)
            {
                _shieldHealth--;
                if(_shieldHealth <= 0)
                {
                    _hasShield = false;
                    _beconShieldFX.gameObject.SetActive(false);
                }
                return;
            }
            DoDamage(1);
        }
    }
    //Todo: Shield has 2X helth then part health if 0 then its at least 1.
    private bool _hasShield = false;
    private int _shieldHealth = 1;
    [SerializeField]
    private Transform _beconShieldFX;
    private void DoDamage(int damage)
    {
        if (!_isDestroid)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _isDestroid = true;
                CheckObjectAssignment(_motherShipPart);
                onObjectDestroyed?.Invoke(_motherShipPart);
                return;
            }
            _damageAnimationTimer = Time.time;
        }
    }
    private void DisableAllCollidersInChildren(BossParts thisPart)
    {
        //Todo: Move this code to BossColliderParts
        CheckObjectAssignment(thisPart);
        if (thisPart == _motherShipPart)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
    private void EnableAllCollidersInChildren(BossParts thisPart)
    {
        CheckObjectAssignment(thisPart);
        if (thisPart == _motherShipPart)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }
    private void ResetAll()
    {
        _currentHealth = _health; //Delete: this health set is not used
        _isDestroid = false;
        if (_motherShipPart == BossParts.Beacon) SetupBeconShield();
        SetupMaterialProperties(false);
    }
    private void SetupMaterialProperties(bool isDamaged = false)
    {
        if(isDamaged)
        {
            //Setup on Destroied
            if (_meshRenderer.material.HasProperty("_EdgeWidth"))
                _meshRenderer.material.SetFloat("_EdgeWidth", _edgeWidth);
            else
                Debug.Log("Property Does not exist", transform);
            if (_meshRenderer.material.HasProperty("_EdgeColor"))
                _meshRenderer.material.color = _damageColor;
            else
                Debug.Log("Property Does not exist", transform);
            if (_meshRenderer.material.HasProperty("_DisolveAmount"))
                _meshRenderer.material.SetFloat("_DisolveAmount", _disolveAmount);
            else
                Debug.Log("Property Does not exist", transform);
        }
        else
        {
            //Setup On Start
            if(_meshRenderer.material.HasProperty("_EdgeWidth"))
                _meshRenderer.material.SetFloat("_EdgeWidth", _noEdgeWidth);
            else
                Debug.Log("Property Does not exist", transform);
            if(_meshRenderer.material.HasProperty("_EdgeColor"))
                _meshRenderer.material.SetColor("_EdgeColor", _originalColor);
            else
                Debug.Log("Property Does not exist", transform);
            if (_meshRenderer.material.HasProperty("_DisolveAmount"))
                _meshRenderer.material.SetFloat("_DisolveAmount", _noDisolveAmount);
            else
                Debug.Log("Property Does not exist", transform);
            if(_meshRenderer.material.HasProperty("_NoiseScale"))
                _meshRenderer.material.SetFloat("_NoiseScale", _noiseScale);
            else
                Debug.Log("Property Does not exist", transform);

            _damageAnimationTimer -= _damageDuration;
        }
        
    }
    void FixedUpdate()
    {
        if (_damageAnimationTimer + _damageDuration >= Time.time && !_isDestroid)
        {
            float t = (Time.time - _damageAnimationTimer) / _damageDuration;
            float curveValue = _damageCurve.Evaluate(t);
            _meshRenderer.material.SetColor("_EdgeColor", Color.Lerp(_originalColor, _damageColor, curveValue));
        }
        if(_isDestroid)
        {
            //pulse material between _originalColor and _damageColor by Animation curve
            //and repeat the animation for ever
            float t = ((Time.time - _damageAnimationTimer) * _destroySpeedScaler) / _damageDuration;
            float curveValue = _damageCurve.Evaluate(t);
            if (t >= 1.0f)
            {
                _damageAnimationTimer = Time.time;
            }
            _meshRenderer.material.SetColor("_EdgeColor", Color.Lerp(_originalColor, _damageColor, curveValue));
            //Iterate threw the explosion list
            //and send an event to BossFightManger of the part that was destroyed.
        }
    }
    private void CheckObjectAssignment(BossParts part)
    {
        if(part == BossParts.None)
        {
            Debug.Log("Part was not set when called", transform);
        }
    }
    private void DisablePartFunction(BossParts part)
    {
        if (part == _motherShipPart)
        {
            if(_bossPartFunctions)
            _bossPartFunctions.DisablePartFunction();
            SetupMaterialProperties(true);
            DisableAllCollidersInChildren(_motherShipPart);
        }
    }
    //Todo: Reset All for MotherShip When restarting
    private void OnDisable()
    {
        BossFightManager.EnableStageColliderPart -= EnableAllCollidersInChildren;
        BossExplosions.OnDisablePart -= DisablePartFunction;
        BossColliderParts.OnBossColliderParts -= DamagePart;
        BossFightManager.ResetBossEvent -= ResetAll;

        BossFightManager.SetDifficulty -= (x) => SetCurrentHealth(x);
    }
}
public enum BossParts
{
    //Note: Only the left of the ship is used
    None = 0, Beacon = 1, Body = 2, LeftWing = 3, RightWing = 4, LeftEngine = 5, RightEngine = 6, SM_Thruster_01 = 7, SM_Thruster_02 = 8,
    SM_Thruster_03 = 9, SM_Thruster_04 = 10
}
