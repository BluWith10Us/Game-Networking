using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MagnetMask : WearableMask
{
    [SerializeField] private float grabIncrease = 4f;
    [SerializeField] private float magnetDuration = 7f;
    [SerializeField] private GameObject magnetField; 

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

            ApplyMagnetClientRpc(netObj.NetworkObjectId, grabIncrease, rpcParams);
            StartCoroutine(RespawnRoutine());
        }

        AttachToHead(target);
        WearRoutine(magnetDuration);
    }

    [ClientRpc]
    private void ApplyMagnetClientRpc(
    ulong playerId,
    float increase,
    ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
            playerId,
            out NetworkObject playerObj))
        {
            if (playerObj.TryGetComponent<PlayerPickUp>(out var pickup))
            {
                StartCoroutine(BoostRoutine(pickup, increase));

                GameObject field = Instantiate(magnetField, playerObj.transform);
                field.transform.localPosition = Vector3.zero;

                // Scale to match the pickup radius
                float diameter = pickup.CurrentRange * 2f;
                field.transform.localScale = Vector3.one * diameter;

                Destroy(field, magnetDuration);
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
