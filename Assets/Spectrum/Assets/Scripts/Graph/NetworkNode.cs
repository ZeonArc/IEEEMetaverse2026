using UnityEngine;

using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable), typeof(Rigidbody))]
public class NetworkNode : MonoBehaviour
{
    public enum NodeType { Router, Endpoint }
    public NodeType Type = NodeType.Router;
    public bool IsPoweredOn = true;

    public int Id;
    public float BandwidthCapacity = 10f;
    public float CurrentLoad;

    public float CongestionRatio => BandwidthCapacity > 0 ? Mathf.Clamp01(CurrentLoad / BandwidthCapacity) : 0f;

    private List<Renderer> _ledRenderers = new List<Renderer>();
    private static readonly int ColorProp = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorProp = Shader.PropertyToID("_Color");
    private TMPro.TextMeshPro _label;

    void Awake()
    {
        // Find all Renderers that are LEDs
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            if (r.name.Contains("LED") || r.name.Contains("PowerButton"))
            {
                _ledRenderers.Add(r);
                r.material.EnableKeyword("_EMISSION");
            }
        }

        // If no LEDs found, just grab the first renderer we find (fallback)
        if (_ledRenderers.Count == 0)
        {
            var fallback = GetComponentInChildren<Renderer>();
            if (fallback != null) _ledRenderers.Add(fallback);
        }

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
        {
            grab.trackPosition = true;
            grab.trackRotation = false;
            grab.throwOnDetach = false;
        }

        // Add Floating Label
        var labelGo = new GameObject("NodeLabel");
        labelGo.transform.SetParent(transform);
        labelGo.transform.localPosition = new Vector3(0, 0.6f, 0);
        
        _label = labelGo.AddComponent<TMPro.TextMeshPro>();
        _label.fontSize = 2f;
        _label.alignment = TMPro.TextAlignmentOptions.Center;
        
        // Ensure it always faces the player
        labelGo.AddComponent<PTBillboard>();
    }

    void Update()
    {
        Color c = Color.gray;
        if (IsPoweredOn)
        {
            c = Color.Lerp(Color.green, Color.red, CongestionRatio);
            if (Type == NodeType.Endpoint) c = Color.Lerp(Color.cyan, Color.blue, CongestionRatio);
        }

        foreach (var r in _ledRenderers)
        {
            // Set emission if it's an LED, otherwise just set base color
            if (r.name.Contains("LED") || r.name.Contains("PowerButton"))
            {
                r.material.SetColor(ColorProp, c * 2f); // Boost emission
                r.material.SetColor(BaseColorProp, c);
            }
            else
            {
                r.material.SetColor(BaseColorProp, c);
            }
        }

        if (_label != null)
        {
            _label.text = $"Node {Id}\nLoad: {CurrentLoad:F1}/{BandwidthCapacity:F1}";
            _label.color = c;
        }
    }
}
