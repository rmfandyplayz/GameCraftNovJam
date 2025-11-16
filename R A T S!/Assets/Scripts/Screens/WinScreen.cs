using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public const int firstLevelIndex = 5;

    [SerializeField] private Sprite[] winScreens;
    

    private void Start()
    {
        Image image = transform.Find("BG").GetComponent<Image>();
        image.sprite = winScreens[FadeoutTransition.lastSceneIndex - firstLevelIndex];
    }

    public void NextLevel()
    {
        FadeoutTransition.SceneTransition(SceneUtility.GetScenePathByBuildIndex(FadeoutTransition.lastSceneIndex + 1));
    }

    public void ToMenu()
    {
        FadeoutTransition.SceneTransition("MainMenu");
    }
}
