using UnityEngine;

using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable), typeof(Rigidbody))]
public class PTNode : MonoBehaviour
{
    public enum DeviceType { PC, Server, Switch, Router, Hub, Monitor }
    public DeviceType Type = DeviceType.Router;
    public bool IsPoweredOn = true;
    
    public System.Action<string> OnLogEvent;

    [Header("Network Identity")]
    public string IPAddress = "0.0.0.0";
    public string SubnetMask = "255.255.255.0";
    public string DefaultGateway = "0.0.0.0";
    public string MACAddress;

    public List<PTEdge> Connections = new List<PTEdge>();
    public Dictionary<string, PTEdge> RoutingTable = new Dictionary<string, PTEdge>(); // Target Subnet -> Edge
    public Dictionary<string, string> ArpTable = new Dictionary<string, string>(); // IP -> MAC

    private List<Renderer> _ledRenderers = new List<Renderer>();
    private static readonly int ColorProp = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorProp = Shader.PropertyToID("_Color");
    
    private TMPro.TextMeshPro _ipText;

    void Awake()
    {
        MACAddress = System.Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            if (r.name.Contains("LED") || r.name.Contains("PowerButton"))
            {
                _ledRenderers.Add(r);
                r.material.EnableKeyword("_EMISSION");
            }
        }

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
    }

    void Start()
    {
        // Create a glowing, floating text label
        var textGo = new GameObject("IP_Label");
        textGo.transform.SetParent(transform);
        textGo.transform.localPosition = new Vector3(0, 0.4f, 0);
        
        textGo.AddComponent<PTBillboard>();
        
        _ipText = textGo.AddComponent<TMPro.TextMeshPro>();
        _ipText.fontSize = 2f;
        _ipText.alignment = TMPro.TextAlignmentOptions.Center;
        _ipText.color = Color.cyan;
        
        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (_ipText != null)
        {
            // Position the label dynamically so it doesn't hide inside models
            float yPos = 0.4f;
            if (Type == DeviceType.PC || Type == DeviceType.Server) yPos = 1.1f;
            else if (Type == DeviceType.Monitor) yPos = 0.9f;
            else yPos = 0.5f;

            _ipText.transform.localPosition = new Vector3(0, yPos, 0);
            
            // For monitors, rotate the text slightly if needed, or just let it float higher
            if (Type == DeviceType.Switch || Type == DeviceType.Hub)
            {
                _ipText.text = $"{Type.ToString()}\n<size=50%>(Layer 2)</size>";
            }
            else
            {
                _ipText.text = $"{Type.ToString()}\n<size=80%>{IPAddress}</size>";
            }
        }
    }

    void Update()
    {
        Color c = IsPoweredOn ? Color.green : Color.gray;

        foreach (var r in _ledRenderers)
        {
            if (r.name.Contains("LED") || r.name.Contains("PowerButton"))
            {
                r.material.SetColor(ColorProp, c * 2f);
                r.material.SetColor(BaseColorProp, c);
            }
            else
            {
                r.material.SetColor(BaseColorProp, c);
            }
        }
    }

    public void Log(string msg)
    {
        Debug.Log($"[{name}] {msg}");
        OnLogEvent?.Invoke(msg);
    }

    public Dictionary<string, PTEdge> MacTable = new Dictionary<string, PTEdge>(); // Simulated MAC Table

    public void ReceivePacket(PTPacket packet, PTEdge incomingEdge)
    {
        if (!IsPoweredOn) return;

        // Layer 1: Hub forwards everywhere except where it came from
        if (Type == DeviceType.Hub)
        {
            foreach (var edge in Connections)
            {
                // We have to clone the packet visually to send it down multiple paths
                if (edge != incomingEdge) 
                {
                    var clone = Instantiate(packet.gameObject).GetComponent<PTPacket>();
                    clone.Init(packet.SourceIP, packet.DestinationIP);
                    clone.Payload = packet.Payload;
                    edge.Transmit(clone, this);
                }
            }
            Destroy(packet.gameObject); // Destroy the original since we cloned it
            return;
        }

        // Layer 2: Switch (Learn Source, Forward to Dest or Broadcast)
        if (Type == DeviceType.Switch)
        {
            // Learn (using IP as MAC proxy for sandbox simplicity)
            if (!MacTable.ContainsKey(packet.SourceIP))
            {
                MacTable[packet.SourceIP] = incomingEdge;
                Log($"Learned MAC/IP: {packet.SourceIP}");
            }

            // Forward
            if (MacTable.ContainsKey(packet.DestinationIP))
            {
                var targetEdge = MacTable[packet.DestinationIP];
                if (targetEdge != incomingEdge)
                {
                    Log($"Forwarding packet to {packet.DestinationIP} via specific port.");
                    targetEdge.Transmit(packet, this);
                }
            }
            else
            {
                // Broadcast if destination is unknown
                Log($"Destination unknown. Broadcasting packet.");
                foreach (var edge in Connections)
                {
                    if (edge != incomingEdge) 
                    {
                        var clone = Instantiate(packet.gameObject).GetComponent<PTPacket>();
                        clone.Init(packet.SourceIP, packet.DestinationIP);
                        clone.Payload = packet.Payload;
                        edge.Transmit(clone, this);
                    }
                }
                Destroy(packet.gameObject); // Destroy original
            }
            return;
        }

        // Layer 3: PC/Server
        if (Type == DeviceType.PC || Type == DeviceType.Server)
        {
            if (packet.DestinationIP == IPAddress)
            {
                if (packet.Payload == "Ping Request")
                {
                    Log($"Ping Request from {packet.SourceIP}! Sending Reply...");
                    StartCoroutine(FlashColor(Color.green));
                    
                    // Generate Reply
                    var ptGraph = FindObjectOfType<PTGraph>();
                    if (ptGraph != null && Connections.Count > 0)
                    {
                        var go = Instantiate(ptGraph.PacketPrefab, transform.position, Quaternion.identity);
                        var replyPacket = go.GetComponent<PTPacket>();
                        replyPacket.Init(IPAddress, packet.SourceIP);
                        replyPacket.Payload = "Ping Reply";
                        Connections[0].Transmit(replyPacket, this);
                    }
                }
                else if (packet.Payload == "Ping Reply")
                {
                    Log($"Ping Reply received from {packet.SourceIP}!");
                    StartCoroutine(FlashColor(Color.cyan)); // Cyan for successful round trip!
                }
                
                Destroy(packet.gameObject);
                return;
            }
        }

        // Layer 3: Router
        if (Type == DeviceType.Router)
        {
            string targetSubnet = GetSubnet(packet.DestinationIP, SubnetMask);
            if (RoutingTable.ContainsKey(targetSubnet))
            {
                Log($"Forwarding to {targetSubnet}...");
                StartCoroutine(FlashColor(Color.yellow));
                RoutingTable[targetSubnet].Transmit(packet, this);
            }
            else
            {
                Log($"Request timed out (No route to {packet.DestinationIP})");
                StartCoroutine(FlashColor(Color.red));
                Destroy(packet.gameObject);
            }
        }
    }

    private System.Collections.IEnumerator FlashColor(Color color)
    {
        foreach (var r in _ledRenderers)
        {
            r.material.SetColor(ColorProp, color * 3f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Return to normal color
        Color c = IsPoweredOn ? Color.green : Color.gray;
        foreach (var r in _ledRenderers)
        {
            if (r.name.Contains("LED") || r.name.Contains("PowerButton"))
            {
                r.material.SetColor(ColorProp, c * 2f);
            }
        }
    }

    public static string GetSubnet(string ip, string mask)
    {
        // Very basic string-based subnet calculator for 255.255.255.0
        string[] ipParts = ip.Split('.');
        string[] maskParts = mask.Split('.');
        if (ipParts.Length == 4 && maskParts.Length == 4)
        {
            return $"{int.Parse(ipParts[0]) & int.Parse(maskParts[0])}." +
                   $"{int.Parse(ipParts[1]) & int.Parse(maskParts[1])}." +
                   $"{int.Parse(ipParts[2]) & int.Parse(maskParts[2])}.0";
        }
        return ip;
    }
}
