using UnityEngine;

/// <summary>
/// An abstract script containing functions all menu buttons share.
/// </summary>
public abstract class GenericMenuButton : MonoBehaviour
{
    Vector2 originalPos;

    /// <summary>
    /// Disable this button from being interacted with in any way.
    /// </summary>
    public abstract void DisableButtonInteractions();

    /// <summary>
    /// Allows this button to be interacted with.
    /// </summary>
    public abstract void EnableButtonInteractions();

    /// <summary>
    /// Called when the pointer enters this button's area.
    /// </summary>
    public abstract void OnPointerEnter();

    /// <summary>
    /// Called when the pointer exits this button's area.
    /// </summary>
    public abstract void OnPointerExit();

    /// <summary>
    /// Called when this button is clicked.
    /// </summary>
    public abstract void OnClick();

    /// <summary>
    /// Returns the original anchored position of this button
    /// </summary>
    public virtual Vector2 GetOriginalPos()
    {
        return originalPos;
    }

    public virtual void SetOriginalPosition()
    {
        originalPos = GetComponent<RectTransform>().anchoredPosition;
    }

    /// <summary>
    /// An optional implementable function to make the button go back to its original position
    /// </summary>
    public virtual void TweenToOriginalPosition() { }


}
