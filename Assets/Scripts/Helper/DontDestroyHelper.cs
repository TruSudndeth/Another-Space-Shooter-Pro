using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyHelper<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Check if an object of this type already exists in the scene
                _instance = FindObjectOfType<T>();

                // If not, create a new object
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }

                // Don't destroy the object when loading a new scene
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    // Override this method in derived classes to implement custom behavior on Awake
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
