using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;

public class Cronometro : NetworkBehaviour
{
    public TextMeshProUGUI tiempoTxt;
    
    // Option 1: SyncVar: Contras: Requiere siempre de banda ancha.
    // readonly SyncVar<float> tiempoRestante = new SyncVar<float>();
    
    // Option 2: RPC Contras: No considera el lag.
    
    // Option 3: RPC y enviamos tiempo: Funciona pero implica algo de calculos.
    
    readonly SyncTimer timepoRestante = new SyncTimer();

    private void Awake()
    {
        timepoRestante.OnChange += OnTiempoRestanteChange;
    }
    
    void OnTiempoRestanteChange(SyncTimerOperation operation, float prev, float next, bool asServer)
    {
        print($"Tiempo restante evento:{operation} - prev: {prev} - next: {next}");
        switch (operation)
        {
            case SyncTimerOperation.Start:
                tiempoTxt.color = Color.green;
                break;
            case SyncTimerOperation.Pause:
                tiempoTxt.color = Color.yellow; 
                break;
            case SyncTimerOperation.PauseUpdated:
                break;
            case SyncTimerOperation.Unpause:
                break;
            case SyncTimerOperation.Stop:
                tiempoTxt.color = Color.red;
                break;
            case SyncTimerOperation.StopUpdated:
                break;
            case SyncTimerOperation.Finished: // Llego a 0
                tiempoTxt.color = Color.gray; 
                break;
            case SyncTimerOperation.Complete: // Ya se terminaron de procesar todas las operaciones.
                break;
        }
    }
    private void Update()
    {
        timepoRestante.Update(Time.deltaTime); // Como cliente o server se necesita llamar
        tiempoTxt.text = $"{timepoRestante.Remaining:.00}"; // :.00 = Solo 2 decimales
        
        if (Input.GetKeyDown(KeyCode.T) && IsServerStarted)
        {
            timepoRestante.StartTimer(10f);
            /*timepoRestante.PauseTimer();
            timepoRestante.UnpauseTimer();
            timepoRestante.StopTimer();*/
        }
    }

    //[Server (Logging = LoggingType.Off)]
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && IsServerStarted)
        {
            uint tickSend = TimeManager.Tick;
            
            //Time.time // Tiempo desde que inicio el juego
            IniciarTiempoRPC(tickSend); // 100 segundos el segundo 100 del servidor cuando se inició el cronómetro.
        }
        
        if (tiempoAvanzando)
        {
            tiempoRestante -= Time.deltaTime;
            tiempoTxt.text = $"{tiempoRestante:.00}"; // es para que solo permita 2 decimales como maximo.
        }
    }

    [ObserversRpc (RunLocally = true)]
    void IniciarTiempoRPC(uint tickStarted)
    {
        tiempoAvanzando = true;
        float timePassed = (float)TimeManager.TimePassed(TimeManager.Tick, tickStarted); // 0.8 segs
        tiempoRestante = 10f - timePassed;
    }*/
}
