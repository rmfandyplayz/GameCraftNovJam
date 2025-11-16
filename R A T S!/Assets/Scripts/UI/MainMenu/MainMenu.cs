using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Various utility functions for the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] Button[] topLevelButtons;

    public Button[] GetTopLevelButtons()
    {
        return topLevelButtons;
    }

    public GenericMenuButton[] GetTopLevelButtonsGenericVariant()
    {
        GenericMenuButton[] genericButtons = new GenericMenuButton[topLevelButtons.Length];

        for (int i = 0; i < topLevelButtons.Length; i++)
        {
            genericButtons[i] = topLevelButtons[i].GetComponent<GenericMenuButton>();
        }

        return genericButtons;
    }
}
