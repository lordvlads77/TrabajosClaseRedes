using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Bandera : NetworkBehaviour
{
    private Transform follow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!follow) //No tengo jugador a seguir, no movemos bandera.
        {
            return;
        }
        bool isOwner = false;
        Debug.Log($"Owner: {Owner}, OwnerId: {OwnerId}, isOwner: {IsOwner}");
        if (base.IsServerStarted) // este es el servidor
        {

            if (OwnerId == -1)
            {
                isOwner = true;
            }
            
            /*if (Owner != null) // Si el dueño es null y es codigo del servidor, significa que el dueño es el servidor
            {
                return; // El servidor no hace nada porque lo esta controlando un cliente
            }*/
        }
        if (base.IsClientStarted)
        {
            if (base.IsOwner)
            {
                isOwner = true;
            }
        }
        if (isOwner == false)
        {
            return;
        }
        /*else //Es un cliente
        {
            if (base.IsOwner == false)
            {
                return;
            }
        }*/

        transform.position = follow.position + Vector3.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        follow = other.transform;
        if (base.IsServerStarted)
        {
            base.GiveOwnership(other.GetComponent<NetworkObject>().Owner);
        }
        //base.RemoveOwnership(); Servidor regresa a ser dueño del objeto. 
    }
}
