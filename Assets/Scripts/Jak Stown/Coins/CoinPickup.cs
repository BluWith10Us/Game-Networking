using Unity.Netcode;
using UnityEngine;

public class CoinPickup : NetworkBehaviour
{
    private bool collected = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CoinManager.Instance.Register(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || collected) return;

        NetworkPlayerController player = other.GetComponent<NetworkPlayerController>();
        if (player == null) return;

        LauncherInteractable playerLauncher = player.GetComponent<LauncherInteractable>();

        if (playerLauncher != null && !playerLauncher.isBusy.Value)
        {
            return;
        }

        PlayerCoinHolder holder = player.GetComponent<PlayerCoinHolder>();
        if (holder != null && holder.TryCollectCoin())
        {
            collected = true;
            SetCoinStateClientRpc(false);
        }
    }

    public void ResetCoin()
    {
        collected = false;
        SetCoinStateClientRpc(true);
    }

    [ClientRpc]
    private void SetCoinStateClientRpc(bool state)
    {
        gameObject.SetActive(state);
    }
}