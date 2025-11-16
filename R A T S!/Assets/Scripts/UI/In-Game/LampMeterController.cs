using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class LampMeterController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lampMeterText;

    [SerializeField] Image lampSprite;
    [SerializeField] Sprite fullLampSprite;
    [SerializeField] Sprite threeQuarterLampSprite;
    [SerializeField] Sprite halfLampSprite;
    [SerializeField] Sprite oneQuarterLampSprite;
    [SerializeField] Sprite emptyLampSprite;

    private float lampAmount = 100f;

    private const float animDuration = 0.25f;

    private void Start()
    {
        lampSprite.sprite = fullLampSprite;
    }

    public void SetLampMeter(float newAmount)
    {
        float previousAmount = lampAmount;

        lampAmount = newAmount;

        if (lampAmount > 75)
            lampSprite.sprite = fullLampSprite;
        else if (lampAmount > 50)
            lampSprite.sprite = threeQuarterLampSprite;
        else if (lampAmount > 25)
            lampSprite.sprite = halfLampSprite;
        else if (lampAmount > 0)
            lampSprite.sprite = oneQuarterLampSprite;
        else if(lampAmount <= 0)
            lampSprite.sprite = emptyLampSprite;

        DOTween.Kill(lampMeterText, true);
        
        DOTween.To(
            () => previousAmount,
            value => lampMeterText.text = $"{value.ToString("0.0")}%", // 1 decimal place
            lampAmount,
            animDuration
        );
    }
}
