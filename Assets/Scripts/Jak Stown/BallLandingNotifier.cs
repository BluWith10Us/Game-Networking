using Unity.Netcode;
using UnityEngine;

public class BallLandingNotifier : NetworkBehaviour
{
    private LauncherInteractable launcher;
    private float strength;
    private ulong ownerClientId;
    private bool resolved;

    public void Init(LauncherInteractable l, float s, ulong ownerId)
    {
        launcher = l;
        strength = s;
        ownerClientId = ownerId;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * strength * 3f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || resolved) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var respawner = collision.gameObject.GetComponent<Respawner>();
            respawner?.SuccessRespawn();

            launcher?.OnBallResolved();
            Resolve();
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            launcher?.OnBallResolved();

            GameObject playerObj = null;

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(ownerClientId, out var client))
            {
                playerObj = client.PlayerObject != null ? client.PlayerObject.gameObject : null;
            }

            if (playerObj != null)
            {
                var respawner = playerObj.GetComponent<Respawner>();
                respawner?.FailedRespawn();
            }

            Resolve();
        }
    }

    void Resolve()
    {
        resolved = true;
        GetComponent<NetworkObject>().Despawn(true);
    }
}