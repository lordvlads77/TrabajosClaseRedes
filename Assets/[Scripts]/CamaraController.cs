using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public Transform targetfollow;
    public Vector3 offset;
    void Start()
    {
        GameEvents.instance.OnLocalPlayerSpawn.AddListener(OnLocalPlayerSpawned);
    }

    private void OnDestroy()
    {
        if (GameEvents.instance)
        {
            GameEvents.instance.OnLocalPlayerSpawn.RemoveListener(OnLocalPlayerSpawned);
        }
    }

    private void LateUpdate()
    {
        if (!targetfollow)
        {
            return;
        }

        Vector3 pos = targetfollow.position;
        pos += offset;
        transform.position = pos;
    }

    void Update()
    {
        
    }

    void OnLocalPlayerSpawned()
    {
        targetfollow = PersonajeNetwork.LocalPersonaje.transform;
    }
}
