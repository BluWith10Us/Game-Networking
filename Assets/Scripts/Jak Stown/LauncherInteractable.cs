using Unity.Netcode;
using UnityEngine;

public class LauncherInteractable : NetworkBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float maxStrength = 10f;

    public NetworkVariable<bool> isBusy = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public void LaunchBall(ulong playerId, float strength)
    {
        if (!IsServer) return;
        if (isBusy.Value) return;

        isBusy.Value = true;

        float clampedStrength = Mathf.Clamp(strength, 0f, maxStrength);

        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);

        var netObj = ball.GetComponent<NetworkObject>();
        netObj.Spawn();

        var notifier = ball.GetComponent<BallLandingNotifier>();
        notifier.Init(this, clampedStrength, playerId);
    }

    public void OnBallResolved()
    {
        if (!IsServer) return;
        isBusy.Value = false;
    }
}