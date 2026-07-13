using Unity.Netcode;
using UnityEngine;

public class CoinPickup : NetworkBehaviour, IPickUpable
{
    private bool collected = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CoinManager.Instance.Register(this);
        }
    }

    public void PickUp(GameObject target)
    {
        if (!IsServer || collected) return;

        //Grabs components from player
        NetworkPlayerController player = target.GetComponent<NetworkPlayerController>();
        if (player == null) return;

        BallLauncher playerLauncher = player.GetComponent<BallLauncher>();

        if (playerLauncher != null && !playerLauncher.isBusy.Value)
        {
            return; //does not pick up if the player is throwing the ball into the air
        }

        PlayerCoinHolder holder = player.GetComponent<PlayerCoinHolder>();
        if (holder != null && holder.TryCollectCoin()) //Collects coin if conditions are met
        {
            collected = true;
            SetCoinStateClientRpc(false); //turns off coin object for later respawn
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