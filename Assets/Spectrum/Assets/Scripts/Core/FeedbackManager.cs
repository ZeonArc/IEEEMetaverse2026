using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

// Plays audio stingers + controller haptics on key game events.
public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private AuctionManager auctions;
    [SerializeField] private ScenarioManager scenarios;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip auctionClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private HapticImpulsePlayer leftHaptics;
    [SerializeField] private HapticImpulsePlayer rightHaptics;

    void OnEnable()
    {
        auctions.OnAuctionResolved += OnAuction;
        scenarios.OnScenarioEnded += OnScenarioEnded;
    }
    void OnDisable()
    {
        auctions.OnAuctionResolved -= OnAuction;
        scenarios.OnScenarioEnded -= OnScenarioEnded;
    }

    private void OnAuction(NetworkNode node, AuctionResult r)
    {
        Play(auctionClip);
        Pulse(0.4f, 0.1f);
    }

    private void OnScenarioEnded(bool won)
    {
        Play(won ? winClip : loseClip);
        Pulse(won ? 0.8f : 0.5f, 0.3f);
    }

    private void Play(AudioClip clip) { if (clip) sfx.PlayOneShot(clip); }

    private void Pulse(float amplitude, float duration)
    {
        if (leftHaptics) leftHaptics.SendHapticImpulse(amplitude, duration);
        if (rightHaptics) rightHaptics.SendHapticImpulse(amplitude, duration);
    }
}
