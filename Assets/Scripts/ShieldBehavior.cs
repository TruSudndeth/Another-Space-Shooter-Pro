using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    //Todo: After shield is Disabled Decrease player movement for 3-5 seconds
    [SerializeField] private float _alphaSpeed = 1.5f;
    [SerializeField] private int _shieldHealth = 3;
    [Space]
    private Renderer _renderer;
    private Color _defaultColor;
    private Color _flashAlpha = Color.white;
    private float _shieldAlphaMultiplier;
    [Space]
    private int _health;
    private int _currentDamage = 0;
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _defaultColor = _renderer.material.color;
    }
    private void OnEnable()
    {
        ResetShield();
    }
    public void Damage(int damage)
    {
        damage = Mathf.Abs(damage);
        _health -= damage;
        _currentDamage += damage;
        if (_health <= 0)
        {
            AudioManager.Instance.PlayAudioOneShot(Types.SFX.ShieldOff);
            gameObject.SetActive(false);
        }
    }

    public void ResetShield()
    {
        Debug.Log("ResetShield"); //Delete: Testing Debug.Log
        _renderer.material.color = _defaultColor;
        _health = _shieldHealth;
        _currentDamage = 0;
        _shieldAlphaMultiplier = 1.0f / _shieldHealth;
    }
    // Update is called once per frame
    private void Update()
    {
        float alpha;
        if(_health != _shieldHealth)
        {
            alpha = _currentDamage * _shieldAlphaMultiplier;
            _flashAlpha.a = 1.0f - Mathf.PingPong(_alphaSpeed * Time.time, alpha);
            _renderer.material.color = _flashAlpha;
        }
    }
}
