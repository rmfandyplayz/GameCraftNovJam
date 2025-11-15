using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LevelSelectButton : GenericMenuButton
{
    [SerializeField] string targetScene;

    private bool isInteractable = true;

    [SerializeField] Button button;

    const float scaleUpSize = 1.15f;
    const float rippleEffectDistance = 25f;
    const float animationDuration = 0.35f;
    

    public override void OnPointerEnter()
    {
        if (!isInteractable)
            return;

        DOTween.Kill(button);

        button.image.rectTransform.DOScale(scaleUpSize, animationDuration).SetEase(Ease.OutBack);
    }

    public override void OnPointerExit()
    {
        if (!isInteractable)
            return;

        DOTween.Kill(button);

        button.image.rectTransform.DOScale(1, animationDuration).SetEase(Ease.OutBack);
    }

    public override void OnClick()
    {
        
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
