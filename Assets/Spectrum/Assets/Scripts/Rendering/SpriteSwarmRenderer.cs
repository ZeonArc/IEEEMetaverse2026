using UnityEngine;

public class SpriteSwarmRenderer : MonoBehaviour
{
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Material spriteMaterial;
    [SerializeField] private int particlesPerNode = 32;
    [SerializeField] private float swarmRadius = 0.15f;

    private NetworkGraph _graph;
    private Matrix4x4[] _matrices;
    private MaterialPropertyBlock _props;

    void Start()
    {
        _graph = GetComponent<NetworkGraph>();
        _matrices = new Matrix4x4[particlesPerNode];
        _props = new MaterialPropertyBlock();
    }

    void Update()
    {
        foreach (var node in _graph.Nodes)
        {
            float jitter = node.CongestionRatio * 0.05f;

            for (int i = 0; i < particlesPerNode; i++)
            {
                Vector3 offset = Random.insideUnitSphere * swarmRadius;
                offset += Random.insideUnitSphere * jitter;
                Vector3 pos = node.transform.position + offset;
                _matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * 0.02f);
            }

            Color tint = Color.Lerp(Color.cyan, Color.red, node.CongestionRatio);
            _props.SetColor("_BaseColor", tint);
            Graphics.DrawMeshInstanced(quadMesh, 0, spriteMaterial, _matrices, particlesPerNode, _props);
        }
    }
}
