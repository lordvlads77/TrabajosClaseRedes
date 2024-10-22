using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Bala : NetworkBehaviour
{
    float tiempoDestroy = 5f;
    public readonly SyncVar<int> _duenioId = new SyncVar<int>(); //Id del jugador que lo disparo

    [Server (Logging = LoggingType.Off)]
    private void Update()
    {
        tiempoDestroy -= Time.deltaTime;
        if (tiempoDestroy <= 0)
        { 
            Despawn(gameObject);
        }
    }
}
