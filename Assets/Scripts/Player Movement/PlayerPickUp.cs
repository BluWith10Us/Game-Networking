using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float baseRange = 1f;
    [SerializeField] private LayerMask pickupLayer;

    private float currentRange;

    private void Start()
    {
        currentRange = baseRange;
    }

    void Update()
    {
        // Automatically check for nearby objects every frame
        TryPickUp();
    }

    private void TryPickUp()
    {
        // Check for all colliders within the radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentRange, pickupLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<IPickUpable>(out IPickUpable pickup))
            {
                pickup.PickUp(gameObject);
            }
        }
    }

    // Visualize the detection area in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Draw the radius around the player
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }

    public void ModifyRange(float modifier)
    {
        currentRange += modifier;
    }
}