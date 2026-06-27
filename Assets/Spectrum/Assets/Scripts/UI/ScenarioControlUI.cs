using UnityEngine;
using UnityEngine.UI;
using TMPro;

// World-space buttons to pick and (re)start scenarios.
public class ScenarioControlUI : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarios;
    [SerializeField] private ProgressionManager progression;
    [SerializeField] private Button nextButton;   // cycle to next scenario
    [SerializeField] private Button startButton;   // start the selected one
    [SerializeField] private TextMeshProUGUI selectionLabel;

    private int _index;

    void Start()
    {
        nextButton.onClick.AddListener(Next);
        startButton.onClick.AddListener(TryStart);
        Refresh();
    }

    private void Next()
    {
        _index = (_index + 1) % scenarios.Count;
        Refresh();
    }

    private void TryStart()
    {
        if (progression.IsUnlocked(_index)) scenarios.StartScenario(_index);
    }

    private void Refresh()
    {
        bool locked = !progression.IsUnlocked(_index);
        startButton.interactable = !locked;
        selectionLabel.text = $"Scenario {_index + 1} / {scenarios.Count}{(locked ? "  [LOCKED]" : "")}";
    }
}
