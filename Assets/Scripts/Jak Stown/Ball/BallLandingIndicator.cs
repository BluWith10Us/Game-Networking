using UnityEngine;

public class BallLandingIndicator : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorPrefab; // Display for ground
    [SerializeField] private LayerMask groundLayer;    

    private GameObject currentIndicator;

    void Start()
    {
        if (indicatorPrefab != null)
        {
            currentIndicator = Instantiate(indicatorPrefab);
        }
    }

    void Update()
    {
        if (currentIndicator == null) return;

        // Raycast downwards from the ball to find the ground
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, groundLayer))
        {
            currentIndicator.SetActive(true);

            // Position the indicator at the hit point, slightly offset to prevent Z-fighting
            currentIndicator.transform.position = hit.point + (Vector3.up * 0.05f);

            // Keep the indicator flat on the ground
            currentIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            currentIndicator.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Cleanup the indicator if the ball is destroyed
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }
    }
}