using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0, 10, -8);
    [SerializeField] float followSpeed;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredDest = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredDest, followSpeed * Time.deltaTime);
    }
}
