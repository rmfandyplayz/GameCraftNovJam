using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public void NextLevel()
    {
        FadeoutTransition.SceneTransition(SceneUtility.GetScenePathByBuildIndex(FadeoutTransition.lastSceneIndex + 1));
    }

    public void ToMenu()
    {
        FadeoutTransition.SceneTransition("MainMenu");
    }
}
