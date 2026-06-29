using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTPacket : MonoBehaviour
{
    public string SourceIP;
    public string DestinationIP;
    
    public string Payload = "Ping Request";
    public float Speed = 2f;
    public bool IsMalicious = false;
    
    private bool _isInitialized = false;

    public enum Protocol { ICMP, HTTP }
    public Protocol PacketProtocol = Protocol.ICMP;

    public void Init(string sourceIp, string destIp, bool isMalicious = false, Protocol protocol = Protocol.ICMP)
    {
        SourceIP = sourceIp;
        DestinationIP = destIp;
        IsMalicious = isMalicious;
        PacketProtocol = protocol;
        
        if (_isInitialized) return;
        _isInitialized = true;
        
        Color packetColor = Color.green; // Normal ICMP
        
        if (isMalicious) 
            packetColor = Color.red; 
        else if (protocol == Protocol.HTTP) 
            packetColor = Color.blue; // HTTP is Blue
        else if (Payload == "Ping Reply") 
            packetColor = Color.cyan; // Keep reply cyan for visibility

        // Visual setup (a small glowing sphere)
        var mesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mesh.transform.SetParent(transform);
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localScale = Vector3.one * 0.08f;
        
        var renderer = mesh.GetComponent<Renderer>();
        renderer.material.color = packetColor;
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", packetColor * 2f);
        
        Destroy(mesh.GetComponent<Collider>());
        
        // Add a glowing trail
        var trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.04f;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.material.color = packetColor;
        
        // Add floating text label to the packet
        var textGo = new GameObject("Dest_Label");
        textGo.transform.SetParent(transform);
        textGo.transform.localPosition = new Vector3(0, 0.1f, 0);
        
        textGo.AddComponent<PTBillboard>();
        
        var text = textGo.AddComponent<TMPro.TextMeshPro>();
        text.fontSize = 1.5f;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.color = Color.white;
        text.text = $"To: {destIp}";
    }

    public void TraverseEdge(PTEdge edge, PTNode fromNode, PTNode toNode)
    {
        StartCoroutine(TravelRoutine(edge, fromNode, toNode));
    }

    private IEnumerator TravelRoutine(PTEdge edge, PTNode fromNode, PTNode toNode)
    {
        float activeSpeed = Speed * edge.SpeedMultiplier;
        bool hasPathPoints = edge.PathPoints != null && edge.PathPoints.Count >= 2;
        
        // Physics Simulation: Packet Loss on long Cat5 cables
        bool dropsMidway = false;
        if (edge.Type == PTEdge.CableType.Cat5)
        {
            float dist = Vector3.Distance(fromNode.transform.position, toNode.transform.position);
            if (dist > 2.0f)
            {
                float dropChance = (dist - 2.0f) * 0.2f; // 20% per meter over 2m
                if (Random.value < dropChance)
                {
                    dropsMidway = true;
                }
            }
        }
        
        float totalDist = Vector3.Distance(fromNode.transform.position, toNode.transform.position);
        float distTraveled = 0f;

        if (!hasPathPoints)
        {
            // Move directly from node to node, dynamically updating to their live positions
            transform.position = fromNode.transform.position;
            while (Vector3.Distance(transform.position, toNode.transform.position) > 0.02f)
            {
                float step = activeSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, toNode.transform.position, step);
                distTraveled += step;
                
                if (dropsMidway && distTraveled > totalDist * 0.5f)
                {
                    Debug.Log($"[Packet Loss] Packet dropped due to Cat5 attenuation!");
                    Destroy(gameObject);
                    yield break;
                }
                
                yield return null;
            }
        }
        else
        {
            // Pre-calculated curved paths
            List<Vector3> points = edge.PathPoints;
            bool reverse = Vector3.Distance(points[0], fromNode.transform.position) > 
                           Vector3.Distance(points[points.Count - 1], fromNode.transform.position);

            int startIdx = reverse ? points.Count - 1 : 0;
            int endIdx = reverse ? -1 : points.Count;
            int stepDir = reverse ? -1 : 1;

            transform.position = points[startIdx];

            for (int i = startIdx; i != endIdx; i += stepDir)
            {
                Vector3 targetPoint = points[i];
                while (Vector3.Distance(transform.position, targetPoint) > 0.01f)
                {
                    float step = activeSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);
                    distTraveled += step;
                    
                    if (dropsMidway && distTraveled > totalDist * 0.5f)
                    {
                        Debug.Log($"[Packet Loss] Packet dropped due to Cat5 attenuation!");
                        Destroy(gameObject);
                        yield break;
                    }
                    
                    yield return null;
                }
            }
        }

        // We arrived!
        toNode.ReceivePacket(this, edge);
    }
}
