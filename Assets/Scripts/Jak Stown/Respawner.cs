using Unity.Netcode;
using UnityEngine;

public class Respawner : NetworkBehaviour
{
    private PlayerCoinHolder coins;

    private void Awake()
    {
        coins = GetComponent<PlayerCoinHolder>();
    }

    public void SuccessRespawn()
    {
        if (!IsOwner)
            return;

        SuccessRespawnRpc();
    }

    [Rpc(SendTo.Server)]
    private void SuccessRespawnRpc()
    {
        coins.CashInCoins();
        LobbyManager.Instance.RespawnPlayer(OwnerClientId);
        CoinManager.Instance.ResetCoins();
    }

    public void FailedRespawn()
    {
        if (!IsOwner)
            return;
        Debug.Log("hello I am respawning");
        FailedRespawnRpc();
    }

    [Rpc(SendTo.Server)]
    private void FailedRespawnRpc()
    {
        Debug.Log("hello I am losing coins");
        coins.LoseCoins();
        LobbyManager.Instance.RespawnPlayer(OwnerClientId);
        CoinManager.Instance.ResetCoins();
    }
}