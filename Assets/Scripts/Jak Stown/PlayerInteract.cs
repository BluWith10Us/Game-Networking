using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractable();

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void DetectInteractable()
    {
        currentInteractable = null;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            interactLayer
        );

        float closestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentInteractable = interactable;
                }
            }
        }
    }

    void TryInteract()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}