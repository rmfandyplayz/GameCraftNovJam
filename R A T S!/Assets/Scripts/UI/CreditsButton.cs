using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CreditsButton : GenericMenuButton
{
    private bool isInteractable = true;

    [SerializeField] Button button;

    private Coroutine pointerEnterCoroutine;
    private Coroutine pointerExitCoroutine;

    [SerializeField] const float scaleUpSize = 1.3f;
    [SerializeField] const float rippleEffectDistance = 15f;
    [SerializeField] const float animationDuration = 0.35f;

    GenericMenuButton[] menuButtons;

    void Start()
    {
        SetOriginalPosition();
        SetTargetPos(rippleEffectDistance);
        menuButtons = FindObjectsByType<GenericMenuButton>(FindObjectsSortMode.None);
    }

    public override void OnClick()
    {
        throw new System.NotImplementedException();
    }

    public override void OnPointerEnter()
    {
        if (!isInteractable)
            return;

        foreach (Button b in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            DOTween.Kill(b);
        }

        button.image.rectTransform.DOScale(new Vector2(scaleUpSize, scaleUpSize), animationDuration).SetEase(Ease.OutBack);

        foreach (Button b in FindObjectsByType<Button>(FindObjectsSortMode.None))
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
        foreach (Button b in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            DOTween.Kill(b);
        }

        button.image.rectTransform
            .DOScale(Vector2.one, animationDuration)
            .SetEase(Ease.OutBack);

        GenericMenuButton[] menuButtons = FindObjectsByType<GenericMenuButton>(FindObjectsSortMode.None);

        foreach (var genBtn in menuButtons)
        {
            if (genBtn.gameObject == this.gameObject)
                continue;

            Button btn = genBtn.GetComponent<Button>();
            if (btn == null)
                continue;

            btn.image.rectTransform
                .DOAnchorPosY(genBtn.GetOriginalPos().y, animationDuration)
                .SetEase(Ease.OutBack);
        }
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
