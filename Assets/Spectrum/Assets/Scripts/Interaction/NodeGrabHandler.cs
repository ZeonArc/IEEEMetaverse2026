using UnityEngine;


public class NodeGrabHandler : MonoBehaviour
{
    void Start()
    {
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.trackPosition = true;
        grab.trackRotation = false;
        grab.throwOnDetach = false;
    }
}