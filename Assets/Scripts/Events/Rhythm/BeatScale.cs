using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScale : MonoBehaviour
{
    [Tooltip("0-100 The scale of the object when the beat is hit")]
    [SerializeField] private float _scale;
    private Vector3 _targetScale;
    private Vector3 _initialScale;
    private bool _isScaling = false;
    private void Start()
    {
        _initialScale = transform.localScale;
        float _setScale = _scale / 100 + 1;
        _targetScale = transform.localScale * _setScale;
        BackGroundMusic_Events.BGMEvents += ScaleBeat;
    }
    private void OnDisable()
    {
        BackGroundMusic_Events.BGMEvents -= ScaleBeat;
    }
    private void ScaleBeat()
    {
        _isScaling = !_isScaling;
        if (_isScaling)
            transform.localScale = _initialScale;
        else
            transform.localScale = _targetScale;
    }
}
