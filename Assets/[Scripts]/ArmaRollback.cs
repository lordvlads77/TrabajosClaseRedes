using System.Collections;
using System.Collections.Generic;
using FishNet.Component.ColliderRollback;
using FishNet.Managing.Timing;
using FishNet.Object;
using UnityEngine;

public class ArmaRollback : NetworkBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Le mandamos el tick del ultimo paquete recibido del servidor 
            FireServerRPC(TimeManager.GetPreciseTick(TickType.LastPacketTick));
        }
    }

    [ServerRpc]
    void FireServerRPC(PreciseTick tick)
    {
        //FishNet va a regresar en el tiempo los colliders
        base.RollbackManager.Rollback(tick, RollbackPhysicsType.Physics, base.IsOwner);
        
        Debug.DrawRay(transform.position, transform.forward * 50f, Color.blue, 10f);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            print("El juagador impacto en: " + hit.collider);
        }
        //Ya que terminamos le pedimos que los regrese al tiempo actual
        RollbackManager.Return();
    }
}
