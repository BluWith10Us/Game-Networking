using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerShooter : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab; //instantiated bullet object when shooting
    [SerializeField] private Transform bulletSpawnPoint; //point of fire where the bullet spawns from
    [SerializeField] private float fireCooldown = 0.5f; //time in seconds between shots, para hindi everytime nagprepress tayo may lumalabas
    [SerializeField] KeyCode fireKey = KeyCode.F; //Ginawa kong Mouse1 kasi Mouse0 is melee spam thingy

    private float lastFireTime;

    private void Update()
    {
        if(!IsOwner) return;
        if(Input.GetKeyDown(fireKey) && Time.time >= lastFireTime + fireCooldown)
        {
            lastFireTime = Time.time + fireCooldown;
            RequestShootServerRpc(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        }
    }
    
    [ServerRpc]
    private void RequestShootServerRpc(Vector3 spawnPosition, Vector3 spawnDirection)
    {
        //Creates the object in the server
        GameObject projectileInstance = Instantiate(
            bulletPrefab, 
            spawnPosition, 
            Quaternion.LookRotation(spawnDirection));

        //Tells Unity Netcode to show this object to all players
        NetworkObject networkObject = projectileInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }
}
