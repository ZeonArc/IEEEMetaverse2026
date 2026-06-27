using System.IO;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphSaveData
{
    public List<Vector3> NodePositions = new();
    public List<int[]> Edges = new();
}

public class GraphSaveLoad : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;

    private string SavePath => Path.Combine(Application.persistentDataPath, "spectrum_save.json");

    public void Save()
    {
        var data = new GraphSaveData();
        foreach (var node in graph.Nodes)
            data.NodePositions.Add(node.transform.position);
        foreach (var edge in graph.Edges)
            data.Edges.Add(new[] { edge.NodeA.Id, edge.NodeB.Id });

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        Debug.Log($"Saved to {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath)) { Debug.LogWarning("No save file found."); return; }

        var data = JsonUtility.FromJson<GraphSaveData>(File.ReadAllText(SavePath));

        foreach (var node in graph.Nodes) Destroy(node.gameObject);
        foreach (var edge in graph.Edges) Destroy(edge.gameObject);
        graph.Nodes.Clear();
        graph.Edges.Clear();

        foreach (var pos in data.NodePositions)
            graph.AddNode(pos);
        foreach (var pair in data.Edges)
            graph.AddEdge(graph.Nodes[pair[0]], graph.Nodes[pair[1]]);
    }
}
