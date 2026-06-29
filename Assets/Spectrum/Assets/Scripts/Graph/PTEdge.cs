using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PTEdge : MonoBehaviour
{
    public PTNode NodeA;
    public PTNode NodeB;
    public List<Vector3> PathPoints = new List<Vector3>();

    public enum CableType { Cat5, Fiber }
    public CableType Type = CableType.Cat5;

    public float Bandwidth = 100f; // Mbps
    public float SpeedMultiplier = 1f;

    private LineRenderer _lr;
    private TextMeshPro _label;

    public void Init(PTNode a, PTNode b, List<Vector3> points = null, CableType cableType = CableType.Cat5)
    {
        NodeA = a;
        NodeB = b;
        Type = cableType;
        
        NodeA.Connections.Add(this);
        NodeB.Connections.Add(this);

        if (points != null && points.Count >= 2)
        {
            PathPoints = new List<Vector3>(points);
        }

        _lr = GetComponent<LineRenderer>();
        
        if (Type == CableType.Fiber)
        {
            _lr.startWidth = 0.008f;
            _lr.endWidth = 0.008f;
            Bandwidth = 10000f; // 10 Gbps
            SpeedMultiplier = 3f; // 3x visual speed
            
            _lr.material = new Material(Shader.Find("Sprites/Default"));
            _lr.material.color = new Color(1f, 0.5f, 0f); // Bright Orange
            _lr.material.EnableKeyword("_EMISSION");
            _lr.material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 2f);
        }
        else // Cat5
        {
            _lr.startWidth = 0.015f;
            _lr.endWidth = 0.015f;
            Bandwidth = 100f; // 100 Mbps
            SpeedMultiplier = 1f;
            
            _lr.material = new Material(Shader.Find("Standard"));
            _lr.material.color = new Color(0.3f, 0.3f, 0.3f); // Dark Gray
        }

        var labelGo = new GameObject("EdgeLabel");
        labelGo.transform.SetParent(transform);
        _label = labelGo.AddComponent<TextMeshPro>();
        _label.fontSize = 2f;
        _label.alignment = TextAlignmentOptions.Center;
    }

    void Update()
    {
        if (!NodeA || !NodeB) return;

        if (PathPoints == null || PathPoints.Count < 2)
        {
            _lr.positionCount = 2;
            _lr.SetPosition(0, NodeA.transform.position);
            _lr.SetPosition(1, NodeB.transform.position);
            Vector3 mid = (NodeA.transform.position + NodeB.transform.position) / 2f;
            _label.transform.position = mid + Vector3.up * 0.05f;
        }
        else
        {
            PathPoints[0] = NodeA.transform.position;
            PathPoints[PathPoints.Count - 1] = NodeB.transform.position;

            _lr.positionCount = PathPoints.Count;
            _lr.SetPositions(PathPoints.ToArray());

            Vector3 mid = PathPoints[PathPoints.Count / 2];
            _label.transform.position = mid + Vector3.up * 0.05f;
        }

        // Optional: show link speed
        _label.text = $"{Bandwidth}Mbps";
    }

    // Transmit packet FROM sourceNode TO the other end
    public void Transmit(PTPacket packet, PTNode sourceNode)
    {
        PTNode targetNode = (sourceNode == NodeA) ? NodeB : NodeA;
        
        // Tell the packet to visually travel across this edge
        packet.TraverseEdge(this, sourceNode, targetNode);
    }
}
