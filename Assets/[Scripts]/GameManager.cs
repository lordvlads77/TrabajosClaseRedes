using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Cualquier mensaje que sea del tipo 'MensajeBroadcast' en SERVIDOR, procesalo en la función 'ServidorMensajeRecibido'
        InstanceFinder.ServerManager.RegisterBroadcast<MensajeBroadcast>(ServidorMensajeRecibido);
        // Si recibo un mensaje del tipo 'MensajeBroadcast' como cliente, procesalo en la función 'ClienteMensajeRecibido'
        InstanceFinder.ClientManager.RegisterBroadcast<MensajeBroadcast>(ClienteMensajeRecibido);
    }

    private void OnDestroy()
    {
        // Es buena practica desregistrar al destruir el gameObject
        if (InstanceFinder.ServerManager)
        {
            InstanceFinder.ServerManager.UnregisterBroadcast<MensajeBroadcast>(ServidorMensajeRecibido);
        }
        if (InstanceFinder.ClientManager)
        {
            InstanceFinder.ClientManager.UnregisterBroadcast<MensajeBroadcast>(ClienteMensajeRecibido);
        }
    }

    void ServidorMensajeRecibido(NetworkConnection connection, MensajeBroadcast mensaje, Channel channel)
    {
        print($"Este es el servidor y el mensaje de {connection.ClientId}: {mensaje.unColor} - {mensaje.unNumero}");
        mensaje.jugadorID = connection.ClientId;
        
        // Envio el mensaje a TODOS los jugadores.
        InstanceFinder.ServerManager.Broadcast(mensaje);
    }

    void ClienteMensajeRecibido(MensajeBroadcast mensaje, Channel channel)
    {
        print($"Este es el cliente y el mensaje de {mensaje.jugadorID} es: {mensaje.unColor} - {mensaje.unNumero}");
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Genero el mensaje que voy a enviar
            MensajeBroadcast mensaje = new MensajeBroadcast()
            {
                unColor = Random.ColorHSV(),
                unNumero = Random.Range(1, 100)
            };
            //Un cliente va a enviar un mensaje al servidor
            InstanceFinder.ClientManager.Broadcast(mensaje);
        }
    }
    
    public struct MensajeBroadcast : IBroadcast
    {
        public int jugadorID;
        public Color unColor;
        public int unNumero;
    }
}
