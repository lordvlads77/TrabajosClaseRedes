using System;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using UnityEngine;

public class CharacterPrediction : NetworkBehaviour
{
    public struct InputData : IReplicateData
    {
        public Vector2 axis; //Input horizontal y vertical
        public bool jump; //Input de salto
        uint tick;

        public InputData(Vector2 axis, bool jump) : this()
        {
            this.axis = axis;
            this.jump = jump;
        }
        public uint GetTick() => tick;

        public void SetTick(uint value)
        {
            tick = value;
        }

        public void Dispose() { } //Fishnet se encarga de esta funcion.
    }
    
    
    
    // Esta estructura almacenará la información que el servidor enviara a los clientes
    public struct ReconcileData : IReconcileData
    {
        // public Vector3 position;
        public PredictionRigidbody predictionRigidbody;
        uint tick;

        public ReconcileData(PredictionRigidbody pr) : this()
        {
            this.predictionRigidbody = pr;
        }
        public uint GetTick() => tick;

        public void SetTick(uint value)
        {
            tick = value;
        }

        public void Dispose() { } //Fishnet se encarga de esta funcion.
    }
    
    public float saltoFuerza = 7f;
    public float velocidad = 11f;
    public PredictionRigidbody _predictionRigidbody;
    bool _jump;
        
    private void Awake()
    {
        _predictionRigidbody = ObjectCaches<PredictionRigidbody>.Retrieve();
        _predictionRigidbody.Initialize(GetComponent<Rigidbody>());
        
        TimeManager.OnTick += OnTick;
        TimeManager.OnPostTick += PostTick;
    }

    private void OnDestroy()
    {
        ObjectCaches<PredictionRigidbody>.StoreAndDefault(ref _predictionRigidbody);
        if (!TimeManager)
        {
            TimeManager.OnTick -= OnTick;
            TimeManager.OnPostTick -= PostTick;
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {           
            _jump = true; // Guardamos que el usuario intento saltar.
        }
    }

    void OnTick() //Se va a volver nuestro fixedUpdate pero se sincroniza con fishnet
    {
        InputData input = CreateInputData();
        RunInputs(input);
    }

    InputData CreateInputData()
    {
        if (!IsOwner)
        {
            return default; // es un tipo null
        }
        
        Vector2 axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        InputData input = new InputData(axis, _jump);
        _jump = false; // Reseteamos porque ya se utilizo.
        return input;
    }

    void RunInputs(InputData input, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable )
    {
        
    }

    void PostTick() // Un buen lugar para que el servidor envie resultados de usar los inputs.
    {
        
    }
    
    // FixedUpdate es el Update de las físicas.
}
