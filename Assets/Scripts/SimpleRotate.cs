using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    [SerializeField] private Vector3 _direcition;
    [SerializeField] private float _speed = 1;
    void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            transform.Rotate(_direcition * _speed * Time.fixedDeltaTime);
        }
    }
}
