using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private Transform[] spawnPoints;

    private readonly Dictionary<ulong, bool> readyStates = new();
    private bool gameStarted;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        readyStates[clientId] = false;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (readyStates.ContainsKey(clientId))
            readyStates.Remove(clientId);
    }

    [Rpc(SendTo.Server)]
    public void ReadyRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        readyStates[clientId] = true;

        CheckStartGame();
    }

    private void CheckStartGame()
    {
        Debug.Log("Ready Counts: " + readyStates.Count);
        if (gameStarted)
            return;

        if (readyStates.Count < 2)
            return;

        foreach (var kvp in readyStates)
        {
            if (!kvp.Value)
                return;
        }

        gameStarted = true;
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (!IsServer) return;
        HideReadyButtonRpc();
        int index = 0;

        foreach (var kvp in readyStates)
        {
            ulong clientId = kvp.Key;
            Transform spawn = spawnPoints[Mathf.Min(index, spawnPoints.Length - 1)];
            GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);
            NetworkObject netObj = player.GetComponent<NetworkObject>();
            netObj.SpawnAsPlayerObject(clientId, destroyWithScene: true);
            index++;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideReadyButtonRpc()
    {
        if (readyButton != null)
            readyButton.SetActive(false);
    }
}