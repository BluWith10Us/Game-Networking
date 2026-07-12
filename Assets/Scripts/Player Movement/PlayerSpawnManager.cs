using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnManager : NetworkBehaviour
{
    public static int nextSpawnIndex;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if(spawnPointObjects.Length == 0)
        {
            Debug.LogWarning("No Spawnpoints found");
            return;
        }

        Transform selectedSpawnPoint = spawnPointObjects[nextSpawnIndex].transform;
        CharacterController characterController = GetComponent<CharacterController>();
        Debug.Log(characterController);
        if(characterController != null )
        {
            characterController.enabled = false;
        }

        transform.position = selectedSpawnPoint.position;
        transform.rotation = selectedSpawnPoint.rotation;
        if(characterController != null )
        {
            characterController.enabled = true;
        }

        nextSpawnIndex++;
        if(nextSpawnIndex >= spawnPointObjects.Length)
        {
            nextSpawnIndex = 0;
        }
    }
}
