using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class WearableMask : NetworkBehaviour, IPickUpable
{
    [SerializeField] private Vector3 headOffset;
    [SerializeField] private GameObject maskVisualPrefab; // Prefab to instantiate on head
    [SerializeField] private float respawnTime = 5f;

    private GameObject equippedVisual;

    public virtual void PickUp(GameObject target)
    {

        AttachToHead(target);
    }

    protected void AttachToHead(GameObject player)
    {
        Transform head = player.transform.Find("Head");

        if (head != null && maskVisualPrefab != null)
        {
            equippedVisual = Instantiate(maskVisualPrefab, head);
            equippedVisual.transform.localPosition = headOffset;
            equippedVisual.transform.localRotation = Quaternion.identity;
        }

        if (TryGetComponent<Collider>(out var col))
            col.enabled = false;
    }

    protected void RemoveMask()
    {
        if (equippedVisual != null)
        {
            Destroy(equippedVisual);
            equippedVisual = null;
        }
    }

    protected IEnumerator WearRoutine(float duration)
    {
        Debug.Log($"wait routine called for: {duration}");
        yield return new WaitForSeconds(duration);

        RemoveMask();
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