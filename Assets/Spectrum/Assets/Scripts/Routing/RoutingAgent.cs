using System.Collections.Generic;
using UnityEngine;

public class RoutingAgent : MonoBehaviour
{
    public NetworkGraph Graph;
    public NetworkNode Source;
    public NetworkNode Destination;
    public float Speed = 2f;
    public float Valuation;

    private List<NetworkNode> _path = new();
    private int _pathIndex;
    private float _t;

    public bool IsPaused = false;

    public bool HasRoute => _path.Count >= 2;

    void Start()
    {
        Valuation = Random.Range(1f, 10f);
        
        // Add TrailRenderer for visual polish
        var tr = gameObject.AddComponent<TrailRenderer>();
        tr.startWidth = 0.05f;
        tr.endWidth = 0.01f;
        tr.time = 2f;
        
        // Use a generic material if possible, or color it by Valuation
        tr.material = new Material(Shader.Find("Sprites/Default"));
        Color c = Color.Lerp(Color.red, Color.yellow, Valuation / 10f);
        tr.startColor = c;
        tr.endColor = new Color(c.r, c.g, c.b, 0f);

        RecalculatePath();
    }

    void OnEnable() => GameModes.OnRoutingChanged += RecalculatePath;
    void OnDisable() => GameModes.OnRoutingChanged -= RecalculatePath;

    void Update()
    {
        if (!HasRoute || IsPaused) return;

        var from = _path[_pathIndex];
        var to = _path[_pathIndex + 1];
        float dist = Vector3.Distance(from.transform.position, to.transform.position);

        _t += Time.deltaTime * Speed / Mathf.Max(dist, 0.01f);
        transform.position = Vector3.Lerp(from.transform.position, to.transform.position, _t);

        if (_t >= 1f)
        {
            _t = 0f;
            to.CurrentLoad += 1f;
            _pathIndex++;

            if (_pathIndex >= _path.Count - 1)
                RecalculatePath();
        }
    }

    public void RecalculatePath()
    {
        foreach (var node in _path)
            node.CurrentLoad = Mathf.Max(0, node.CurrentLoad - 0.5f);

        _path = PathSolver.FindPath(Graph, Source, Destination);
        _pathIndex = 0;
        _t = 0f;

        if (HasRoute)
            transform.position = Source.transform.position;
    }
}
