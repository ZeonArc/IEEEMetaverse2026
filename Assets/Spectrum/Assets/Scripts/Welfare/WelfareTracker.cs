using System.Linq;
using UnityEngine;

public class WelfareTracker : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private AuctionManager auctionManager;

    public float SocialWelfare { get; private set; }
    public float AverageLatency { get; private set; }
    public int TotalAuctions { get; private set; }

    void OnEnable() => auctionManager.OnAuctionResolved += OnAuction;
    void OnDisable() => auctionManager.OnAuctionResolved -= OnAuction;

    private void OnAuction(NetworkNode node, AuctionResult result) => TotalAuctions++;

    void Update()
    {
        var agents = FindObjectsByType<RoutingAgent>(FindObjectsSortMode.None);
        if (agents.Length == 0) return;

        float totalWelfare = 0f;
        float totalLatency = 0f;
        int activeCount = 0;

        foreach (var agent in agents)
        {
            if (!agent.HasRoute) continue;
            float pathCost = Vector3.Distance(agent.transform.position, agent.Destination.transform.position);
            totalWelfare += agent.Valuation - pathCost;
            totalLatency += pathCost;
            activeCount++;
        }

        SocialWelfare = totalWelfare;
        AverageLatency = activeCount > 0 ? totalLatency / activeCount : 0f;
    }
}
