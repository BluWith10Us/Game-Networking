using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private BallLauncher currentLauncher;

    private bool isCharging;
    private float charge;

    [SerializeField] private float maxChargeRate = 5f;
    [SerializeField] private float maxStrength = 10f;

    private void Awake()
    {
        currentLauncher = GetComponent<BallLauncher>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (currentLauncher != null)
            {
                isCharging = true;
                charge = 0f;
            }
        }

        if (isCharging && Input.GetKey(interactKey))
        {
            charge += Time.deltaTime * maxChargeRate;
            charge = Mathf.Clamp(charge, 0f, maxStrength);
        }

        if (isCharging && Input.GetKeyUp(interactKey))
        {
            isCharging = false;

            if (currentLauncher != null)
            {
                InteractServerRpc(currentLauncher.NetworkObjectId, charge);
            }
        }
    }

    [ServerRpc]
    void InteractServerRpc(ulong launcherId, float strength)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(launcherId, out var netObj))
            return;

        if (!netObj.TryGetComponent(out BallLauncher launcher))
            return;

        launcher.LaunchBall(OwnerClientId, strength);
    }
}