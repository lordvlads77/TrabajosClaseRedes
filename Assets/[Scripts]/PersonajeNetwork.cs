using System;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
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
    private readonly SyncVar<GameObject> target = new SyncVar<GameObject>();
    
    // Se puede sincronizar GameObject solo SI tiene el NetworkObject
    
    private void Awake()
    {
        // La funcion OnChange requiere (T valorAnterior, T valorNuevo, bool asServer)
        _myColor.OnChange += myColorChange;
    }

    void EnviarTeletransporte()
    {
        //Gameobject go;
        //NetworkObject networkObject = go.GetComponent<NetworkObject>();
        //Teletransportar(networkObject.ObjectId);
    }

    [ObserversRpc]
    void Teletransportar(GameObject target)
    {
        transform.position = target.transform.position;
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

    private void OnTriggerEnter(Collider other)
    {
        if (IsServerStarted == false) // No soy el servidor, me salgo y no hago nada.
        {
            //Reproducir Sonido
            //Dar feedback al usuario directamente
            return;
        }
        /*if(IsClientStarted) // Hay que tener cuidado con este si estamos usando un HOST.
        Servidor y cliente al mismo tiempo.*/
        if (other.CompareTag("Cube"))
        {
            Despawn(other.gameObject); // Destroy (aunque seria mas bien un SetActive = false) porque lo esconde,
            // pero se sincroniza en red.
            //Owner // <--- owner es un NetworkConnection
            EquiparArmaRPC(connection:Owner); // Le mando mensaje solo al jugador que controla este gameObject
            // Si Owner es == null, // Lo controla el servidor
        }
    }
    
    [TargetRpc]
    void EquiparArmaRPC(NetworkConnection connection)
    {
        print("Si tome el arma");
    }

    // Puedo llamar esta funcion en gameObjects que no me pertenecen
    [ServerRpc (RequireOwnership = false)]
    void CambiarColorServerRPC2()
    {
        Color nuevoColor = Random.ColorHSV();
        CambiarColorRPC2(nuevoColor);
    }
    
    //[TargetRpc] Que permite mandar mensajes "paquetes" hacia un solo usuario/jugador en especifico.
    // RunLocally => Permite que el servidor ejecute el mismo codigo que el cliente SIN ser cliente
    [ObserversRpc (RunLocally = true)] // Observer es un cliente, solo se ejecuta en clientes
    void CambiarColorRPC2(Color nuevoColor)
    {
        GetComponent<MeshRenderer>().material.color = nuevoColor;
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

        if (Input.GetKeyDown(KeyCode.V))
        {
            CambiarColorServerRPC2();
        }
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(moveInput * (Time.deltaTime * _moveSpeed));
    }
    
    // SyncVar<Dato/Valor>
    // C# 2 tipoa de variables de dato/valor y de referencia
    // Dato/valor: int, float, bool, string, char, STRUCT, Vector3, Vector2, Quaterniones
    // referencia: class, son las que hacen referencia a las clases.

    struct MyStruct
    {
        public float a; // Si se sincroniza
        public Vector3 pos; // Si se sincroniza
        public Transform t; // No se sincroniza
    }
    
    
}
