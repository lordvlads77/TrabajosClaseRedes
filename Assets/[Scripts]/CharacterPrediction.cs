using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Prediction;
using GameKit.Dependencies.Utilities;
using UnityEngine;

public class CharacterPrediction : NetworkBehaviour
{
    public float saltoFuerza = 7f;
    public float velocidad = 11f;
    PredictionRigidbody _predictionRigidbody;
    bool _isSaltando;
    private void Awake()
    {
        _predictionRigidbody = ObjectCaches<PredictionRigidbody>.Retrieve();
        _predictionRigidbody.Initialize(GetComponent<Rigidbody>());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
