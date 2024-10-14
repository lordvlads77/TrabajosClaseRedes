using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using UnityEngine;

public class OnlineRigidbody : NetworkBehaviour
{
    public struct ReplicateData : IReplicateData
    {
        private uint tick;
        
        public ReplicateData(int _) : this() {}
        public uint GetTick()
        {
            return tick;
        }

        public void SetTick(uint value)
        {
            tick = value;
        }

        public void Dispose()
        {
        }
    }
    
    public struct ReconcileData : IReconcileData
    {
        public PredictionRigidbody predictionRigidbody;
        private uint tick;

        public ReconcileData(PredictionRigidbody pr) : this()
        {
            predictionRigidbody = pr;
        }
        public uint GetTick()
        {
            return tick;
        }

        public void SetTick(uint value)
        {
            tick = value;
        }

        public void Dispose()
        {
        }
    }
    
    PredictionRigidbody predictionRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        predictionRigidbody = ObjectCaches<PredictionRigidbody>.Retrieve();
        predictionRigidbody.Initialize(GetComponent<Rigidbody>());
        
        TimeManager.OnPostTick += PostTick;
    }
    
    private void OnDestroy()
    {
        ObjectCaches<PredictionRigidbody>.StoreAndDefault( ref predictionRigidbody);
        if (TimeManager)
        {
            TimeManager.OnPostTick -= PostTick;
        }
    }

    public override void OnStartNetwork()
    {
        TimeManager.OnPostTick += PostTick;
    }
    
    public override void OnStopNetwork()
    {
        TimeManager.OnPostTick -= PostTick;
    } 

    
    
    void PostTick()
    {
        RunInputs(default);
        CreateReconcile();
    }

    [Replicate]
    void RunInputs(ReplicateData md, ReplicateState state = ReplicateState.Invalid,
        Channel channel = Channel.Unreliable)
    {
    }

    public override void CreateReconcile()
    {
        if (TimeManager.Tick % 3 != 0)
        {
            return;
        }
        ReconcileData rd = new ReconcileData(predictionRigidbody);
        ReconcileState(rd);
    }

    [Reconcile]
    void ReconcileState(ReconcileData data, Channel channel = Channel.Unreliable)
    {
        predictionRigidbody.Reconcile(data.predictionRigidbody);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
