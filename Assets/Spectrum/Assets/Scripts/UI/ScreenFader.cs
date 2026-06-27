using System.Collections;
using UnityEngine;

// Fades a full-screen overlay through black on scenario load (VR comfort + transition juice).
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarios;
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private float fadeDuration = 0.4f;

    void OnEnable() => scenarios.OnScenarioStarted += FadeThrough;
    void OnDisable() => scenarios.OnScenarioStarted -= FadeThrough;

    private void FadeThrough(Scenario _) => StartCoroutine(FadeRoutine());

    private IEnumerator FadeRoutine()
    {
        yield return Fade(0f, 1f);
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // works even when paused
            overlay.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        overlay.alpha = to;
    }
}
