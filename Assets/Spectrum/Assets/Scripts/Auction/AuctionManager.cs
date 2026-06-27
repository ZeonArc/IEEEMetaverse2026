using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AuctionManager : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private float congestionThreshold = 0.8f;
    [SerializeField] private float auctionCooldown = 3f;

    public event System.Action<NetworkNode, AuctionResult> OnAuctionResolved;

    private Dictionary<NetworkNode, float> _cooldowns = new();

    void Update()
    {
        foreach (var node in graph.Nodes)
        {
            if (_cooldowns.ContainsKey(node))
            {
                _cooldowns[node] -= Time.deltaTime;
                if (_cooldowns[node] > 0) continue;
                _cooldowns.Remove(node);
            }

            if (node.CongestionRatio < congestionThreshold) continue;

            var agents = FindAgentsUsingNode(node);
            if (agents.Count < 2) continue;

            var result = AuctionResolver.Resolve(agents);

            foreach (var loser in result.Losers)
                loser.RecalculatePath();

            node.CurrentLoad = Mathf.Max(0, node.CurrentLoad - result.Losers.Count);
            _cooldowns[node] = auctionCooldown;
            OnAuctionResolved?.Invoke(node, result);
        }
    }

    private List<RoutingAgent> FindAgentsUsingNode(NetworkNode node)
    {
        return FindObjectsByType<RoutingAgent>(FindObjectsSortMode.None)
            .Where(a => a.HasRoute && Vector3.Distance(a.transform.position, node.transform.position) < 0.3f)
            .ToList();
    }
}
