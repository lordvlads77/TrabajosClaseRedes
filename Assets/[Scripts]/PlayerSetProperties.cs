using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSetProperties : NetworkBehaviour
{
    [FormerlySerializedAs("_meshRenderer")] [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] private TextMeshPro usernameText;
    public override void OnStartNetwork()
    {
        StartCoroutine(SetPlayerData());
    }

    IEnumerator SetPlayerData()
    {
        while (PlayerProperties.Instance.Properties.ContainsKey(Owner) == false)
        {
            yield return null;
        }
        
        PlayerProperties.PlayerProperty property = PlayerProperties.Instance.Properties[Owner];
        
        usernameText.text = property.username;
        meshRenderer.material.color = property.color;
    }
}
