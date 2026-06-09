using UnityEngine;
using Unity.Netcode;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] float speed = 12.54897126834712348126f;
    [SerializeField] float lifetime = 20f;
    
    private float despawnTime;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            despawnTime = Time.time + lifetime;
        }
    }

    private void Update()
    {
        if(!IsServer) return;
        transform.position += 
            transform.forward * 
            speed * 
            Time.deltaTime;
        if(Time.time >= despawnTime)
        {
            NetworkObject.Despawn(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if(other.CompareTag("Player"))
        {
            NetworkObject.Despawn();
        }
    }
}
