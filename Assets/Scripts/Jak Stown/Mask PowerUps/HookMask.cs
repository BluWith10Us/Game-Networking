using Unity.Netcode;
using UnityEngine;

public class HookMask : WearableMask
{
    [SerializeField] private float hookDuration = 7f;

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

            ApplyHookClientRpc(netObj.NetworkObjectId, hookDuration, rpcParams);

            StartCoroutine(RespawnRoutine());
        }

        AttachToHead(target);
        WearRoutine(hookDuration);
    }

    [ClientRpc]
    private void ApplyHookClientRpc(
    ulong playerId,
    float duration,
    ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
            playerId,
            out NetworkObject playerObj))
        {
            if (playerObj.TryGetComponent<PlayerHook>(out var hook))
            {
                Debug.Log("Hook Found");
                hook.EnableHook(duration);
            }
        }

        Debug.Log("Hook Client called");
    }
}