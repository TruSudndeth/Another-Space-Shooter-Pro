using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleForwards : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Vector3 _direction = Vector3.forward;
    void FixedUpdate()
    {
        transform.position += _speed * Time.deltaTime * _direction;
    }
}
