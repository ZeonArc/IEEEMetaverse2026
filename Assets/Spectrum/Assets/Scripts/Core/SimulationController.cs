using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private GraphSpawner graphSpawner;
    [SerializeField] private AgentSpawner agentSpawner;
    [SerializeField] private float fastForwardScale = 3f;

    private bool _paused;

    // Tear down agents + graph, then rebuild from scratch.
    public void ResetSimulation() => SetupFor(6, 8);

    // Rebuild the world for a specific topology size and agent count (used by scenarios).
    public void SetupFor(int nodeCount, int agentCount)
    {
        foreach (var agent in FindObjectsByType<RoutingAgent>(FindObjectsSortMode.None))
            Destroy(agent.gameObject);

        graph.Clear();
        graphSpawner.Build(nodeCount);
        agentSpawner.SpawnAgents(agentCount);
    }

    public void AddAgents(int count) => agentSpawner.SpawnAgents(count);

    public void TogglePause()
    {
        _paused = !_paused;
        Time.timeScale = _paused ? 0f : 1f;
    }

    public void ToggleFastForward()
    {
        if (_paused) return;
        Time.timeScale = Mathf.Approximately(Time.timeScale, 1f) ? fastForwardScale : 1f;
    }
}
