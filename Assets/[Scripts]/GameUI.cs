using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image miColorChange;
    PersonajeNetwork _personajeLocal;
    /*void Start()
    {
        GameEvents.instance.OnLocalPlayerSpawn.AddListener(OnLocalPlayerSpawned);
    }*/

    private void Awake() // o puede ser en el OnEnable
    {
        GameEvents.instance.OnLocalPlayerSpawn.AddListener(OnLocalPlayerSpawned);
        GameEvents.instance.OnLocalPlayerSpawn.AddListener(OnLocalPlayerDespawned);
        // gameObject .SetActive(false); // Si no tenemos network Object
    }

    private void OnDestroy()
    {
        if (GameEvents.instance)
        {
            GameEvents.instance.OnLocalPlayerSpawn.RemoveListener(OnLocalPlayerSpawned);
            GameEvents.instance.OnLocalPlayerSpawn.RemoveListener(OnLocalPlayerDespawned);
        }

        if (_personajeLocal)
        {
            _personajeLocal._myColor.OnChange -= OnColorChange;
        }
    }
    void OnLocalPlayerSpawned()
    {
        _personajeLocal = PersonajeNetwork.LocalPersonaje;
        _personajeLocal._myColor.OnChange += OnColorChange;
        OnColorChange(Color.white, _personajeLocal._myColor.Value, false); // Forzamos que actualize valores actuales
        // gameObject.SetActive(true); // Si no tenemos network Object
    }

    void OnLocalPlayerDespawned()
    {
        //gameObject.SetActive(false); // Si no tenemos network Object
    }
    void OnColorChange(Color before, Color next, bool asServer)
    {
        miColorChange.color = next;
    }
}
