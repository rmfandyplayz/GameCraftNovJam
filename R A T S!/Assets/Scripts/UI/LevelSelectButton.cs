using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] string targetScene;

    private bool isInteractable = true;

    [SerializeField] Button button;

    const float unAnimatedSize = 0.75f;
    const float scaleUpSize = 1.15f;
    const float animationDuration = 0.2f;
    
    void Start()
    {
        DisableButtonInteractions();
        button.image.DOFade(0, 0);
        button.image.rectTransform.DOScale(0.8f, 0);
    }

    public void AnimateOpening()
    {
        button.image.DOFade(0, 0);
        button.image.rectTransform.DOScale(unAnimatedSize, 0);

        button.image.DOFade(1, animationDuration);
        button.image.rectTransform.DOScale(1, animationDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            EnableButtonInteractions();
        });
    }

    public void AnimateClosing()
    {
        DOTween.Kill(button);

        button.image.DOFade(0, animationDuration);
        button.image.rectTransform.DOScale(unAnimatedSize, animationDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            DisableButtonInteractions();
        });
    }

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
        if(!isInteractable)
            return;

        Debug.Log("dfdf");

        DOTween.Kill(button);

        foreach (MonoBehaviour type in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (type is GenericMenuButton genericButton)
            {
                genericButton.DisableButtonInteractions();
            }
        }

        StartCoroutine(LevelSelectButtonAnimationSequence());
    }

    IEnumerator LevelSelectButtonAnimationSequence()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(button.image.rectTransform.DOScale(scaleUpSize + .2f, animationDuration / 3).SetEase(Ease.OutQuint));
        sequence.Append(button.image.rectTransform.DOScale(1, animationDuration).SetEase(Ease.OutQuad));

        yield return new WaitForSeconds(animationDuration);

        //TODO scene transition?

        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
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
