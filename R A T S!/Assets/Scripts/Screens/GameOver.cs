using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void Retry()
    {
        FadeoutTransition.SceneTransition(FadeoutTransition.lastSceneName);
    }

    public void Menu()
    {
        FadeoutTransition.SceneTransition("MainMenu");
    }
}
