using UnityEngine;
using Unity.Netcode;
using System.Xml.Schema;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            CurrentHealth.Value = maxHealth;
        }
        CurrentHealth.OnValueChanged += OnChangeHealth;
    }

    public override void OnNetworkDespawn()
    {
        CurrentHealth.OnValueChanged -= OnChangeHealth;
    }

    public void OnChangeHealth(int previousAmount, int newAmount)
    {
        Debug.Log($"{gameObject.name} change health: {previousAmount} -> {newAmount}");
    }

    public void TakeDamage(int damageTaken)
    {
        if (!IsServer) return;
        CurrentHealth.Value -= damageTaken;
        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value, 0, maxHealth);

        if (CurrentHealth.Value > 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        CurrentHealth.Value = maxHealth;
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int rand = Random.Range(0, spawnPointObjects.Length);
        Transform selectedSpawn = spawnPointObjects[rand].transform;

        CharacterController characterController = GetComponent<CharacterController>();

        Debug.Log(characterController);

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = selectedSpawn.position;
        transform.rotation = selectedSpawn.rotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}
