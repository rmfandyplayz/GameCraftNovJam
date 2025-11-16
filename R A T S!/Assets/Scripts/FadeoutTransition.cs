using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeoutTransition : MonoBehaviour
{

    public static string lastSceneName;
    public static int lastSceneIndex;
    public static void SceneTransition(string sceneName)
    {
        lastSceneName = SceneManager.GetActiveScene().path;
        lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GameObject prefab = Resources.Load("FadeoutTransition") as GameObject;
        GameObject instance = Instantiate(prefab);
        DontDestroyOnLoad(instance);
        instance.GetComponent<FadeoutTransition>().ChangeScene(sceneName);
    }


    public void ChangeScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        yield return new WaitForEndOfFrame();
        Animator animator = transform.GetChild(0).GetComponent<Animator>();
        animator.SetBool("FadeIn", false);
        animator.SetTrigger("Go");

        yield return new WaitForEndOfFrame();

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Decide"))
        {
            yield return null;
        }

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOp.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        animator.SetBool("FadeIn", true);
        animator.SetTrigger("Go");

        yield return new WaitForEndOfFrame();
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Decide"))
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}