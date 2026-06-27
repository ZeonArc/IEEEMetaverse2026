using System.Collections.Generic;
using UnityEngine;

public class NetworkGraph : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;

    public List<NetworkNode> Nodes { get; private set; } = new();
    public List<NetworkEdge> Edges { get; private set; } = new();

    public NetworkNode AddNode(Vector3 position)
    {
        return AddNodePrefab(nodePrefab, position);
    }

    public NetworkNode AddNodePrefab(GameObject prefab, Vector3 position)
    {
        if (prefab == null) prefab = nodePrefab;
        var go = Instantiate(prefab, position, Quaternion.identity, transform);
        var node = go.GetComponent<NetworkNode>();
        node.Id = Nodes.Count;
        Nodes.Add(node);
        return node;
    }

    public NetworkEdge AddEdge(NetworkNode a, NetworkNode b, List<Vector3> points = null)
    {
        var go = Instantiate(edgePrefab, transform);
        var edge = go.GetComponent<NetworkEdge>();
        edge.Init(a, b, points);
        Edges.Add(edge);
        return edge;
    }

    public List<(NetworkNode neighbor, float cost)> GetNeighbors(NetworkNode node)
    {
        var result = new List<(NetworkNode, float)>();
        foreach (var edge in Edges)
        {
            if (edge.NodeA == node) result.Add((edge.NodeB, edge.Cost));
            else if (edge.NodeB == node) result.Add((edge.NodeA, edge.Cost));
        }
        return result;
    }

    public void Clear()
    {
        foreach (var node in Nodes) if (node) Destroy(node.gameObject);
        foreach (var edge in Edges) if (edge) Destroy(edge.gameObject);
        Nodes.Clear();
        Edges.Clear();
    }
}
