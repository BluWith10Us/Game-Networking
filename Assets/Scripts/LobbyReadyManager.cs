using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyReadyManager : NetworkBehaviour
{
    [SerializeField] TMP_Text playerListText;
    [SerializeField] TMP_Text readyStatusText;
    [SerializeField] GameObject startGameButton;

    private Dictionary<ulong, bool> playerReadyStates = new();
    //unassign long int, bool for ready or not.

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
            foreach( ulong clientID in NetworkManager.Singleton.ConnectedClientsIds )
            {
                playerReadyStates[clientID] = false;
            }        
            startGameButton.SetActive(IsServer);
        }
    }

    private void Singleton_OnClientDisconnectCallback(ulong obj)
    {
        throw new System.NotImplementedException();
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        throw new System.NotImplementedException();
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
