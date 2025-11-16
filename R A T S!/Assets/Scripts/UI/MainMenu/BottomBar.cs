using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEditor.Rendering;

public class BottomBar : MonoBehaviour
{
    private AudioSource[] sounds;
    [SerializeField] Button backButton;

    [SerializeField] LevelSelectButtonGroup levelSelectButtonGroup;

    RectTransform bottomBarPos;

    Vector2 originalPos;
    [SerializeField] RectTransform targetPos;

    [SerializeField] MainMenu menuScript;

    private void Start()
    {
        backButton.interactable = false;
        bottomBarPos = GetComponent<RectTransform>();
        originalPos = bottomBarPos.anchoredPosition;
        sounds = backButton.GetComponentsInChildren<AudioSource>();
    }

    /// <summary>
    /// Tweens the bottom bar to be within the screen's view.
    /// </summary>
    public void ActivateBottomBar()
    {
        levelSelectButtonGroup.gameObject.SetActive(true); 
        levelSelectButtonGroup.AnimateOpening();
        bottomBarPos.DOAnchorPos(targetPos.anchoredPosition, 0.75f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                backButton.interactable = true;
            });
    }

    /// <summary>
    /// Tweens the bottom bar out of the screen's view.
    /// </summary>
    public void DeactivateBottomBar()
    {
        sounds[0].Play();
        backButton.interactable = false;
        levelSelectButtonGroup.AnimateClosing();
        bottomBarPos.DOAnchorPos(originalPos, 0.75f).SetEase(Ease.InBack).OnComplete(() =>
        {
            levelSelectButtonGroup.gameObject.SetActive(false);
            foreach(GenericMenuButton button in menuScript.GetTopLevelButtonsGenericVariant())
            {
                button.TweenToOriginalPosition();
                button.EnableButtonInteractions();
            }
        });
    }
}
