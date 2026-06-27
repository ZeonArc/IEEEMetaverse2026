using UnityEngine;
using TMPro;

public class PTPingTool : MonoBehaviour
{
    [SerializeField] private PTGraph graph;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool _isPingMode = false;
    private bool _isHackerMode = false;
    private PTNode _sourceNode = null;

    public void TogglePingMode()
    {
        _isPingMode = !_isPingMode;
        _isHackerMode = false;
        ResetSelection();
        UpdateStatusText();
    }

    public void ToggleHackerMode()
    {
        _isPingMode = !_isPingMode;
        _isHackerMode = _isPingMode; // If ping mode turned on, it's hacker mode. If off, it's off.
        ResetSelection();
        UpdateStatusText();
    }

    public void RegisterNode(PTNode node)
    {
        var interactable = node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener((args) => OnNodeSelected(node));
        }
    }

    private void OnNodeSelected(PTNode node)
    {
        if (!_isPingMode) return;

        // Can't ping FROM a switch/hub, only layer 3 devices
        if (_sourceNode == null)
        {
            if (node.Type == PTNode.DeviceType.Switch || node.Type == PTNode.DeviceType.Hub)
            {
                if (statusText) statusText.text = "Error: Must select PC/Server/Router as Source";
                return;
            }

            _sourceNode = node;
            UpdateStatusText();
        }
        else
        {
            if (_sourceNode != node) // Don't ping yourself
            {
                // Inject the packet into the network!
                graph.InjectPacket(_sourceNode, node.IPAddress, _isHackerMode);
                string packetType = _isHackerMode ? "<color=red>Malicious Packet</color>" : "Ping";
                if (statusText) statusText.text = $"{packetType} Sent to {node.IPAddress}!";
            }
            
            // Allow them to immediately send another ping by keeping Ping Mode on but resetting source
            ResetSelection();
            // Turn it back on so they can select the next source
            _isPingMode = true; 
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

        string toolName = _isHackerMode ? "<color=red>Hacker Tool</color>" : "Ping Tool";

        if (!_isPingMode)
        {
            statusText.text = $"{toolName}: OFF";
        }
        else if (_sourceNode == null)
        {
            statusText.text = $"{toolName}: Grab Source Device";
        }
        else
        {
            statusText.text = $"{toolName}: Grab Destination Device\n<size=50%>(Source: {_sourceNode.IPAddress})</size>";
        }
    }
}
