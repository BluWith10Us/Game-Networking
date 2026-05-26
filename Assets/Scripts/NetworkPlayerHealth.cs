using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject damageTextPrefab;

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

        ShowDamageClientRpc(damageTaken);

        if (CurrentHealth.Value <= 0)
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

    [ClientRpc]
    private void ShowDamageClientRpc(int damage)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 2f;

        GameObject damageObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

        PlayerDamagePopup damageText = damageObj.GetComponent<PlayerDamagePopup>();

        if (damageText != null)
        {
            damageText.SetDamage(damage);
        }
    }
}
