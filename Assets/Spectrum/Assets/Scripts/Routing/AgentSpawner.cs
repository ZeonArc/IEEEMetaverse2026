using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private GameObject packetPrefab;
    [SerializeField] private int agentCount = 8;

    void Start() => SpawnAgents(agentCount);

    public void SpawnAgents(int count)
    {
        if (graph.Nodes.Count < 2) return; // Guard: do-while below would loop forever with <2 nodes.

        for (int i = 0; i < count; i++)
        {
            int srcIdx = Random.Range(0, graph.Nodes.Count);
            int dstIdx;
            do { dstIdx = Random.Range(0, graph.Nodes.Count); } while (dstIdx == srcIdx);

            var go = Instantiate(packetPrefab, transform);
            var agent = go.GetComponent<RoutingAgent>();
            agent.Graph = graph;
            agent.Source = graph.Nodes[srcIdx];
            agent.Destination = graph.Nodes[dstIdx];
        }
    }

    public void SpawnSingleAgent(NetworkNode src, NetworkNode dst)
    {
        var go = Instantiate(packetPrefab, transform);
        var agent = go.GetComponent<RoutingAgent>();
        agent.Graph = graph;
        agent.Source = src;
        agent.Destination = dst;
    }
}
