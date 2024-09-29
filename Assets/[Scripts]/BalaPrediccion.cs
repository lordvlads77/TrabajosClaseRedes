using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

public class BalaPrediccion : MonoBehaviour
{
    public float passedTime;
    public float velocidad = 4f;
    public Vector3 direction;
    
    Rigidbody elrigido;

    void Start()
    {
        elrigido = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float step = 0f;
        if (passedTime > 0f) // Tenemos que agregarle velocidad extra?
        {
            step = passedTime * 0.03f; //la bala va a ir 7% más rapido de su velocidad
            passedTime -= step; //Reducimos al timepo, lo extra vamos a mover

            // Si el tiempo que queda es menor a la mitad del DeltaTime, movemos todo de golpe
            if (passedTime <= Time.deltaTime /2f)
            {
                step += passedTime; //Sumamos el tiempo que falta-
            }
        }
        
        elrigido.MovePosition((Time.deltaTime + step) * velocidad * direction + elrigido.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (InstanceFinder.IsClientStarted)
        {
            // Dar feedback como particuals, sonidos
        }

        if (InstanceFinder.IsServerStarted)
        {
            //Aplicar daño
        }
        
        Destroy(gameObject);
    }
}
