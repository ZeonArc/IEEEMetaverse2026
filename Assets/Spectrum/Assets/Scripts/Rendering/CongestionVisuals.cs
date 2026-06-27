using UnityEngine;

public class CongestionVisuals : MonoBehaviour
{
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private ParticleSystem warningParticles;

    void Update()
    {
        foreach (var node in graph.Nodes)
        {
            if (node.CongestionRatio > 0.9f && !warningParticles.isPlaying)
            {
                warningParticles.transform.position = node.transform.position;
                warningParticles.Play();
            }
        }
    }
}
