using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Logging;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Canion : NetworkBehaviour
{
    [FormerlySerializedAs("bala")] public GameObject balaPrefab;
    public float velocidadBala = default;

    [Server (Logging = LoggingType.Off)]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DispararRPC(TimeManager.Tick, Vector3.up);
            DispararRPC(TimeManager.Tick, Vector3.right);
        }
    }
    //Version 2
    [ObserversRpc(RunLocally = true)]
    void DispararRPC(uint tickDisparo, Vector3 dir)
    {
        float passedTime = (float) TimeManager.TimePassed(tickDisparo);
        GameObject newBala = Instantiate(balaPrefab, transform.position, Quaternion.identity);
        BalaPrediccion balaPrediccion = newBala.GetComponent<BalaPrediccion>();
        balaPrediccion.passedTime = passedTime;
        balaPrediccion.direction = dir;
    }
    
    //Version 1
    // [ObserversRpc(RunLocally = true)]
    // void DispararRPC(uint tickDisparo, Vector3 dir)
    // {
    //     float passedTime = (float) TimeManager.TimePassed(tickDisparo);
    //     //Calculamos la posicion que en teoria esta en el servidor.
    //     Vector3 position = transform.position + Vector3.up * velocidadBala * passedTime;
    //     GameObject newBala = Instantiate(balaPrefab, transform.position, Quaternion.identity);
    //     newBala.GetComponent<Rigidbody>().velocity = dir * velocidadBala; 
    // }
}
