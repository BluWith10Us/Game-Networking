using Unity.Netcode;
using UnityEngine;

public abstract class WearableMask : NetworkBehaviour, IPickUpable
{
    [SerializeField] private Vector3 headOffset;
    [SerializeField] private GameObject maskVisualPrefab; // Prefab to instantiate on head

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
}