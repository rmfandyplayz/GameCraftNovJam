using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private Light2D light2D;

    [Header("FallOff")]
    public float startFalloff = 10.3f;
    public float endFalloff = 2f;

    [HideInInspector] public float lightAmount;


    private void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Awake()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        // Set starting value
        light2D.shapeLightFalloffSize = startFalloff;
    }

    private void Update()
    {
        // Lerp from start to end
        float newFalloff = Mathf.Lerp(startFalloff, endFalloff, lightAmount);
        light2D.shapeLightFalloffSize = newFalloff;
    }
}
