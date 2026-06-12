using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private LauncherInteractable currentLauncher;

    private bool isCharging;
    private float charge;

    [SerializeField] private float maxChargeRate = 5f;
    [SerializeField] private float maxStrength = 10f;

    private void Update()
    {
        if (!IsOwner) return;

        DetectInteractable();

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

    void DetectInteractable()
    {
        currentLauncher = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactLayer);

        float closestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            var launcher = hit.GetComponent<LauncherInteractable>();
            if (launcher == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                currentLauncher = launcher;
            }
        }
    }

    [ServerRpc]
    void InteractServerRpc(ulong launcherId, float strength)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(launcherId, out var netObj))
            return;

        if (!netObj.TryGetComponent(out LauncherInteractable launcher))
            return;

        launcher.LaunchBall(NetworkObjectId, strength);
    }
}