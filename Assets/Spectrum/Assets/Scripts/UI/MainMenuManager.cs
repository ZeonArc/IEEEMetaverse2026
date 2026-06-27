using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("The initial panel with the Play button")]
    [SerializeField] private GameObject startScreenPanel;
    
    [Tooltip("The panel containing the different learning modules")]
    [SerializeField] private GameObject selectionScreenPanel;

    [Header("Scenes")]
    [Tooltip("Exact name of the Networking Path scene in Build Settings")]
    [SerializeField] private string networkingSceneName = "NetworkingPath";
    
    [Tooltip("Exact name of the Mechanism Design Path scene in Build Settings")]
    [SerializeField] private string mechanismDesignSceneName = "CommandCenter";

    [Tooltip("Exact name of the new Sandbox Mode scene in Build Settings")]
    [SerializeField] private string sandboxSceneName = "SandboxMode";

    void Start()
    {
        // Ensure we start on the correct screen
        ShowStartScreen();
    }

    public void ShowSelectionScreen()
    {
        if (startScreenPanel != null) startScreenPanel.SetActive(false);
        if (selectionScreenPanel != null) selectionScreenPanel.SetActive(true);
    }

    public void ShowStartScreen()
    {
        if (startScreenPanel != null) startScreenPanel.SetActive(true);
        if (selectionScreenPanel != null) selectionScreenPanel.SetActive(false);
    }

    public void LoadNetworkingPath()
    {
        SceneManager.LoadScene(networkingSceneName);
    }

    public void LoadMechanismDesignPath()
    {
        SceneManager.LoadScene(mechanismDesignSceneName);
    }

    public void LoadSandboxMode()
    {
        SceneManager.LoadScene(sandboxSceneName);
    }

    public void QuitExperience()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
