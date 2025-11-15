using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEditor.Rendering;

public class BottomBar : MonoBehaviour
{
    [SerializeField] Button backButton;

    [SerializeField] GameObject levelSelectButtonGroup;
    Button[] levelSelectButtons;

    RectTransform bottomBarPos;

    Vector2 originalPos;
    [SerializeField] RectTransform targetPos;

    [SerializeField] MainMenu menuScript;

    private void Start()
    {
        backButton.interactable = false;
        levelSelectButtons = levelSelectButtonGroup.GetComponentsInChildren<Button>();
        bottomBarPos = GetComponent<RectTransform>();
        originalPos = bottomBarPos.anchoredPosition;

        Debug.Log("Original Pos: " + originalPos);
        Debug.Log("Target Pos: " + targetPos.anchoredPosition);

    }

    /// <summary>
    /// Tweens the bottom bar to be within the screen's view.
    /// </summary>
    public void ActivateBottomBar()
    {
        bottomBarPos.DOAnchorPos(targetPos.anchoredPosition, 0.5f).SetEase(Ease.OutBack)
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
        backButton.interactable = false;
        bottomBarPos.DOAnchorPos(originalPos, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            foreach(GenericMenuButton button in menuScript.GetTopLevelButtonsGenericVariant())
            {
                button.TweenToOriginalPosition();
                button.EnableButtonInteractions();
            }
        });
    }
}
