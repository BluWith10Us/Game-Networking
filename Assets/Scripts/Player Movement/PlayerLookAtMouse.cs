using UnityEngine;
using Unity.Netcode;

public class PlayerLookAtMouse : NetworkBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Only the local owner should control their own character's rotation
        if (!IsOwner || mainCamera == null) return;

        LookAtMouse();
    }

    void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Define a plane at the player's Y level to find the hit point
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 worldPoint = ray.GetPoint(rayDistance);
            Vector3 targetPosition = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);
            transform.LookAt(targetPosition);
        }
    }
}