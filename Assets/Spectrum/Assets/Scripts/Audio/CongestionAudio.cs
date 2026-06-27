using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CongestionAudio : MonoBehaviour
{
    [SerializeField] private NetworkNode node;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2.0f;

    private AudioSource _audio;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.loop = true;
        _audio.spatialBlend = 1f;
        _audio.Play();
    }

    void Update()
    {
        float ratio = node.CongestionRatio;
        _audio.volume = Mathf.Lerp(0f, 0.6f, ratio);
        _audio.pitch = Mathf.Lerp(minPitch, maxPitch, ratio);
    }
}
