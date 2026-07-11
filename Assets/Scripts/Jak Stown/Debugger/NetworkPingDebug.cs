using UnityEngine;
using Unity.Netcode;

public class NetworkPingDebug : NetworkBehaviour
{
    // We update the ping display only every second to avoid spamming calculations
    private float updateInterval = 1f;
    private float timer;
    private int currentPing = 0;
    private float pingStartTime;

    void Update()
    {
        // Only the local player who owns this object should calculate their ping
        if (!IsSpawned || !IsOwner) return;

        // If we are the Host (both Server and Client), latency to ourselves is 0
        if (IsServer && IsClient)
        {
            currentPing = 0;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0;
            pingStartTime = Time.realtimeSinceStartup;
            PingServerRpc(); // Send the ping to the server
        }
    }

    [Rpc(SendTo.Server)]
    private void PingServerRpc()
    {
        // The server receives this and immediately pings back the specific owner
        PongRpc();
    }

    [Rpc(SendTo.Owner)]
    private void PongRpc()
    {
        // The client receives the pong back, calculate the time difference
        float pingInSeconds = Time.realtimeSinceStartup - pingStartTime;
        currentPing = Mathf.RoundToInt(pingInSeconds * 1000f);
    }

    void OnGUI()
    {
        // Only draw the UI for our own player
        if (!IsSpawned || !IsOwner) return;

        // Simple UI overlay to see the ping in the top-left corner
        GUI.Label(new Rect(10, 10, 200, 20), $"Ping: {currentPing} ms");
    }
}