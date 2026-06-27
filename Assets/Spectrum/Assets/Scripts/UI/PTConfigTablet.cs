using UnityEngine;
using TMPro;

public class PTConfigTablet : MonoBehaviour
{
    [SerializeField] private GameObject tabletUI;
    [SerializeField] private TextMeshProUGUI titleText;
    
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField subnetInputField;
    [SerializeField] private TMP_InputField gatewayInputField;

    private PTNode _currentNode;

    void Start()
    {
        if (tabletUI != null)
        {
            tabletUI.SetActive(true);
            titleText.text = "Select a Device to Configure";
        }
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
        _currentNode = node;
        
        if (tabletUI != null)
        {
            tabletUI.SetActive(true);
            titleText.text = $"Configuring: {node.Type}";
            
            ipInputField.text = node.IPAddress;
            subnetInputField.text = node.SubnetMask;
            gatewayInputField.text = node.DefaultGateway;
        }
    }

    public void ApplyConfiguration()
    {
        if (_currentNode != null)
        {
            _currentNode.IPAddress = ipInputField.text;
            _currentNode.SubnetMask = subnetInputField.text;
            _currentNode.DefaultGateway = gatewayInputField.text;
            
            _currentNode.UpdateLabel();
            _currentNode.Log($"Configuration Updated: IP={_currentNode.IPAddress}");
        }
    }
}
