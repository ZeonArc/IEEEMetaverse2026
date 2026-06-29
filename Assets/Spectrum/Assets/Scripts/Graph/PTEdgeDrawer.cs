using UnityEngine;

using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PTEdgeDrawer : MonoBehaviour
{
    [SerializeField] private PTGraph graph;
    [SerializeField] private TextMeshProUGUI statusText;

    public enum DrawerState { OFF, Cat5, Fiber }
    private DrawerState _currentState = DrawerState.OFF;

    private PTNode _firstNode = null;
    private Transform _interactorTransform = null;
    
    private LineRenderer _previewLine;
    private List<Vector3> _drawnPoints = new List<Vector3>();

    void Awake()
    {
        _previewLine = GetComponent<LineRenderer>();
        _previewLine.startWidth = 0.01f;
        _previewLine.endWidth = 0.01f;
        _previewLine.positionCount = 0;
        
        // Ensure it's visible by assigning a default material
        if (_previewLine.material == null || _previewLine.material.name == "Default-Material")
        {
            _previewLine.material = new Material(Shader.Find("Sprites/Default"));
            _previewLine.material.color = Color.yellow;
        }
    }

    public void ToggleDrawingMode()
    {
        if (_currentState == DrawerState.OFF) _currentState = DrawerState.Cat5;
        else if (_currentState == DrawerState.Cat5) _currentState = DrawerState.Fiber;
        else _currentState = DrawerState.OFF;

        ResetSelection();
        UpdateStatusText();
        Debug.Log($"[PTEdgeDrawer] ToggleDrawingMode called. State: {_currentState}");
    }

    public void RegisterNode(PTNode node)
    {
        var interactable = node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener((args) => OnNodeSelected(node, args.interactorObject.transform));
            Debug.Log($"[PTEdgeDrawer] Successfully registered node: {node.gameObject.name}");
        }
        else
        {
            Debug.LogError($"[PTEdgeDrawer] Failed to register node {node.gameObject.name}: Missing XRGrabInteractable!");
        }
    }

    private void OnNodeSelected(PTNode node, Transform interactor)
    {
        Debug.Log($"[PTEdgeDrawer] Node selected: {node.gameObject.name}. State: {_currentState}");
        if (_currentState == DrawerState.OFF) return;

        if (_firstNode == null)
        {
            _firstNode = node;
            _interactorTransform = interactor;
            UpdateStatusText();
            Debug.Log($"[PTEdgeDrawer] Set first node to: {node.gameObject.name}");
        }
        else
        {
            if (_firstNode != node) // Don't connect a node to itself
            {
                Debug.Log($"[PTEdgeDrawer] Connecting {_firstNode.gameObject.name} to {node.gameObject.name}");
                var cableType = _currentState == DrawerState.Fiber ? PTEdge.CableType.Fiber : PTEdge.CableType.Cat5;
                graph.AddEdge(_firstNode, node, null, cableType);
            }
            else
            {
                Debug.Log("[PTEdgeDrawer] Cancelled drawing (selected the same node).");
            }
            
            ResetSelection();
        }
    }

    void Update()
    {
        if (_currentState != DrawerState.OFF && _firstNode != null && _interactorTransform != null)
        {
            // Draw a straight preview line from the first node to the player's hand
            _previewLine.positionCount = 2;
            _previewLine.SetPosition(0, _firstNode.transform.position);
            _previewLine.SetPosition(1, _interactorTransform.position);
            
            if (_currentState == DrawerState.Fiber) _previewLine.material.color = new Color(1f, 0.5f, 0f);
            else _previewLine.material.color = Color.yellow;
        }
    }

    public void ResetSelection()
    {
        _firstNode = null;
        _interactorTransform = null;
        _drawnPoints.Clear();
        _previewLine.positionCount = 0;
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (statusText == null) return;

        if (_currentState == DrawerState.OFF)
        {
            statusText.text = "Cable Drawer: OFF";
        }
        else if (_firstNode == null)
        {
            statusText.text = $"Drawer ({_currentState}): Grab Device A";
        }
        else
        {
            statusText.text = $"Drawer ({_currentState}): Draw & Grab Device B";
        }
    }
}
