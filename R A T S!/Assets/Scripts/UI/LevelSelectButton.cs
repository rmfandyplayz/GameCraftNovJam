using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] string targetScene;

    private bool isInteractable = true;

    [SerializeField] Button button;

    const float scaleUpSize = 1.15f;
    const float rippleEffectDistance = 25f;
    const float animationDuration = 0.35f;
    

    public void OnPointerEnter()
    {
        if (!isInteractable)
            return;

        DOTween.Kill(button);

        button.image.rectTransform.DOScale(scaleUpSize, animationDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit()
    {
        if (!isInteractable)
            return;

        DOTween.Kill(button);

        button.image.rectTransform.DOScale(1, animationDuration).SetEase(Ease.OutBack);
    }

    public void OnClick()
    {
        
    }



    public void DisableButtonInteractions()
    {
        isInteractable = false;
        button.interactable = false;
    }

    public void EnableButtonInteractions()
    {
        isInteractable = true;
        button.interactable = true;
    }
}
