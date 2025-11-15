using UnityEngine;

/// <summary>
/// An abstract script containing functions all menu buttons share.
/// </summary>
public abstract class GenericMenuButton : MonoBehaviour
{
    Vector2 originalPos;
    Vector2 targetPos;

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
    /// Sets the target position of this button based on the ripple effect distance
    /// </summary>
    /// <param name="rippleEffectDistance"></param>
    public void SetTargetPos(float rippleEffectDistance)
    {
        targetPos.y = originalPos.y - rippleEffectDistance;
    }

    /// <summary>
    /// Gets the target position of this button based on the ripple effect distance
    /// </summary>
    public virtual Vector2 GetTargetPos()
    {
        return targetPos;
    }
}
