using Unity.Netcode;
using UnityEngine;

public class LauncherInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private KeyCode launchKey;
    [SerializeField] private float maxStrength = 10f;

    private float launchStrength;
    private bool isCharging;

    public NetworkVariable<bool> isBusy = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private CharacterController currentPlayer;

    private void Update()
    {
        if (!isCharging) return;

        if (Input.GetKey(launchKey))
        {
            launchStrength += Time.deltaTime * 5f;
            launchStrength = Mathf.Clamp(launchStrength, 0f, maxStrength);
            Debug.Log($"Launching with {launchStrength}");
        }

        if (Input.GetKeyUp(launchKey))
        {
            Debug.Log("Key Let go");
            LaunchServerRpc(launchStrength, NetworkManager.Singleton.LocalClientId);
            StopInteraction();
        }
    }

    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out NetworkObject netObj)) return;

        RequestInteractServerRpc(netObj.NetworkObjectId);
    }

    [ServerRpc]
    private void RequestInteractServerRpc(ulong interactorId)
    {
        if (isBusy.Value) return;

        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(interactorId, out var netObj))
            return;

        var interactor = netObj.gameObject;

        if (!interactor.TryGetComponent(out CharacterController controller))
            return;

        currentPlayer = controller;
        currentPlayer.enabled = false;

        isCharging = true;
        launchStrength = 0f;
    }

    [ServerRpc]
    private void LaunchServerRpc(float strength, ulong senderId)
    {
        if (isBusy.Value) return;

        isBusy.Value = true;

        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        ball.GetComponent<NetworkObject>().Spawn();

        var notifier = ball.GetComponent<BallLandingNotifier>();
        notifier.Init(this, strength, senderId);
    }

    public void OnBallLanded()
    {
        if (!IsServer) return;

        isBusy.Value = false;
    }

    private void StopInteraction()
    {
        if (currentPlayer != null)
        {
            currentPlayer.enabled = true;
        }

        isCharging = false;
    }
}