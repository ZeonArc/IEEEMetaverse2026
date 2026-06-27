using UnityEngine;
using UnityEngine.UI;

// World-space panel of buttons driving the SimulationController.
public class SimControlUI : MonoBehaviour
{
    [SerializeField] private SimulationController sim;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button fastForwardButton;
    [SerializeField] private Button addAgentsButton;
    [SerializeField] private int agentsPerClick = 4;

    void Start()
    {
        resetButton.onClick.AddListener(sim.ResetSimulation);
        pauseButton.onClick.AddListener(sim.TogglePause);
        fastForwardButton.onClick.AddListener(sim.ToggleFastForward);
        addAgentsButton.onClick.AddListener(() => sim.AddAgents(agentsPerClick));
    }
}
