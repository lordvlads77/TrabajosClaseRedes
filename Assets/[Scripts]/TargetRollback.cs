using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class TargetRollback : MonoBehaviour
{
    public float velocidad = 5f;
    Rigidbody _rigidbody;
    private Vector3 dir;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        dir = Vector3.right;
    }

    [Server]
    void Update()
    {
        _rigidbody.MovePosition(Time.deltaTime * velocidad * dir +_rigidbody.position);

        if (dir.x > 0) //Vamos a la derecha
        {
            if (_rigidbody.position.x > 6f)
            {
                dir.x = -1f;
            }
            else //Vamos a la izquierda
            {
                if (_rigidbody.position.x <= -6f)
                {
                    dir.x = 1f;
                }
            }
        }
    }
}
