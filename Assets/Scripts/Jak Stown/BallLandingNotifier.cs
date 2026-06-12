using Unity.Netcode;
using UnityEngine;

public class BallLandingNotifier : NetworkBehaviour
{
    private LauncherInteractable launcher;
    private float strength;

    private ulong ownerClientId;
    private bool hasResolved = false;

    public void Init(LauncherInteractable l, float s, ulong ownerId)
    {
        launcher = l;
        strength = s;
        ownerClientId = ownerId;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * strength * 3, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || hasResolved) return;

        // SUCCESS: player catches ball
        if (collision.gameObject.CompareTag("Player"))
        {
            Respawner respawner = collision.gameObject.GetComponent<Respawner>();

            if (respawner != null)
            {
                respawner.SuccessRespawn();
            }

            if (launcher != null)
            {
                launcher.OnBallLanded();
            }

            Resolve();
            return;
        }

        // FAIL: hit ground failed
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (launcher != null)
            {
                launcher.OnBallLanded();
            }

            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ownerClientId, out var netObj))
            {
                var respawner = netObj.GetComponent<Respawner>();
                respawner?.FailedRespawn();
            }

            Resolve();
        }
    }

    private void Resolve()
    {
        hasResolved = true;

        NetworkObject netObj = GetComponent<NetworkObject>();
        netObj.Despawn(true);
    }
}