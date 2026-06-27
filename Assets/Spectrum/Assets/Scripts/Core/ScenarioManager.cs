using System;
using UnityEngine;

[Serializable]
public class Scenario
{
    public string Name;
    [TextArea] public string Objective;
    public float TargetWelfare;
    public float TimeLimit;          // seconds; 0 = no limit
    public int NodeCount = 6;        // topology size for this level
    public int AgentCount = 8;       // traffic / difficulty
}

// Drives objective-based play: hit a target welfare, optionally before time runs out.
public class ScenarioManager : MonoBehaviour
{
    [SerializeField] private WelfareTracker welfare;
    [SerializeField] private SimulationController sim;
    [SerializeField] private Scenario[] scenarios;

    public event Action<Scenario> OnScenarioStarted;
    public event Action<bool> OnScenarioEnded; // true = won

    public Scenario Current { get; private set; }
    public int CurrentIndex { get; private set; }
    public float TimeLeft { get; private set; }
    public int Count => scenarios.Length;

    private bool _active;

    public void StartScenario(int index)
    {
        CurrentIndex = index;
        Current = scenarios[index];
        TimeLeft = Current.TimeLimit;
        sim.SetupFor(Current.NodeCount, Current.AgentCount); // build the level's world
        _active = true;
        OnScenarioStarted?.Invoke(Current);
    }

    void Update()
    {
        if (!_active) return;

        if (welfare.SocialWelfare >= Current.TargetWelfare) { End(true); return; }

        if (Current.TimeLimit > 0)
        {
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0) End(false);
        }
    }

    private void End(bool won)
    {
        _active = false;
        OnScenarioEnded?.Invoke(won);
    }
}
