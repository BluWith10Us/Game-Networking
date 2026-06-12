using TMPro;
using UnityEngine;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    [Tooltip("Game Time in Seconds")]
    [SerializeField] float gameDuration;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject winScreen;
    public NetworkVariable<bool> IsPaused = new NetworkVariable<bool>(false,
       NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float activeTimer;
    bool isGameStart = false;

    public override void OnNetworkSpawn()
    {
        // Listen for changes to the pause state
        IsPaused.OnValueChanged += OnPauseStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        IsPaused.OnValueChanged -= OnPauseStateChanged;
    }

    public void StartTimer()
    {
        if (!IsServer)
            return;

        activeTimer = gameDuration;
        isGameStart = true;
    }

    public void TogglePauseServer()
    {
        if (!IsServer) return;
        IsPaused.Value = !IsPaused.Value;
    }

    private void Update()
    {
        if (!IsServer || !isGameStart || IsPaused.Value)
            return;

        activeTimer -= Time.deltaTime;
        UpdateTimerUIClientRpc(activeTimer);

        if (activeTimer <= 0)
        {
            Debug.Log("Game End");
            isGameStart = false;
            IsPaused.Value = true;
            activeTimer = 0;
            ShowWinScreenRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateTimerUIClientRpc(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnPauseStateChanged(bool previousValue, bool newValue)
    {
        NetworkPlayerController[] players = FindObjectsByType<NetworkPlayerController>(
            FindObjectsSortMode.None
        );

        foreach (NetworkPlayerController player in players)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = !newValue;
            }
        }

        if (newValue)
        {
            Debug.Log("Game Paused");
        }
        else
        {
            Debug.Log("Game Resumed");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowWinScreenRpc()
    {
        Debug.Log("Win Screen Shown");
        if (winScreen != null)
            winScreen.SetActive(true);
    }
}
