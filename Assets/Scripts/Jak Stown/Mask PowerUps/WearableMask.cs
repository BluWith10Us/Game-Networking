using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class WearableMask : NetworkBehaviour, IPickUpable
{
    [SerializeField] private Vector3 headOffset;
    [SerializeField] private GameObject maskVisualPrefab; // Prefab to instantiate on head
    [SerializeField] private float respawnTime = 5f;

    public virtual void PickUp(GameObject target)
    {

        AttachToHead(target);
    }

    protected void AttachToHead(GameObject player)
    {
        // Assuming your player has a child named "Head"
        Transform head = player.transform.Find("Head");
        if (head != null && maskVisualPrefab != null)
        {
            // Instantiate the prefab visual on the head
            GameObject visual = Instantiate(maskVisualPrefab, head);
            visual.transform.localPosition = headOffset;
            visual.transform.localRotation = Quaternion.identity;

            // Disable collider of the mask object itself so it doesn't interfere
            if (TryGetComponent<Collider>(out var col)) col.enabled = false;
        }
    }

    public IEnumerator RespawnRoutine()
    {
        SetStateClientRpc(false);
        yield return new WaitForSeconds(respawnTime);
        SetStateClientRpc(true);
    }

    [ClientRpc]
    private void SetStateClientRpc(bool active)
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = active;
        foreach (Transform child in transform) child.gameObject.SetActive(active);
    }
}