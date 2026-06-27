using UnityEngine;

using TMPro;

public class PacketInjector : MonoBehaviour
{
    [SerializeField] private AgentSpawner agentSpawner;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool _isInjectingMode = false;
    private NetworkNode _sourceNode = null;

    public void ToggleInjectingMode()
    {
        _isInjectingMode = !_isInjectingMode;
        ResetSelection();
        UpdateStatusText();
    }

    public void RegisterNode(NetworkNode node)
    {
        var interactable = node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable != null)
        {
            // Subscribe to the 'selectEntered' event (grabbing or pointing and clicking with Ray Interactor)
            interactable.selectEntered.AddListener((args) => OnNodeActivated(node));
        }
    }

    private void OnNodeActivated(NetworkNode node)
    {
        if (!_isInjectingMode) return;

        if (_sourceNode == null)
        {
            _sourceNode = node;
            UpdateStatusText();
        }
        else
        {
            if (_sourceNode != node) // Don't route to itself
            {
                agentSpawner.SpawnSingleAgent(_sourceNode, node);
            }
            
            // Reset for the next pair
            _sourceNode = null;
            UpdateStatusText();
        }
    }

    public void ResetSelection()
    {
        _sourceNode = null;
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (statusText == null) return;

        if (!_isInjectingMode)
        {
            statusText.text = "Packet Injector: OFF";
        }
        else if (_sourceNode == null)
        {
            statusText.text = "Packet Injector: Select Source Node";
        }
        else
        {
            statusText.text = "Packet Injector: Select Destination Node";
        }
    }
}
