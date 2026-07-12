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
        if (!IsServer) return;

        ulong clientId = OwnerClientId;

        if (coins != null)
        {
            coins.CashInCoins();
        }

        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.RespawnPlayer(clientId);
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetCoins();
        }
    }

    public void FailedRespawn()
    {
        if (!IsServer) return;

        ulong clientId = OwnerClientId;

        if (coins != null)
        {
            coins.LoseCoins();
        }

        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.RespawnPlayer(clientId);
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetCoins();
        }
    }
}