using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

public class PlayerProperties : NetworkBehaviour
{
    public static PlayerProperties Instance { get; private set; }
    public readonly SyncDictionary<NetworkConnection, PlayerProperty> Properties = new();
    void Start()
    {
        Instance = this;
        InstanceFinder.ClientManager.OnClientConnectionState += ClientManagerOnClientConnectionState;
        InstanceFinder.ServerManager.OnRemoteConnectionState += ServerManagerOnRemoteConnectionState;
        InstanceFinder.ServerManager.RegisterBroadcast<PlayerProperty>(OnServerPlayerProperty);
    }

    void ServerManagerOnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs arg)
    {
        if (arg.ConnectionState != RemoteConnectionState.Stopped)
        {
            return;
        }
        
        // Si se desconecto un jugador, lo removemos del diccionario
        if (Properties.ContainsKey(conn))
        {
            Properties.Remove(conn); // Removemos del diccionario
        }
    }

    private void OnDestroy()
    {
        if (InstanceFinder.ClientManager)
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= ClientManagerOnClientConnectionState;
            InstanceFinder.ServerManager.UnregisterBroadcast<PlayerProperty>(OnServerPlayerProperty);
        }
    }

    void OnServerPlayerProperty(NetworkConnection conn, PlayerProperty property, Channel channel = Channel.Reliable)
    {
        print($"Se acaba de conectar el jugador {conn.ClientId}, su username:{property.username}, su Color = {property.color}" );
        Properties.Add(conn, property); // Guardamos en diccionario
    }

    void ClientManagerOnClientConnectionState(ClientConnectionStateArgs arg)
    {
        if (arg.ConnectionState != LocalConnectionState.Started)
        {
            return;
        }
        
        string username = PlayerPrefs.GetString("username");
        Color color = PropertiesUI.colores[PlayerPrefs.GetInt("userColor_r")];

        PlayerProperty properties = new PlayerProperty()
        {
            username = username,
            color = color,
            isAlive = true
        };
        InstanceFinder.ClientManager.Broadcast(properties);// Enviamos del cliente al servidor la informacion
    }

    public struct PlayerProperty : IBroadcast
    {
        public string username;
        public Color color;
        public bool  isAlive;
    }
}
