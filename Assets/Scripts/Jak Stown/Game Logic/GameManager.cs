using Unity.Netcode;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text player1Text;
    [SerializeField] private TMP_Text player2Text;

    private NetworkVariable<int> player1Coins = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<int> player2Coins = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        player1Coins.OnValueChanged += OnPlayer1CoinsChanged;
        player2Coins.OnValueChanged += OnPlayer2CoinsChanged;

        UpdateUI();
    }

    public override void OnNetworkDespawn()
    {
        player1Coins.OnValueChanged -= OnPlayer1CoinsChanged;
        player2Coins.OnValueChanged -= OnPlayer2CoinsChanged;
    }

    private void OnPlayer1CoinsChanged(int oldValue, int newValue)
    {
        UpdateUI();
    }

    private void OnPlayer2CoinsChanged(int oldValue, int newValue)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        player1Text.text = $"P1 Score: {player1Coins.Value}";
        player2Text.text = $"P2 Score: {player2Coins.Value}";
    }

    public void AddCoins(ulong clientId)
    {
        if (!IsServer)
            return;

        if (clientId == 0)
        {
            player1Coins.Value += 1;
        }
        else
        {
            player2Coins.Value += 1;
        }
    }

    public string GetResult()
    {
        if (player1Coins.Value > player2Coins.Value)
        {
            return "Player 1 Wins!";
        }
        else if (player2Coins.Value > player1Coins.Value)
        {
            return "Player 2 Wins!";
        }
        else
        {
            return "It's a Draw!";
        }
    }

    //Gets players currently carried coins
    public int GetBankedCoins(ulong clientId)
    {
        if (clientId == 0)
            return player1Coins.Value;

        return player2Coins.Value;
    }

    public int GetCarryLimit(ulong clientId)
    {
        return GetBankedCoins(clientId) + 1;
    }
}