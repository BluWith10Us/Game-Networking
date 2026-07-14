using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpeedMask : WearableMask
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float boostDuration = 10f;

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

            ApplyBoostClientRpc(netObj.NetworkObjectId, speedMultiplier, rpcParams);
            StartCoroutine(RespawnRoutine());
        }

        AttachToHead(target);
        StartCoroutine(WearRoutine(boostDuration));
    }

    [ClientRpc]
    private void ApplyBoostClientRpc(
    ulong playerId,
    float multiplier,
    ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
          playerId,
          out NetworkObject playerObj))
        {
            if (playerObj.TryGetComponent<NetworkPlayerController>(out var movement))
            {
                StartCoroutine(BoostRoutine(movement, multiplier));
            }
        }
    }

    private IEnumerator BoostRoutine(NetworkPlayerController movement, float multiplier)
    {
        movement.ModifySpeed(multiplier);
        yield return new WaitForSeconds(boostDuration);
        movement.ModifySpeed(1f / multiplier);
    }
}