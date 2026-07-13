using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class SpeedMask : NetworkBehaviour, IPickUpable
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float boostDuration = 10f;
    [SerializeField] private float respawnTime = 5f;

    public void PickUp(GameObject target)
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

    private IEnumerator RespawnRoutine()
    {
        SetStateClientRpc(false);
        yield return new WaitForSeconds(respawnTime);
        SetStateClientRpc(true);
    }

    [ClientRpc]
    private void SetStateClientRpc(bool active)
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = active;
        foreach (Transform child in transform) child.gameObject.SetActive(active);
    }
}