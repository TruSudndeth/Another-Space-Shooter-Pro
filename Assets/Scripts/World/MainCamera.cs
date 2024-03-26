using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : DontDestroyHelper<MainCamera>
{
    public Transform CameraTransform { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        //Debug: Camera Refference might be too slow in start might switch to Awake
        CameraTransform = this.transform;
    }
}
