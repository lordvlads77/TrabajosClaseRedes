using System;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PersonajeNetwork : NetworkBehaviour
{
    public static PersonajeNetwork LocalPersonaje { get; private set; }
    [FormerlySerializedAs("_moveInput")] [SerializeField] Vector3 moveInput = default;
    [SerializeField]
    float _moveSpeed = 3f;
    public readonly SyncVar<Color> _myColor = new SyncVar<Color>();
    private readonly SyncVar<GameObject> target = new SyncVar<GameObject>();
    
    // List, Hashet, Dictionary
    readonly SyncList<int> milista = new SyncList<int>(); // List<int>
    
    // Start pero las variables de networking ya estan sincronizadas
    public override void OnStartNetwork()
    {
        if (Owner.IsLocalClient)
        {
            name += "(Local)";
            LocalPersonaje = this;
            //GameEvents.instance.OnLocalPlayerSpawn.Invoke();// Llamar despues de asignar el singleton
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    public override void OnStopNetwork()
    {
        if (GameEvents.instance)
        {
            GameEvents.instance.OnLocalPlayerDespawn.Invoke();
        }
    }
    
    private void Awake()
    {
        
        // La funcion OnChange requiere (T valorAnterior, T valorNuevo, bool asServer)
        _myColor.OnChange += myColorChange;
        milista.OnChange += MiListaChange;
    }

    // 1, 10, 5, 3, 5, 8, 9, 2, 4, 6 // [2] = 13
    void MiListaChange(SyncListOperation operation, int index, int OldValue, int NewValue, bool asServer)
    {
        print($"{operation} - index: {index} - OldValue: {OldValue} - NewValue: {NewValue}");
        switch (operation)
        {
            case SyncListOperation.Add: // miLista.Add(20);
                // index, en que posicion se inserto, newValue que valor se agrego.
                break;
            case SyncListOperation.Insert: // miLista.Insert(20, 5); // Se agrego el numer0 20 en la posicion 5
                break;
            case SyncListOperation.Set: // Set seria cuando ponemos un valor en especifico // miLista[2] = 13;
                // Se puede utilizar index, oldValue, newValue
                break;
            case SyncListOperation.RemoveAt: // Es por si quiero eliminar un elemento // miLista.RemoveAt(1); // Elimina el 10
                // Se puede utilizar index, oldValue // oldValue tendria el valor de la variable que fue removida.
                // en este caso seria 10
                break;
            case SyncListOperation.Clear: //miLista.Clear(); // Esto limpia everything, no hay ni indice, ni newValue, ni OldValue
                break;
            case SyncListOperation.Complete: // Cuando se llamaron todas las operaciones de este fishnet frame
                break;
        }
    }

    [Server (Logging = LoggingType.Off)] // Solo ejecuta el codigo si es servidor nos ahorra el 'if (IsServerStarted) return'
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            milista.Add(Random.Range(1, 100));
        };
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            milista.RemoveAt(Random.Range(0, milista.Count));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            milista.Clear();
        }
    }

    // [SYNC] miLista.Add(10); miLista.Add(15); miLista.Remove(0); [SYNC] [SYNC] miLista.Clear(); miLista.Add(3) [SYNC] 
    
    //C#
    // int[] array
    //List<int> // areglo dinamico
    // Lista // Contra: Si necesito verificar que sea unico, o si existe un elemento, es muy lento porque tiene que
    // recorrer toda la lista de elemento en elemento para poder decirte despues.
    // Hashet<int> // Lista pero con hash
    //Dictionary<string, int> // Es como una agenda, tenemos una key vinculando a un valor.
    //1,10, 5, 3, 7, 8, 9, 2, 4, 6
    
    //miLista.Contains(20);
    
    // Se puede sincronizar GameObject solo SI tiene el NetworkObject
    
    public override void OnStartServer() { } // Start del lado del Server, podemos modificar syncvars para que al
                                             // enviarse al jugador llegue con esos datos
    public override void OnStartClient() { } // Start con syncvars sincronizados pero solo se llama del lado del cliente
    //public override void OnStartNetwork() { } // se llama en server y en el cliente
    
    //Se llaman cuando un GameObject cambia de dueño.
    public override void OnOwnershipServer(NetworkConnection preOwner) { }
    public override void OnOwnershipClient(NetworkConnection preOwner) { }
    
    // Se llama cuando un objeto es destruido en red, y aun no se envia el mensaje
    public override void OnDespawnServer(NetworkConnection connection) { }
    
    // El 'Destroy' de multiplayer, aqui ya fue mandado que se destruyo el objeto.
    public override void OnStopServer() { }
    public override void OnStopClient() { }
    
    
    //Con Virtual la funcion puede cambiar que hace. 
    

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
