using System.Collections;
using System.Collections.Generic;
using FishNet.Broadcast;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            
        }
    }
    
    public struct MensajeBroadcast : IBroadcast
    {
        public Color unColor;
        public int unNumero;
    }
}
