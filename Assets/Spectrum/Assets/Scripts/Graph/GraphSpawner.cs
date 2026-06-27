using UnityEngine;

public class GraphSpawner : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;

    // Awake runs before any Start(), so nodes exist before AgentSpawner reads them.
    void Awake() => Build(6);

    // Procedural ring topology with cross-links to opposite nodes.
    public void Build(int nodeCount)
    {
        float radius = 1.5f;
        float height = 1.2f;
        var nodes = new NetworkNode[nodeCount];

        for (int i = 0; i < nodeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / nodeCount;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
            nodes[i] = graph.AddNode(pos);
        }

        for (int i = 0; i < nodeCount; i++)
            graph.AddEdge(nodes[i], nodes[(i + 1) % nodeCount]); // ring

        for (int i = 0; i < nodeCount / 2; i++)
            graph.AddEdge(nodes[i], nodes[i + nodeCount / 2]);   // cross-links
    }
}
