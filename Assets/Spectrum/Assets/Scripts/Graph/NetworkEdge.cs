using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class NetworkEdge : MonoBehaviour
{
    public NetworkNode NodeA;
    public NetworkNode NodeB;
    public List<Vector3> PathPoints = new List<Vector3>();

    public float Cost 
    {
        get 
        {
            if (PathPoints == null || PathPoints.Count < 2)
                return Vector3.Distance(NodeA.transform.position, NodeB.transform.position);
            
            float dist = 0f;
            for (int i = 0; i < PathPoints.Count - 1; i++)
            {
                dist += Vector3.Distance(PathPoints[i], PathPoints[i+1]);
            }
            return dist;
        }
    }

    private LineRenderer _lr;
    private TextMeshPro _label;

    public void Init(NetworkNode a, NetworkNode b, List<Vector3> points = null)
    {
        NodeA = a;
        NodeB = b;
        if (points != null && points.Count >= 2)
        {
            PathPoints = new List<Vector3>(points);
        }

        _lr = GetComponent<LineRenderer>();
        _lr.startWidth = 0.01f;
        _lr.endWidth = 0.01f;

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
            // Stretch the ends to match the nodes if they moved
            PathPoints[0] = NodeA.transform.position;
            PathPoints[PathPoints.Count - 1] = NodeB.transform.position;

            _lr.positionCount = PathPoints.Count;
            _lr.SetPositions(PathPoints.ToArray());

            Vector3 mid = PathPoints[PathPoints.Count / 2];
            _label.transform.position = mid + Vector3.up * 0.05f;
        }

        _label.text = Cost.ToString("F1");
    }
}
