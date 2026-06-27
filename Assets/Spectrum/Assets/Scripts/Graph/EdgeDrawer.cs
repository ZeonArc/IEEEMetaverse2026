using UnityEngine;

using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class EdgeDrawer : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool _isDrawingMode = false;
    private NetworkNode _firstNode = null;
    private Transform _interactorTransform = null;
    
    private LineRenderer _previewLine;
    private List<Vector3> _drawnPoints = new List<Vector3>();

    void Awake()
    {
        _previewLine = GetComponent<LineRenderer>();
        _previewLine.startWidth = 0.01f;
        _previewLine.endWidth = 0.01f;
        _previewLine.positionCount = 0;
    }

    public void ToggleDrawingMode()
    {
        _isDrawingMode = !_isDrawingMode;
        ResetSelection();
        UpdateStatusText();
    }

    public void RegisterNode(NetworkNode node)
    {
        var interactable = node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener((args) => OnNodeSelected(node, args.interactorObject.transform));
        }
    }

    private void OnNodeSelected(NetworkNode node, Transform interactor)
    {
        if (!_isDrawingMode) return;

        if (_firstNode == null)
        {
            _firstNode = node;
            _interactorTransform = interactor;
            _drawnPoints.Clear();
            _drawnPoints.Add(node.transform.position);
            UpdateStatusText();
        }
        else
        {
            if (_firstNode != node) // Don't connect a node to itself
            {
                _drawnPoints.Add(node.transform.position);
                graph.AddEdge(_firstNode, node, new List<Vector3>(_drawnPoints));
            }
            
            ResetSelection();
        }
    }

    void Update()
    {
        if (_isDrawingMode && _firstNode != null && _interactorTransform != null)
        {
            Vector3 currentPos = _interactorTransform.position;
            Vector3 lastPoint = _drawnPoints[_drawnPoints.Count - 1];
            
            // Record a point every 5cm
            if (Vector3.Distance(currentPos, lastPoint) > 0.05f)
            {
                _drawnPoints.Add(currentPos);
            }

            // Draw the preview
            _previewLine.positionCount = _drawnPoints.Count + 1;
            _previewLine.SetPositions(_drawnPoints.ToArray());
            _previewLine.SetPosition(_drawnPoints.Count, currentPos);
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

        if (!_isDrawingMode)
        {
            statusText.text = "Edge Drawer: OFF";
        }
        else if (_firstNode == null)
        {
            statusText.text = "Edge Drawer: Select Node A";
        }
        else
        {
            statusText.text = "Edge Drawer: Draw & Select Node B";
        }
    }
}
