using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHook : NetworkBehaviour
{
    private bool canHook;

    public void EnableHook(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(HookRoutine(duration));
    }

    IEnumerator HookRoutine(float duration)
    {
        canHook = true;

        yield return new WaitForSeconds(duration);

        canHook = false;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!canHook) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseHookServerRpc();
        }
    }

    [ServerRpc]
    private void UseHookServerRpc()
    {
        HookableBall[] balls = FindObjectsByType<HookableBall>(FindObjectsSortMode.None);

        foreach (var ball in balls)
        {
            if (ball.OwnerClientId == OwnerClientId)
            {
                ball.StartHook();
                break;
            }
        }
    }
}