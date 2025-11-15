using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    public Light2D light2D;

    [Header("FallOff")]
    public float startFalloff = 10.3f;
    public float endFalloff = 2f;
    public float duration = 5f;

    private float timer = 0f;

    private void Awake()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        // Set starting value
        light2D.shapeLightFalloffSize = startFalloff;
    }

    private void Update()
    {
        if (timer >= duration) return;

        timer += Time.deltaTime;
        float t = timer / duration;

        // Lerp from start to end
        float newFalloff = Mathf.Lerp(startFalloff, endFalloff, t);
        light2D.shapeLightFalloffSize = newFalloff;
    }
}
