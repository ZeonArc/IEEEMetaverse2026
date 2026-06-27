using UnityEngine;
using UnityEngine.SceneManagement;

public class VRMainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject selectionScreenPanel;

    [Header("Scene Names")]
    [SerializeField] private string packetTracerSceneName = "PacketTracerScene";
    [SerializeField] private string graphSimulationSceneName = "GraphSimulationScene";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private void Start()
    {
        // Always start with the Main Menu panel active
        ShowMainMenuPanel();
    }

    public void ShowMainMenuPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (selectionScreenPanel != null) selectionScreenPanel.SetActive(false);
    }

    public void ShowSelectionScreen()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (selectionScreenPanel != null) selectionScreenPanel.SetActive(true);
    }

    public void LaunchPacketTracerMode()
    {
        Debug.Log($"Loading Scene: {packetTracerSceneName}");
        SceneManager.LoadScene(packetTracerSceneName);
    }

    public void LaunchGraphSimulationMode()
    {
        Debug.Log($"Loading Scene: {graphSimulationSceneName}");
        SceneManager.LoadScene(graphSimulationSceneName);
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log($"Returning to {mainMenuSceneName}");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitApplication()
    {
        Debug.Log("Exiting Application...");
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
