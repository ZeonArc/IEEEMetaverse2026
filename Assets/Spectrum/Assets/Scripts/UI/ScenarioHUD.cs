using UnityEngine;
using TMPro;

public class ScenarioHUD : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarios;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI resultText;

    void OnEnable()
    {
        scenarios.OnScenarioStarted += OnStart;
        scenarios.OnScenarioEnded += OnEnd;
    }
    void OnDisable()
    {
        scenarios.OnScenarioStarted -= OnStart;
        scenarios.OnScenarioEnded -= OnEnd;
    }

    private void OnStart(Scenario s) => resultText.text = "";
    private void OnEnd(bool won) => resultText.text = won ? "OBJECTIVE COMPLETE!" : "TIME'S UP";

    void Update()
    {
        var s = scenarios.Current;
        if (s == null) return;
        string time = s.TimeLimit > 0 ? $"\nTime: {scenarios.TimeLeft:F0}s" : "";
        objectiveText.text = $"{s.Name}\n{s.Objective}\nTarget Welfare: {s.TargetWelfare:F0}{time}";
    }
}
