using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touched: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player died");

            Respawner respawner = other.GetComponent<Respawner>();

            if (respawner != null)
                respawner.FailedRespawn();
        }
    }
}
