using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private Vector3 _target = Vector3.zero;

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.fixedDeltaTime);
    }
}
