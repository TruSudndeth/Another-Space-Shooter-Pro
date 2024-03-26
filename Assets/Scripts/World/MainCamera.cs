using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : DontDestroyHelper<MainCamera>
{
#if UNITY_EDITOR
    private void OnDestroy()
    {
        Debug.Log("Camera was destroyed");
    }
#endif
}
