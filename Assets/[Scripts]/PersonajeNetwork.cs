using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PersonajeNetwork : NetworkBehaviour
{
    [FormerlySerializedAs("_moveInput")] [SerializeField] Vector3 moveInput = default;
    [SerializeField]
    float _moveSpeed = 3f;
    readonly SyncVar<Color> _myColor = new SyncVar<Color>();
    
    // SyncVar<Dato/Valor>
    // C# 2 tipoa de variables de dato/valor y de referencia
    // Dato/valor: int, float, bool, string, char, STRUCT
    // referencia: Son las que hacen referencia a las clases. 
    
    private void Awake()
    {
        // La funcion OnChange requiere (T valorAnterior, T valorNuevo, bool asServer)
        _myColor.OnChange += myColorChange;
    }
    
    [ServerRpc] // Una función que se ejecuta en el servidor
    void ChangeColorServerRPC()
    {
        _myColor.Value = Random.ColorHSV();
        // print("Se cambio a color: " + _myColor.Value);
    }
    

    private void Start()
    {
        //Aquí no hay garantia de que las variables de networking esten sincronizadas
    }

    // Start pero las variables de networking ya estan sincronizadas
    public override void OnStartNetwork()
    {
        if (Owner.IsLocalClient)
        {
            name += "(Local)";
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    void myColorChange(Color before, Color next, bool asServer) // next == myColor.Value
    {
        GetComponent<MeshRenderer>().material.color = next;
    }
    
    void Update()
    {
        if (IsOwner == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // print("Se presiono la tecla C - color actual " + _myColor.Value);
            ChangeColorServerRPC(); // Mensaje de RED // 5 ms // se conoce como ping, y decimos que tenemos lag
            /*//  print("Color despues de mandar mensaje al servidor " + _myColor.Value);  0.01ms
            // No tiene sentido tratar de imprimir el valor de la variable de arriba porque todavia no se recibe
            //  Debido al tiempo que tarda el paquete en enviarse y recibirse, la ejecucion del codigo es mas rapida por 
            //  lo que el valor de la variable no se ha actualizado. y no se va imprmir el valor apropiadamente*/
        }
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(moveInput * (Time.deltaTime * _moveSpeed));
    }
}
