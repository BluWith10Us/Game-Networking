using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : NetworkBehaviour
{
    [SerializeField] GameObject startScreen;

    [Rpc(SendTo.Server)]
    public void ResetNetworkSceneRpc()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        NetworkManager.Singleton.SceneManager.LoadScene(
            currentSceneName,
            LoadSceneMode.Single
        );
        startScreen.SetActive(false);
    }
}