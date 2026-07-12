using Unity.Netcode;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    public void OnReadyPressed()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found.");
            return;
        }

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogWarning("Not connected to server.");
            return;
        }

        if (LobbyManager.Instance == null)
        {
            Debug.LogError("LobbyManager instance not found.");
            return;
        }

        LobbyManager.Instance.ReadyRpc();
    }
}