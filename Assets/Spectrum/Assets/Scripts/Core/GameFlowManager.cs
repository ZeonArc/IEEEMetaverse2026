using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coordinates the hub → play → results loop.
public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarios;
    [SerializeField] private ProgressionManager progression;
    [SerializeField] private GameObject hubPanel;      // scenario selection panel
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button menuButton;

    void Start()
    {
        retryButton.onClick.AddListener(() => scenarios.StartScenario(scenarios.CurrentIndex));
        nextButton.onClick.AddListener(StartNext);
        menuButton.onClick.AddListener(ShowHub);
        ShowHub();
    }

    void OnEnable()
    {
        scenarios.OnScenarioStarted += OnStarted;
        scenarios.OnScenarioEnded += ShowResults;
    }
    void OnDisable()
    {
        scenarios.OnScenarioStarted -= OnStarted;
        scenarios.OnScenarioEnded -= ShowResults;
    }

    private void OnStarted(Scenario s) // hide menus while playing
    {
        hubPanel.SetActive(false);
        resultsPanel.SetActive(false);
    }

    private void ShowHub()
    {
        hubPanel.SetActive(true);
        resultsPanel.SetActive(false);
    }

    private void ShowResults(bool won)
    {
        resultsPanel.SetActive(true);
        resultText.text = won ? "OBJECTIVE COMPLETE" : "TIME'S UP";
        int next = scenarios.CurrentIndex + 1;
        nextButton.interactable = won && next < scenarios.Count && progression.IsUnlocked(next);
    }

    private void StartNext()
    {
        int next = scenarios.CurrentIndex + 1;
        if (next < scenarios.Count) scenarios.StartScenario(next);
    }
}
