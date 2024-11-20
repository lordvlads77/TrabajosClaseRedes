using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Demo.AdditiveScenes;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class PropertiesUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_Dropdown colorUser;

    private void Start()
    {
        //OnClientConnectionState Solo se necesita si estamos en la misma escena que el NetworkManager
        InstanceFinder.ClientManager.OnClientConnectionState += ClientManagerOnClientConnectionState;
        inputUsername.text = PlayerPrefs.GetString("username");
        colorUser.value = PlayerPrefs.GetInt("userColor_r");
    }

    private void OnDestroy()
    {
        if (InstanceFinder.ClientManager)
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= ClientManagerOnClientConnectionState;
        }
    }

    public void Guardar()
    {
        PlayerPrefs.SetString("username", inputUsername.text);
        PlayerPrefs.SetInt("userColor_r", colorUser.value);
    }
    
    void ClientManagerOnClientConnectionState(ClientConnectionStateArgs arg)
    {
        gameObject.SetActive(arg.ConnectionState == LocalConnectionState.Stopped);// Enviamos del cliente al servidor la informacion
    } 

    // Nota Si hacemos cambios en eldropdown hay que hacer cambios 
    public static readonly Color[] colores = {Color.red, Color.green, Color.blue, Color.yellow, Color.magenta};
}
