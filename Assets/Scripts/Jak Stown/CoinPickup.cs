using Unity.Netcode;
using UnityEngine;

public class CoinPickup : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NetworkPlayerController player = other.GetComponent<NetworkPlayerController>();

        if (player != null)
        {
            GameManager.Instance.AddCoin(player.OwnerClientId);

            if (IsServer)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
