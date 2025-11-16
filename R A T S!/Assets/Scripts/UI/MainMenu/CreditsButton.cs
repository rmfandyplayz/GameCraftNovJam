using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CreditsButton : GenericMenuButton
{
    private bool isInteractable = true;

    [SerializeField] Button button;

    private Coroutine pointerEnterCoroutine;
    private Coroutine pointerExitCoroutine;

    const float scaleUpSize = 1.3f;
    const float rippleEffectDistance = 15f;
    const float animationDuration = 0.35f;

    MainMenu menuScript;

    void Start()
    {
        SetOriginalPosition();
        menuScript = FindFirstObjectByType<MainMenu>();
    }

    public override void OnClick()
    {
        throw new System.NotImplementedException();
    }

    public override void OnPointerEnter()
    {
        if (!isInteractable)
            return;

        foreach (Button b in menuScript.GetTopLevelButtons())
        {
            DOTween.Kill(b);
        }

        button.image.rectTransform.DOScale(scaleUpSize, animationDuration).SetEase(Ease.OutBack);

        foreach (Button b in menuScript.GetTopLevelButtons())
        {
            if (b.gameObject != this.gameObject)
            {
                if (b.image.rectTransform.anchoredPosition.y < button.image.rectTransform.anchoredPosition.y)
                    b.image.rectTransform.DOAnchorPosY(b.image.rectTransform.anchoredPosition.y - rippleEffectDistance, animationDuration).SetEase(Ease.OutBack);
                else
                    b.image.rectTransform.DOAnchorPosY(b.image.rectTransform.anchoredPosition.y + rippleEffectDistance, animationDuration).SetEase(Ease.OutBack);
            }
        }
    }

    public override void OnPointerExit()
    {
        if (!isInteractable)
            return;

        foreach (Button b in menuScript.GetTopLevelButtons())
        {
            DOTween.Kill(b);
        }

        button.image.rectTransform
            .DOScale(1, animationDuration)
            .SetEase(Ease.OutBack);

        GenericMenuButton[] menuButtons = FindObjectsByType<GenericMenuButton>(FindObjectsSortMode.None);

        foreach (var genBtn in menuButtons)
        {
            if (genBtn.gameObject == this.gameObject)
                continue;

            Button btn = genBtn.GetComponent<Button>();
            if (btn == null)
                continue;

            genBtn.TweenToOriginalPosition();
        }
    }

    public override void TweenToOriginalPosition()
    {
        button.image.rectTransform
            .DOAnchorPos(GetOriginalPos(), animationDuration)
            .SetEase(Ease.OutExpo);

        button.image.rectTransform.DORotate(Vector3.zero, animationDuration);
        button.image.DOFade(1, animationDuration);
    }
    public override void DisableButtonInteractions()
    {
        isInteractable = false;
        button.interactable = false;
    }

    public override void EnableButtonInteractions()
    {
        isInteractable = true;
        button.interactable = true;
    }
}
