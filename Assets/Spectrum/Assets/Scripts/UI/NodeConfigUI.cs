using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class NodeConfigUI : MonoBehaviour
{
    [SerializeField] private GameObject configPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Toggle powerToggle;
    [SerializeField] private Slider bandwidthSlider;
    [SerializeField] private TextMeshProUGUI bandwidthValueText;

    private NetworkNode _currentNode;
    private bool _isConfigMode = false;

    void Start()
    {
        if (configPanel != null) configPanel.SetActive(false);
        
        powerToggle.onValueChanged.AddListener(OnPowerToggled);
        bandwidthSlider.onValueChanged.AddListener(OnBandwidthChanged);
    }

    public void ToggleConfigMode()
    {
        _isConfigMode = !_isConfigMode;
        if (!_isConfigMode && configPanel != null)
        {
            configPanel.SetActive(false);
            _currentNode = null;
        }
    }

    public void RegisterNode(NetworkNode node)
    {
        var interactable = node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener((args) => OnNodeActivated(node));
        }
    }

    private void OnNodeActivated(NetworkNode node)
    {
        if (!_isConfigMode) return;

        _currentNode = node;
        
        // Position UI near the node
        if (configPanel != null)
        {
            configPanel.SetActive(true);
            configPanel.transform.position = node.transform.position + Vector3.up * 0.4f;
            // Make it face the camera (assuming main camera is the player)
            if (Camera.main != null)
            {
                configPanel.transform.LookAt(configPanel.transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
            }
        }

        // Update UI to reflect node state
        titleText.text = $"Configure Node {node.Id}";
        powerToggle.isOn = node.IsPoweredOn;
        bandwidthSlider.value = node.BandwidthCapacity;
        bandwidthValueText.text = node.BandwidthCapacity.ToString("F1");
    }

    private void OnPowerToggled(bool isOn)
    {
        if (_currentNode != null)
        {
            _currentNode.IsPoweredOn = isOn;
        }
    }

    private void OnBandwidthChanged(float value)
    {
        if (_currentNode != null)
        {
            _currentNode.BandwidthCapacity = value;
            bandwidthValueText.text = value.ToString("F1");
        }
    }
}
