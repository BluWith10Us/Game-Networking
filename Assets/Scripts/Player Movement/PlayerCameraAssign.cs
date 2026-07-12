using UnityEngine;
using Unity.Netcode;

public class PlayerCameraAssign : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        CameraPlayerFollow cameraFollow = Camera.main.GetComponent<CameraPlayerFollow>();
        cameraFollow.SetTarget(transform);
    }
}
   
