using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PacketInspector : MonoBehaviour
{
    [SerializeField] private RoutingAgent agent;
    [SerializeField] private GameObject inspectorUI;
    [SerializeField] private TextMeshProUGUI sourceText;
    [SerializeField] private TextMeshProUGUI destText;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (_interactable != null)
        {
            _interactable.selectEntered.AddListener(OnGrabbed);
            _interactable.selectExited.AddListener(OnReleased);
        }

        if (inspectorUI != null) inspectorUI.SetActive(false);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (agent != null)
        {
            agent.IsPaused = true;
            
            if (inspectorUI != null)
            {
                inspectorUI.SetActive(true);
                sourceText.text = $"Src: Node {agent.Source?.Id}";
                destText.text = $"Dst: Node {agent.Destination?.Id}";
            }
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (agent != null)
        {
            agent.IsPaused = false;
        }

        if (inspectorUI != null)
        {
            inspectorUI.SetActive(false);
        }
    }
}
