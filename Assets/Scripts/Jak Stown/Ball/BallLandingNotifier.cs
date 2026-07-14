using Unity.Netcode;
using UnityEngine;

public class BallLandingNotifier : NetworkBehaviour
{
    private BallLauncher launcher;
    private float strength;
    private ulong ownerClientId;
    private bool resolved;

    public void Init(BallLauncher l, float s, ulong ownerId)
    {
        launcher = l;
        strength = s;
        ownerClientId = ownerId;

        GetComponent<HookableBall>()?.SetOwner(ownerId);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * strength * 3f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || resolved) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var respawner = collision.gameObject.GetComponent<Respawner>();
            var coinHolder = collision.gameObject.GetComponent<PlayerCoinHolder>();

            if (respawner != null && coinHolder != null)
            {
                int maxCoins = GameManager.Instance.GetCarryLimit(collision.gameObject.GetComponent<NetworkObject>().OwnerClientId);
                int currentCoins = coinHolder.GetCarriedCoins();

                if (currentCoins == maxCoins)
                {
                    respawner.SuccessRespawn();
                }
                else
                {
                    // Respawn them (failure) because they didn't have the right amount
                    respawner.FailedRespawn();
                }
            }

            launcher?.OnBallResolved();
            Resolve();
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            launcher?.OnBallResolved();

            // Player loses coins if the ball hits the ground
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(ownerClientId, out var client))
            {
                var playerObj = client.PlayerObject;
                if (playerObj != null)
                {
                    playerObj.GetComponent<Respawner>()?.FailedRespawn();
                }
            }
            Resolve();
        }
    }

    //For deathzone
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            launcher?.OnBallResolved();

            // Player loses coins if the ball hits the ground
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(ownerClientId, out var client))
            {
                var playerObj = client.PlayerObject;
                if (playerObj != null)
                {
                    playerObj.GetComponent<Respawner>()?.FailedRespawn();
                }
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