using System;
using System.Collections.Generic;

public static class PathSolver
{
    // Generic Dijkstra with a pluggable edge-cost function.
    public static List<NetworkNode> FindPath(NetworkGraph graph, NetworkNode start, NetworkNode end,
        Func<NetworkNode, NetworkNode, float, float> edgeCost)
    {
        var dist = new Dictionary<NetworkNode, float>();
        var prev = new Dictionary<NetworkNode, NetworkNode>();
        var unvisited = new HashSet<NetworkNode>();

        foreach (var node in graph.Nodes)
        {
            dist[node] = float.MaxValue;
            prev[node] = null;
            unvisited.Add(node);
        }
        dist[start] = 0f;

        while (unvisited.Count > 0)
        {
            NetworkNode current = null;
            float minDist = float.MaxValue;
            foreach (var n in unvisited)
                if (dist[n] < minDist) { minDist = dist[n]; current = n; }

            if (current == null || current == end) break;
            unvisited.Remove(current);

            foreach (var (neighbor, baseCost) in graph.GetNeighbors(current))
            {
                if (!unvisited.Contains(neighbor)) continue;
                if (!neighbor.IsPoweredOn) continue; // Skip powered-off nodes
                float alt = dist[current] + edgeCost(current, neighbor, baseCost);
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = current;
                }
            }
        }

        var path = new List<NetworkNode>();
        var step = end;
        while (step != null)
        {
            path.Insert(0, step);
            step = prev.ContainsKey(step) ? prev[step] : null;
        }

        return path.Count > 0 && path[0] == start ? path : new List<NetworkNode>();
    }

    // Selects the edge-cost function for the active routing mode.
    public static List<NetworkNode> FindPath(NetworkGraph graph, NetworkNode start, NetworkNode end)
    {
        Func<NetworkNode, NetworkNode, float, float> cost = GameModes.Routing switch
        {
            RoutingMode.ShortestHops    => (a, b, c) => 1f,                          // every edge = 1 hop
            RoutingMode.CongestionAware => (a, b, c) => c + b.CongestionRatio * 10f, // penalize busy nodes
            _                           => (a, b, c) => c,                           // Dijkstra: raw distance
        };
        return FindPath(graph, start, end, cost);
    }
}
