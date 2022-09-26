using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    private bool move = false;
    private WaitForSeconds laserLife;

    private void Awake()
    {
        laserLife = new(5.0f);
    }
    private void OnEnable()
    {
        move = true;
        StartCoroutine("KillLaser");
        //move this transform in player script not ideal but fix later.
        //make laser a child to player then grab the parents transfom (spawn Point)
        //null check if there is a parent
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        move = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (move)
            transform.position += transform.up * speed * Time.fixedDeltaTime;
    }

    IEnumerator KillLaser()
    {
        yield return laserLife;
        transform.gameObject.SetActive(false);
    }
}
