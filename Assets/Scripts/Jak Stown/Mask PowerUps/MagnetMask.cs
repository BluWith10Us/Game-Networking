using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MagnetMask : WearableMask
{
    [SerializeField] private float grabMultiplier = 4f;
    [SerializeField] private float magnetDuration = 7f;

    public override void PickUp(GameObject target)
    {
        if (!IsServer) return;

        if (target.TryGetComponent<NetworkObject>(out var netObj))
        {
            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { netObj.OwnerClientId }
                }
            };

            ApplyMagnetClientRpc(netObj.NetworkObjectId, grabMultiplier, rpcParams);
            StartCoroutine(RespawnRoutine());
        }

        AttachToHead(target);
    }

    [ClientRpc]
    private void ApplyMagnetClientRpc(
    ulong playerId,
    float multiplier,
    ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
          playerId,
          out NetworkObject playerObj))
        {
            if (playerObj.TryGetComponent<PlayerPickUp>(out var pickup))
            {
                StartCoroutine(BoostRoutine(pickup, multiplier));
            }
        }
    }

    private IEnumerator BoostRoutine(PlayerPickUp pickup, float multiplier)
    {
        pickup.ModifyRange(multiplier);
        yield return new WaitForSeconds(magnetDuration);
        pickup.ModifyRange(-multiplier);
    }
}
