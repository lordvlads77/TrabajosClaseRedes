using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[DefaultExecutionOrder(-50)]
public class GameEvents : MonoBehaviour
{
    public static GameEvents instance { get; private set; }
    public UnityEvent OnLocalPlayerSpawn;
    public UnityEvent OnLocalPlayerDespawn;

    private void Awake()
    {
        instance = this;
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
