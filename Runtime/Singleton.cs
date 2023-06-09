﻿using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    Debug.LogWarning("<color=red>An instance of " + typeof(T) + " is needed in the scene, but there is none.</color>");
                }
            }
            return instance;
        }
    }
}