using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HookableBall : NetworkBehaviour
{
    [SerializeField] private float hookSpeed = 10f;
    [SerializeField] private float hookTime = 1.5f;

    private ulong ownerClientId;

    public ulong OwnerClientId => ownerClientId;

    public void SetOwner(ulong owner)
    {
        ownerClientId = owner;
    }

    public void StartHook()
    {
        if (!IsServer) return;

        StopAllCoroutines();
        StartCoroutine(HookRoutine());
    }

    IEnumerator HookRoutine()
    {
        float timer = hookTime;

        while (timer > 0f)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(ownerClientId, out var client))
            {
                Transform player = client.PlayerObject.transform;

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    player.position,
                    hookSpeed * Time.deltaTime);
            }

            timer -= Time.deltaTime;
            yield return null;
        }
    }
}