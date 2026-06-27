using UnityEngine;
using UnityEditor;

using System.IO;

public class SetupNetworkPrefabs
{
    [MenuItem("Networking/Rig Prefabs for Both Modes")]
    public static void RigPrefabs()
    {
        string sourceFolder = "Assets/GeneratedNetworkAssets";
        string graphFolder = "Assets/GeneratedNetworkAssets/GraphMode";
        string ptFolder = "Assets/GeneratedNetworkAssets/PacketTracerMode";

        if (!AssetDatabase.IsValidFolder(sourceFolder))
        {
            Debug.LogError($"Source folder {sourceFolder} does not exist! Please generate the models first.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(graphFolder)) AssetDatabase.CreateFolder(sourceFolder, "GraphMode");
        if (!AssetDatabase.IsValidFolder(ptFolder)) AssetDatabase.CreateFolder(sourceFolder, "PacketTracerMode");

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { sourceFolder });
        
        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            // Skip if it's already inside one of our subfolders
            if (path.Contains("/GraphMode/") || path.Contains("/PacketTracerMode/")) continue;

            string fileName = Path.GetFileName(path);
            
            // 1. Create Graph Mode Version
            string graphPath = $"{graphFolder}/{fileName}";
            if (AssetDatabase.CopyAsset(path, graphPath))
            {
                RigForGraphMode(graphPath);
            }

            // 2. Create Packet Tracer Version
            string ptPath = $"{ptFolder}/{fileName}";
            if (AssetDatabase.CopyAsset(path, ptPath))
            {
                RigForPacketTracerMode(ptPath);
            }
        }
        
        GeneratePTExtras(ptFolder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Prefabs rigged successfully for both Graph and Packet Tracer modes!");
    }

    private static void GeneratePTExtras(string folderPath)
    {
        // Generate PTEdge Prefab
        GameObject edgeGo = new GameObject("PTEdge_Prefab");
        edgeGo.AddComponent<LineRenderer>();
        edgeGo.AddComponent<PTEdge>();
        PrefabUtility.SaveAsPrefabAsset(edgeGo, $"{folderPath}/PTEdge.prefab");
        Object.DestroyImmediate(edgeGo);

        // Generate PTPacket Prefab
        GameObject packetGo = new GameObject("PTPacket_Prefab");
        packetGo.AddComponent<PTPacket>();
        PrefabUtility.SaveAsPrefabAsset(packetGo, $"{folderPath}/PTPacket.prefab");
        Object.DestroyImmediate(packetGo);
    }

    private static void RigForGraphMode(string prefabPath)
    {
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        
        // Add Rigidbody
        var rb = contentsRoot.GetComponent<Rigidbody>();
        if (rb == null) rb = contentsRoot.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Add XR Interactable
        var grab = contentsRoot.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null) grab = contentsRoot.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.trackPosition = true;
        grab.trackRotation = false;

        // Add NetworkNode
        var node = contentsRoot.GetComponent<NetworkNode>();
        if (node == null) node = contentsRoot.AddComponent<NetworkNode>();

        if (prefabPath.Contains("Server") || prefabPath.Contains("PC"))
            node.Type = NetworkNode.NodeType.Endpoint;
        else
            node.Type = NetworkNode.NodeType.Router;

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
    }

    private static void RigForPacketTracerMode(string prefabPath)
    {
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        
        // Add Rigidbody
        var rb = contentsRoot.GetComponent<Rigidbody>();
        if (rb == null) rb = contentsRoot.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Add XR Interactable
        var grab = contentsRoot.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null) grab = contentsRoot.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.trackPosition = true;
        grab.trackRotation = false;

        // Add PTNode
        var node = contentsRoot.GetComponent<PTNode>();
        if (node == null) node = contentsRoot.AddComponent<PTNode>();

        if (prefabPath.Contains("Server")) node.Type = PTNode.DeviceType.Server;
        else if (prefabPath.Contains("PC")) 
        {
            node.Type = PTNode.DeviceType.PC;
            if (contentsRoot.GetComponent<PTMonitor>() == null) contentsRoot.AddComponent<PTMonitor>();
        }
        else if (prefabPath.Contains("Switch")) node.Type = PTNode.DeviceType.Switch;
        else if (prefabPath.Contains("Router")) node.Type = PTNode.DeviceType.Router;
        else if (prefabPath.Contains("Firewall")) node.Type = PTNode.DeviceType.Firewall;
        else if (prefabPath.Contains("Hub")) node.Type = PTNode.DeviceType.Hub;
        else if (prefabPath.Contains("Monitor")) 
        {
            node.Type = PTNode.DeviceType.Monitor;
            if (contentsRoot.GetComponent<PTMonitor>() == null) contentsRoot.AddComponent<PTMonitor>();
        }
        else node.Type = PTNode.DeviceType.Router;

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
    }
}
