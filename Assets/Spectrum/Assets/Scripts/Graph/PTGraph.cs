using System.Collections.Generic;
using UnityEngine;

public class PTGraph : MonoBehaviour
{
    public List<PTNode> Nodes { get; private set; } = new List<PTNode>();
    public List<PTEdge> Edges { get; private set; } = new List<PTEdge>();

    public GameObject EdgePrefab;
    public GameObject PacketPrefab;

    private int _nextIpSuffix = 10;
    private int _nextSubnetSuffix = 1;

    public PTNode AddNodePrefab(GameObject prefab, Vector3 position)
    {
        var go = Instantiate(prefab, position, Quaternion.identity, transform);
        var node = go.GetComponent<PTNode>();
        
        // Auto-DHCP logic (Very basic for Sandbox)
        if (node.Type == PTNode.DeviceType.Router)
        {
            // Routers get multiple IPs in real life, but for now we give it a main one
            node.IPAddress = $"192.168.{_nextSubnetSuffix}.1";
            _nextSubnetSuffix++;
        }
        else if (node.Type == PTNode.DeviceType.PC || node.Type == PTNode.DeviceType.Server)
        {
            node.IPAddress = $"192.168.1.{_nextIpSuffix}";
            node.DefaultGateway = "192.168.1.1";
            _nextIpSuffix++;
        }
        
        node.UpdateLabel();
        
        Nodes.Add(node);
        return node;
    }

    public PTEdge AddEdge(PTNode a, PTNode b, List<Vector3> points = null)
    {
        var go = Instantiate(EdgePrefab, transform);
        var edge = go.GetComponent<PTEdge>();
        edge.Init(a, b, points);
        Edges.Add(edge);
        
        // Auto-configure routing table if we attach a PC to a Router
        AutoConfigureRouting(a, b, edge);
        
        // If one node is a monitor, attach its terminal to the other node
        if (a.Type == PTNode.DeviceType.Monitor) a.GetComponent<PTMonitor>()?.ConnectToNode(b);
        if (b.Type == PTNode.DeviceType.Monitor) b.GetComponent<PTMonitor>()?.ConnectToNode(a);
        
        return edge;
    }

    private void AutoConfigureRouting(PTNode a, PTNode b, PTEdge edge)
    {
        // If one is a Router and one is a Switch/PC, auto-add a route
        if (a.Type == PTNode.DeviceType.Router)
        {
            string subnet = PTNode.GetSubnet(b.IPAddress, b.SubnetMask);
            a.RoutingTable[subnet] = edge;
        }
        if (b.Type == PTNode.DeviceType.Router)
        {
            string subnet = PTNode.GetSubnet(a.IPAddress, a.SubnetMask);
            b.RoutingTable[subnet] = edge;
        }
    }

    public void InjectPacket(PTNode source, string destIP)
    {
        var go = Instantiate(PacketPrefab, source.transform.position, Quaternion.identity);
        var packet = go.GetComponent<PTPacket>();
        packet.Init(source.IPAddress, destIP);

        // Start transmission
        // In reality, it sends an ARP request, but we simulate it by pushing to its first connection
        if (source.Connections.Count > 0)
        {
            source.Connections[0].Transmit(packet, source);
        }
        else
        {
            Debug.LogWarning("Cannot inject packet: Source node has no connections.");
            Destroy(go);
        }
    }

    public void Clear()
    {
        foreach (var node in Nodes) if (node) Destroy(node.gameObject);
        foreach (var edge in Edges) if (edge) Destroy(edge.gameObject);
        Nodes.Clear();
        Edges.Clear();
    }
}
