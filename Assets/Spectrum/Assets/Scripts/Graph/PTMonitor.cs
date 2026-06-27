using UnityEngine;
using TMPro;

[RequireComponent(typeof(PTNode))]
public class PTMonitor : MonoBehaviour
{
    private TextMeshPro _terminalText;
    private PTNode _attachedNode;
    
    void Start()
    {
        var textGo = new GameObject("Terminal_Text");
        textGo.transform.SetParent(transform);
        
        var node = GetComponent<PTNode>();
        
        // Position terminal text over the screen
        if (node != null && node.Type == PTNode.DeviceType.PC)
        {
            textGo.transform.localPosition = new Vector3(-0.4f, 0.55f, -0.05f); 
        }
        else
        {
            textGo.transform.localPosition = new Vector3(0, 0.3f, 0.25f); 
        }
        
        _terminalText = textGo.AddComponent<TextMeshPro>();
        _terminalText.fontSize = 1.2f;
        _terminalText.alignment = TextAlignmentOptions.TopLeft;
        _terminalText.color = Color.green;
        
        // Make text fit in a specific area
        _terminalText.rectTransform.sizeDelta = new Vector2(1.5f, 1f);
        
        _terminalText.text = "Monitor Ready.\nWaiting for cable connection...";
        
        // Auto-connect if it's a PC!
        if (node != null && node.Type == PTNode.DeviceType.PC)
        {
            ConnectToNode(node);
        }
    }
    
    public void ConnectToNode(PTNode node)
    {
        _attachedNode = node;
        _terminalText.text = $"Connected to {node.Type}\nIP: {node.IPAddress}\nSubnet: {node.SubnetMask}\n---";
        node.OnLogEvent += AppendLog;
    }
    
    public void Disconnect()
    {
        if (_attachedNode != null)
        {
            _attachedNode.OnLogEvent -= AppendLog;
            _attachedNode = null;
        }
        _terminalText.text = "Monitor Disconnected.";
    }
    
    public void AppendLog(string message)
    {
        _terminalText.text += $"\n> {message}";
        
        // Keep to max 6 lines so it fits on screen
        string[] lines = _terminalText.text.Split('\n');
        if (lines.Length > 7)
        {
            _terminalText.text = string.Join("\n", lines, lines.Length - 7, 7);
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }
}
