using UnityEngine;

public class PTBuilderUI : MonoBehaviour
{
    [SerializeField] private PTGraph graph;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PTEdgeDrawer edgeDrawer;
    [SerializeField] private PTPingTool pingTool;
    
    // UI Panels for tools
    [SerializeField] private GameObject edgeDrawerModeIndicator; 
    [SerializeField] private GameObject packetInjectorModeIndicator;

    [SerializeField] private GameObject[] nodePrefabs;

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

        if (prefabToSpawn != null)
        {
            var newNode = graph.AddNodePrefab(prefabToSpawn, pos);
            if (edgeDrawer != null) edgeDrawer.RegisterNode(newNode);
            if (pingTool != null) pingTool.RegisterNode(newNode);
        }
    }

    public void ClearGraph()
    {
        if (graph != null)
        {
            graph.Clear();
        }
    }

    // Temporary logic for injecting test packets
    public void InjectTestPacket()
    {
        if (graph == null || graph.Nodes.Count < 2) return;

        // Find a PC and pick a random destination
        PTNode pc = graph.Nodes.Find(n => n.Type == PTNode.DeviceType.PC);
        if (pc != null)
        {
            // Just test sending to 8.8.8.8 (Google) to see if the router catches it, or another PC
            string targetIp = "192.168.1.11"; // Assuming a second PC spawned gets this IP
            graph.InjectPacket(pc, targetIp);
        }
    }
}
