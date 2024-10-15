using FishNet.Object;
using UnityEngine;

public class Arma : NetworkBehaviour
{
    public GameObject prefabBala;
    public float _velocidadBala;
    
    void Update()
    {
        if (!IsOwner) // Solo puede disparar el dueño de este personaje
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DispararServerRPC();
        }
    }

    [ServerRpc]
    void DispararServerRPC()
    {
        GameObject nuevabala = Instantiate(prefabBala,transform.position + transform.forward, Quaternion.identity);
        // Modificar todas las variables que sean SYNCVAR para que cuando se instancie le mande toda la info rmación
        nuevabala.GetComponent<Bala>()._duenioId.Value = OwnerId;
        //nuevaBala.GetComponent<Bala>().duenioId.Value = ownerId;
        // Modificar las SYNCVARS antes de llamar al spawn en caso de requerir alguna modificacion
        nuevabala.GetComponent<Rigidbody>().velocity = Vector3.forward * _velocidadBala; 
        
        ServerManager.Spawn(nuevabala); // AL FINAL Le aviso a todos los clientes de la nueva bala
    }
}
