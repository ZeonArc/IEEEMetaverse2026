using UnityEngine;
using UnityEditor;

public class NetworkModelGenerator
{
    static Material CreateMat(string name, Color color, float metallic, float smoothness, float emission = 0)
    {
        string matFolder = "Assets/GeneratedNetworkAssets/Materials";
        if (!AssetDatabase.IsValidFolder(matFolder))
        {
            AssetDatabase.CreateFolder("Assets/GeneratedNetworkAssets", "Materials");
        }

        string matPath = $"{matFolder}/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

        if (mat == null)
        {
            // Create a temporary primitive to steal the correct default material for the active render pipeline
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mat = new Material(tempCube.GetComponent<Renderer>().sharedMaterial);
            Object.DestroyImmediate(tempCube);
            
            AssetDatabase.CreateAsset(mat, matPath);
        }

        // URP/HDRP use _BaseColor, Standard uses _Color
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            
            if (emission > 0)
            {
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", color * emission);
            }
        }
        else
        {
            if (mat.HasProperty("_Color")) mat.color = color;
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
            
            if (emission > 0)
            {
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", color * emission);
            }
        }
        
        EditorUtility.SetDirty(mat);
        return mat;
    }

    // Helper to create a cube and parent it
    static GameObject CreateCube(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent);
        cube.transform.localPosition = pos;
        cube.transform.localScale = scale;
        cube.GetComponent<Renderer>().material = mat;
        return cube;
    }

    // Helper to create a cylinder and parent it
    static GameObject CreateCylinder(string name, Vector3 pos, Vector3 scale, Vector3 rotation, Material mat, Transform parent)
    {
        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cyl.name = name;
        cyl.transform.SetParent(parent);
        cyl.transform.localPosition = pos;
        cyl.transform.localEulerAngles = rotation;
        cyl.transform.localScale = scale;
        cyl.GetComponent<Renderer>().material = mat;
        return cyl;
    }

    // --- MODEL BUILDERS ---

    [MenuItem("Networking/Generate Network Prefabs")]
    public static void GenerateAllPrefabs()
    {
        string folderPath = "Assets/GeneratedNetworkAssets";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedNetworkAssets");
        }

        // Define Materials
        Material matServer = CreateMat("Mat_Server", new Color(0.13f, 0.13f, 0.13f), 0.7f, 0.3f);
        Material matSwitch = CreateMat("Mat_Switch", new Color(0.1f, 0.1f, 0.1f), 0.6f, 0.4f);
        Material matRouter = CreateMat("Mat_Router", new Color(0.2f, 0.2f, 0.2f), 0.6f, 0.4f);
        Material matHub = CreateMat("Mat_Hub", new Color(0.8f, 0.8f, 0.66f), 0.1f, 0.8f); // Beige
        Material matPC = CreateMat("Mat_PC", new Color(0.06f, 0.06f, 0.06f), 0.8f, 0.2f);
        Material matBlack = CreateMat("Mat_Black", Color.black, 0.2f, 0.8f);
        Material matPortTeal = CreateMat("Mat_PortTeal", new Color(0f, 1f, 0.8f), 0.1f, 0.5f, 2f);
        Material matPortYellow = CreateMat("Mat_PortYellow", new Color(1f, 0.6f, 0f), 0.1f, 0.5f, 2f);
        Material matLEDGreen = CreateMat("Mat_LEDGreen", Color.green, 0.1f, 0.5f, 3f);
        Material matLEDRed = CreateMat("Mat_LEDRed", Color.red, 0.1f, 0.5f, 3f);
        Material matScreen = CreateMat("Mat_Screen", new Color(0f, 0.1f, 0.2f), 0.1f, 0.5f, 1f);

        CreateAndSavePrefab("Server", BuildServer(matServer, matBlack, matLEDGreen), folderPath);
        CreateAndSavePrefab("Switch", BuildSwitch(matSwitch, matPortTeal, matLEDGreen), folderPath);
        CreateAndSavePrefab("Router", BuildRouter(matRouter, matPortYellow, matBlack), folderPath);
        CreateAndSavePrefab("Hub", BuildHub(matHub, matBlack, matLEDRed), folderPath);
        CreateAndSavePrefab("PC", BuildPC(matPC, matBlack, matPortTeal, matScreen), folderPath);
        CreateAndSavePrefab("Monitor", BuildMonitor(matPC, matScreen), folderPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Network Prefabs Generated in " + folderPath);
    }

    static void CreateAndSavePrefab(string name, GameObject root, string folderPath)
    {
        string localPath = $"{folderPath}/{name}.prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(root, localPath);
        
        // Destroy the scene instance after saving as prefab
        Object.DestroyImmediate(root);
    }

    static GameObject BuildServer(Material bodyMat, Material bayMat, Material ledMat)
    {
        GameObject root = new GameObject("Server");
        
        CreateCube("Body", new Vector3(0, 0.22f, 0), new Vector3(1, 0.44f, 0.9f), bodyMat, root.transform);
        
        for (int i = 0; i < 6; i++)
        {
            float x = -0.7f + (i * 0.28f); 
            CreateCube($"Bay_{i}", new Vector3(x, 0.45f, 0.22f), new Vector3(0.11f, 0.02f, 0.15f), bayMat, root.transform);
            CreateCube($"LED_{i}", new Vector3(x - 0.08f, 0.46f, 0.32f), new Vector3(0.01f, 0.01f, 0.01f), ledMat, root.transform);
        }

        return root;
    }

    static GameObject BuildSwitch(Material bodyMat, Material portMat, Material ledMat)
    {
        GameObject root = new GameObject("Switch");
        CreateCube("Body", new Vector3(0, 0.11f, 0), new Vector3(1, 0.22f, 0.9f), bodyMat, root.transform);
        
        for (int i = 0; i < 16; i++)
        {
            float x = -0.72f + (i * 0.096f);
            CreateCube($"Port_{i}", new Vector3(x, 0.45f, 0.06f), new Vector3(0.08f, 0.02f, 0.05f), portMat, root.transform);
            CreateCube($"LED_{i}", new Vector3(x, 0.46f, 0.13f), new Vector3(0.01f, 0.01f, 0.01f), ledMat, root.transform);
        }
        return root;
    }

    static GameObject BuildRouter(Material bodyMat, Material portMat, Material blackMat)
    {
        GameObject root = new GameObject("Router");
        CreateCube("Body", new Vector3(0, 0.11f, 0), new Vector3(1, 0.22f, 0.9f), bodyMat, root.transform);
        
        for (int i = 0; i < 4; i++)
        {
            float x = -0.3f + (i * 0.2f);
            CreateCube($"Port_{i}", new Vector3(x, 0.45f, 0.11f), new Vector3(0.14f, 0.02f, 0.07f), portMat, root.transform);
        }
        
        // Antennas
        for (int i = 0; i < 2; i++)
        {
            float x = -0.5f + (i * 1.0f);
            CreateCylinder($"Antenna_{i}", new Vector3(x, -0.4f, 0.35f), new Vector3(0.02f, 0.3f, 0.02f), new Vector3(30, 0, 0), blackMat, root.transform);
        }
        return root;
    }

    static GameObject BuildHub(Material bodyMat, Material portMat, Material ledMat)
    {
        GameObject root = new GameObject("Hub");
        CreateCube("Body", new Vector3(0, 0.11f, 0), new Vector3(1, 0.22f, 0.9f), bodyMat, root.transform);
        
        for (int i = 0; i < 8; i++)
        {
            float x = -0.6f + (i * 0.16f);
            CreateCube($"Port_{i}", new Vector3(x, 0.45f, 0.11f), new Vector3(0.12f, 0.02f, 0.06f), portMat, root.transform);
        }
        
        CreateCube("LED_Collision", new Vector3(0.8f, 0.46f, 0.16f), new Vector3(0.02f, 0.01f, 0.02f), ledMat, root.transform);
        return root;
    }

    static GameObject BuildPC(Material bodyMat, Material blackMat, Material pwrMat, Material screenMat)
    {
        GameObject root = new GameObject("PC");
        
        // PC Tower (Moved right)
        CreateCube("Body", new Vector3(0.5f, 0.5f, 0), new Vector3(0.6f, 1f, 1f), bodyMat, root.transform); 
        CreateCube("DVD", new Vector3(0.5f, 0.8f, 0.5f), new Vector3(0.4f, 0.02f, 0.02f), blackMat, root.transform);
        CreateCube("PowerButton", new Vector3(0.5f, 0.6f, 0.5f), new Vector3(0.03f, 0.01f, 0.03f), pwrMat, root.transform);
        
        // Attached Monitor to the left
        CreateCylinder("Base", new Vector3(-0.4f, 0.02f, 0), new Vector3(0.4f, 0.02f, 0.4f), Vector3.zero, bodyMat, root.transform);
        CreateCube("Neck", new Vector3(-0.4f, 0.2f, 0), new Vector3(0.1f, 0.4f, 0.1f), bodyMat, root.transform);
        CreateCube("ScreenFrame", new Vector3(-0.4f, 0.45f, 0), new Vector3(1.1f, 0.7f, 0.05f), bodyMat, root.transform);
        CreateCube("Display", new Vector3(-0.4f, 0.45f, -0.03f), new Vector3(1.0f, 0.6f, 0.01f), screenMat, root.transform);

        return root;
    }

    static GameObject BuildMonitor(Material bodyMat, Material screenMat)
    {
        GameObject root = new GameObject("Monitor");
        CreateCylinder("Base", new Vector3(0, 0.02f, 0), new Vector3(0.4f, 0.02f, 0.4f), Vector3.zero, bodyMat, root.transform);
        CreateCube("Neck", new Vector3(0, 0.2f, 0), new Vector3(0.1f, 0.4f, 0.1f), bodyMat, root.transform);
        CreateCube("ScreenFrame", new Vector3(0, 0.45f, 0), new Vector3(1.1f, 0.7f, 0.05f), bodyMat, root.transform);
        CreateCube("Display", new Vector3(0, 0.45f, -0.03f), new Vector3(1.0f, 0.6f, 0.01f), screenMat, root.transform);
        return root;
    }
}
