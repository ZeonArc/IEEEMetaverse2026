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
            CreateWebBrowserUI();
        }
    }
    
    private TMPro.TMP_InputField _ipInput;

    private void CreateWebBrowserUI()
    {
        // Create Canvas
        var canvasGo = new GameObject("BrowserCanvas");
        canvasGo.transform.SetParent(transform);
        canvasGo.transform.localPosition = new Vector3(-0.4f, 0.45f, -0.05f); // Below terminal
        canvasGo.transform.localScale = Vector3.one * 0.005f;
        
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Background
        var bgGo = new GameObject("BG");
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgImage = bgGo.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        bgImage.rectTransform.sizeDelta = new Vector2(300, 60);

        // Input Field
        var inputGo = new GameObject("InputField");
        inputGo.transform.SetParent(bgGo.transform, false);
        inputGo.transform.localPosition = new Vector3(-30, 0, 0);
        var inputImage = inputGo.AddComponent<UnityEngine.UI.Image>();
        inputImage.color = Color.white;
        inputImage.rectTransform.sizeDelta = new Vector2(180, 40);

        // Input Field Text
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(inputGo.transform, false);
        var text = textGo.AddComponent<TextMeshProUGUI>();
        text.color = Color.black;
        text.fontSize = 20;
        text.rectTransform.sizeDelta = new Vector2(170, 30);
        
        _ipInput = inputGo.AddComponent<TMP_InputField>();
        _ipInput.textComponent = text;
        _ipInput.text = "192.168.1.11"; // Default for quick testing
        
        // Go Button
        var btnGo = new GameObject("GoButton");
        btnGo.transform.SetParent(bgGo.transform, false);
        btnGo.transform.localPosition = new Vector3(90, 0, 0);
        var btnImage = btnGo.AddComponent<UnityEngine.UI.Image>();
        btnImage.color = Color.blue;
        btnImage.rectTransform.sizeDelta = new Vector2(80, 40);
        
        var btnTextGo = new GameObject("Text");
        btnTextGo.transform.SetParent(btnGo.transform, false);
        var btnText = btnTextGo.AddComponent<TextMeshProUGUI>();
        btnText.text = "GO";
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.fontSize = 20;
        btnText.rectTransform.sizeDelta = new Vector2(80, 40);

        var button = btnGo.AddComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(OnGoClicked);
    }
    
    private void OnGoClicked()
    {
        if (_attachedNode == null) return;
        string targetIp = _ipInput.text;
        
        AppendLog($"Sending HTTP GET to {targetIp}...");
        
        var graph = FindObjectOfType<PTGraph>();
        graph.InjectPacket(_attachedNode, targetIp, false, PTPacket.Protocol.HTTP);
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
    
    public void DisplayWebpage(string html)
    {
        // Clear terminal text and show the HTML instead!
        _terminalText.text = $"<color=white>{html}</color>";
    }

    void OnDestroy()
    {
        Disconnect();
    }
}
