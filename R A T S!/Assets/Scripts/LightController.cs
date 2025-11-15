using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    public Light2D light2D;

    [Header("FallOff")]
    public float startFalloff = 10.3f;
    public float endFalloff = 2f;
    public float duration = 5f;

    [Header("Instant Change Settings")]
    public float hitDecrease = 8f;                // how much light to take away when hit
    public float lightIncreaseUpgradeAmount = 12f; // how much light to add on replenish
    public float minFalloff = 1.5f;
    public float maxFalloff = 35f;

    // Hit pause to allow overrides to not happen
    private float hitPauseDuration = 0.1f; // pause fade for an instant in order to have fade not override!

    private float fadeSpeed;
    private float pauseTimer = 0f;

    private void Awake()
    {
        // Light setup
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        light2D.shapeLightFalloffSize = startFalloff;

        if (duration > 0f)
            fadeSpeed = Mathf.Abs(startFalloff - endFalloff) / duration;
        else
            fadeSpeed = 0f;
    }

    private void Update()
    {
        // for an instant stop fading when hit to allow for sudden changes
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        float current = light2D.shapeLightFalloffSize;
        float target = endFalloff;

        if (fadeSpeed > 0f && Mathf.Abs(current - target) > 0.001f)
        {
            float newValue = Mathf.MoveTowards(current, target, fadeSpeed * Time.deltaTime);
            light2D.shapeLightFalloffSize = Mathf.Clamp(newValue, minFalloff, maxFalloff);
        }
    }

    // Call this when hit by enemy
    public void DecreaseFalloff()
    {
        float current = light2D.shapeLightFalloffSize;
        float newValue = current - hitDecrease;
        light2D.shapeLightFalloffSize = Mathf.Clamp(newValue, minFalloff, maxFalloff);

        // Briefly pause the fade so the "step" is noticeable
        pauseTimer = hitPauseDuration;
    }

    // Call this when you want to replenish / upgrade the light
    public void IncreaseFalloff()
    {
        float current = light2D.shapeLightFalloffSize;
        float newValue = current + lightIncreaseUpgradeAmount;
        light2D.shapeLightFalloffSize = Mathf.Clamp(newValue, minFalloff, maxFalloff);

        // Optional: also pause on increase, if you want the bump to pop visually
        // pauseTimer = hitPauseDuration;
    }
}
