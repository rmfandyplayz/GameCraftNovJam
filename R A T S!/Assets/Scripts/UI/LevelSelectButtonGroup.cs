using System.Collections;
using UnityEngine;

public class LevelSelectButtonGroup : MonoBehaviour
{
    [SerializeField] LevelSelectButton[] levelSelectButtons;


    public void AnimateOpening()
    {
        StartCoroutine(AnimateOpeningSequence());
    }

    public void AnimateClosing()
    {
        StartCoroutine(AnimateClosingSequence());
    }


    IEnumerator AnimateOpeningSequence()
    {
        foreach(LevelSelectButton button in levelSelectButtons)
        {
            button.AnimateOpening();
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator AnimateClosingSequence()
    {
        foreach(LevelSelectButton button in levelSelectButtons)
        {
            button.AnimateClosing();
            yield return new WaitForSeconds(0.05f);
        }
    }

}
