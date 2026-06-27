using UnityEngine;

// Persists how many scenarios the player has unlocked. Index 0 is always unlocked.
public class ProgressionManager : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarios;
    private const string Key = "UnlockedScenarios";

    public int Unlocked
    {
        get => Mathf.Max(1, PlayerPrefs.GetInt(Key, 1));
        private set { PlayerPrefs.SetInt(Key, value); PlayerPrefs.Save(); }
    }

    public bool IsUnlocked(int index) => index < Unlocked;

    void OnEnable() => scenarios.OnScenarioEnded += OnEnded;
    void OnDisable() => scenarios.OnScenarioEnded -= OnEnded;

    private void OnEnded(bool won)
    {
        // Win unlocks the next scenario.
        if (won && scenarios.CurrentIndex + 1 >= Unlocked && scenarios.CurrentIndex + 1 < scenarios.Count)
            Unlocked = scenarios.CurrentIndex + 2;
    }
}
