using UnityEngine;

public class NetworkBuilderUI : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private Transform spawnPoint; // Where new nodes appear
    [SerializeField] private EdgeDrawer edgeDrawer; // Reference to auto-register new nodes
    [SerializeField] private PacketInjector packetInjector; // Reference to auto-register new nodes
    [SerializeField] private NodeConfigUI nodeConfigUI; // Reference to auto-register new nodes

    [SerializeField] private GameObject[] nodePrefabs;

    public void SpawnNode()
    {
        SpawnSpecificNode(-1);
    }

    public void SpawnSpecificNode(int prefabIndex)
    {
        if (graph == null) return;
        
        Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
        Vector3 pos = spawnPoint != null ? spawnPoint.position + randomOffset : transform.position + randomOffset + Vector3.forward * 1f;

        GameObject prefabToSpawn = null;
        if (prefabIndex >= 0 && nodePrefabs != null && prefabIndex < nodePrefabs.Length)
        {
            prefabToSpawn = nodePrefabs[prefabIndex];
        }

        NetworkNode newNode;
        if (prefabToSpawn != null)
        {
            newNode = graph.AddNodePrefab(prefabToSpawn, pos);
        }
        else
        {
            newNode = graph.AddNode(pos); // Fallback to graph's default prefab
        }

        if (edgeDrawer != null) edgeDrawer.RegisterNode(newNode);
        if (packetInjector != null) packetInjector.RegisterNode(newNode);
        if (nodeConfigUI != null) nodeConfigUI.RegisterNode(newNode);
    }

    public void ClearGraph()
    {
        if (graph != null)
        {
            graph.Clear();
            if (edgeDrawer != null) edgeDrawer.ResetSelection();
            if (packetInjector != null) packetInjector.ResetSelection();
        }
    }
}
