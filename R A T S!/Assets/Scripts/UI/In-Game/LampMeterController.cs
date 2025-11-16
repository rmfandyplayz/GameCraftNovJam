using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class LampMeterController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lampMeterText;

    [SerializeField] Image lampSprite;
    [SerializeField] Image fullLampSprite;
    [SerializeField] Image threeQuarterLampSprite;
    [SerializeField] Image halfLampSprite;
    [SerializeField] Image oneQuarterLampSprite;
    [SerializeField] Image emptyLampSprite;

    private float lampAmount = 100f;

    private const float animDuration = 0.35f;

    private void Start()
    {
        lampSprite = fullLampSprite;
    }

    public void DecreaseLampMeter(float decreaseAmount)
    {
        float previousAmount = lampAmount;

        lampAmount -= decreaseAmount;
        lampAmount = Mathf.Clamp(lampAmount, 0f, 100f);

        if (lampAmount > 75)
            lampSprite = fullLampSprite;
        else if (lampAmount > 50)
            lampSprite = threeQuarterLampSprite;
        else if (lampAmount > 25)
            lampSprite = halfLampSprite;
        else if (lampAmount > 0)
            lampSprite = oneQuarterLampSprite;
        else if(lampAmount <= 0)
            lampSprite = emptyLampSprite;

        DOTween.Sequence().Kill(lampMeterText);

        var sequence = DOTween.Sequence();

        sequence.Join(
            DOTween.To(
                () => previousAmount,
                value => lampMeterText.text = value.ToString("0.0"), // 1 decimal place
                lampAmount,
                animDuration
            )
        );

        sequence.Append(lampMeterText.DOColor(new Color(237f / 255f, 21f / 255f, 57f / 255f), animDuration));
        sequence.OnComplete(() =>
        {
            lampMeterText.DOColor(Color.white, 0.2f);
        });
    }
}
