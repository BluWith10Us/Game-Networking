using System.Collections.Generic;
using Unity.Netcode;

public class CoinManager : NetworkBehaviour
{
    public static CoinManager Instance;

    private readonly List<CoinPickup> coins = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(CoinPickup coin)
    {
        if (!coins.Contains(coin))
            coins.Add(coin);
    }

    public void ResetCoins()
    {
        if (!IsServer) return;

        foreach (var coin in coins)
        {
            if (coin != null)
                coin.ResetCoin();
        }
    }
}