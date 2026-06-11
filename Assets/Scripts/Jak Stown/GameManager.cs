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
        player1Text.text = $"P1 Coins: {player1Coins.Value}";
        player2Text.text = $"P2 Coins: {player2Coins.Value}";
    }

    public void AddCoin(ulong clientId)
    {
        if (!IsServer) return;

        // Host is usually Client ID 0
        if (clientId == 0)
        {
            player1Coins.Value++;
        }
        else
        {
            player2Coins.Value++;
        }
    }
}