using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerCoinHolder : NetworkBehaviour
{
    [SerializeField] private TextMeshPro coinHeldText;

    private NetworkVariable<int> carriedCoins = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Owner,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        carriedCoins.OnValueChanged += OnCoinsChanged;
        UpdateCoinText();

        if (IsOwner) return;
        coinHeldText.gameObject.SetActive(false); //Hides display from other players
    }

    public override void OnNetworkDespawn()
    {
        carriedCoins.OnValueChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int oldValue, int newValue)
    {
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinHeldText == null)
            return;

        int carryLimit = GameManager.Instance.GetCarryLimit(OwnerClientId);

        coinHeldText.text =
            $"Coins Held: {carriedCoins.Value}/{carryLimit}";
    }

    public void AddCollectedCoin()
    {
        if (!IsServer)
            return;

        carriedCoins.Value++;
    }

    public void CashInCoins()
    {
        if (!IsServer)
            return;

        GameManager.Instance.AddCoins(
            OwnerClientId,
            carriedCoins.Value
        );

        carriedCoins.Value = 0;
    }

    public void LoseCoins()
    {
        if (!IsServer)
            return;

        carriedCoins.Value = 0;
    }

    public int GetCarriedCoins()
    {
        return carriedCoins.Value;
    }

    public bool TryCollectCoin()
    {
        if (!IsServer)
            return false;

        int carryLimit = GameManager.Instance.GetCarryLimit(OwnerClientId);
        carriedCoins.Value++;

        if (carriedCoins.Value > carryLimit)
        {
            LoseCoins();

            Respawner player = GetComponent<Respawner>();
            player.FailedRespawn();

            return false;
        }

        return true;
    }
}