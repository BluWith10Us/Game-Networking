using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    [Header("Lobby Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private int minimumPlayersToStart = 2;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;

    private readonly Dictionary<ulong, bool> readyStates = new();
    private readonly Dictionary<ulong, int> playerSpawnIndices = new();

    private bool gameStarted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

            foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                if (!readyStates.ContainsKey(clientId))
                {
                    readyStates[clientId] = false;
                }
            }
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
        readyStates.Remove(clientId);
        playerSpawnIndices.Remove(clientId);
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
        if (gameStarted) return;
        if (readyStates.Count < minimumPlayersToStart) return;

        foreach (var isReady in readyStates.Values)
        {
            if (!isReady) return;
        }

        gameStarted = true;
        SpawnPlayers();
        gameTimer.StartTimer();
    }

    private void SpawnPlayers()
    {
        if (!IsServer) return;

        HideReadyButtonRpc();
        int index = 0;

        foreach (var clientId in readyStates.Keys)
        {
            int spawnIndex = index % spawnPoints.Length;
            Transform spawn = spawnPoints[spawnIndex];

            playerSpawnIndices[clientId] = spawnIndex;

            GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);
            NetworkObject netObj = player.GetComponent<NetworkObject>();

            netObj.SpawnAsPlayerObject(clientId, destroyWithScene: true);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };

            ForcePlayerPositionClientRpc(spawn.position, spawn.rotation, clientRpcParams);

            index++;
        }
    }

    public void RespawnPlayer(ulong clientId)
    {
        if (!IsServer) return;

        if (!playerSpawnIndices.TryGetValue(clientId, out int spawnIndex)) return;

        Transform spawn = spawnPoints[spawnIndex];

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
        };

        ForcePlayerPositionClientRpc(spawn.position, spawn.rotation, clientRpcParams);
    }

    [ClientRpc]
    private void ForcePlayerPositionClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        NetworkObject localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (localPlayer != null)
        {
            CharacterController controller = localPlayer.GetComponent<CharacterController>();

            if (controller != null) controller.enabled = false;

            localPlayer.transform.SetPositionAndRotation(position, rotation);

            if (controller != null) controller.enabled = true;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideReadyButtonRpc()
    {
        if (readyButton != null)
            readyButton.SetActive(false);
    }
}