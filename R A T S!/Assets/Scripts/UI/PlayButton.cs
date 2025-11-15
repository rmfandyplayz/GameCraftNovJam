using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class PlayButton : GenericMenuButton
{
    private bool isInteractable = true;

    [SerializeField] Button button;

    private Coroutine playButtonCoroutine;

    const float scaleUpSize = 1.15f;
    const float rippleEffectDistance = 15f;
    const float animationDuration = 0.35f;

    GenericMenuButton[] menuButtons; 

    void Start()
    {
        SetOriginalPosition();
        menuButtons = FindObjectsByType<GenericMenuButton>(FindObjectsSortMode.None);
    }

    public override void OnClick()
    {
        if (!isInteractable)
            return;

        foreach(MonoBehaviour type in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if(type is GenericMenuButton genericButton)
            {
                genericButton.DisableButtonInteractions();
            }
        }

        DOTween.Sequence().Append(button.image.rectTransform.DOScale(scaleUpSize + .2f, animationDuration / 3).SetEase(Ease.OutQuint))
            .Append(button.image.rectTransform.DOScale(1, animationDuration).SetEase(Ease.OutQuad));


        playButtonCoroutine = StartCoroutine(PlayButtonAnimationSequence());
    }

    public override void OnPointerEnter()
    {
        if (!isInteractable)
            return;

        foreach (Button b in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            DOTween.Kill(b);
        }

        button.image.rectTransform.DOScale(scaleUpSize, animationDuration).SetEase(Ease.OutBack);

        foreach (Button b in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (b.gameObject != this.gameObject)
            {
                if (b.image.rectTransform.anchoredPosition.y < button.image.rectTransform.anchoredPosition.y)
                    b.image.rectTransform.DOAnchorPosY(b.image.rectTransform.anchoredPosition.y - rippleEffectDistance, animationDuration).SetEase(Ease.OutBack);
                else
                    b.image.rectTransform.DOAnchorPosY(b.image.rectTransform.anchoredPosition.y + rippleEffectDistance, animationDuration).SetEase(Ease.OutBack);
            }
        }
    }

    public override void OnPointerExit()
    {
        if(!isInteractable) 
            return;

        foreach (Button b in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            DOTween.Kill(b);
        }

        button.image.rectTransform
            .DOScale(Vector2.one, animationDuration)
            .SetEase(Ease.OutBack);

        GenericMenuButton[] menuButtons = FindObjectsByType<GenericMenuButton>(FindObjectsSortMode.None);

        foreach (var genBtn in menuButtons)
        {
            if (genBtn.gameObject == this.gameObject)
                continue;

            Button btn = genBtn.GetComponent<Button>();
            if (btn == null)
                continue;

            genBtn.TweenToOriginalPosition();
        }
    }


    IEnumerator PlayButtonAnimationSequence()
    {
        yield return new WaitForSeconds(0.1f);

        // make all buttons explode around the screen
        foreach (var genBtn in menuButtons)
        {
            RectTransform rt = genBtn.GetComponent<Button>().image.rectTransform;

            // random direction
            Vector2 dir = new Vector2(
                    Random.Range(-1f, 1f),   // any horizontal direction
                    Random.Range(-1f, -0.3f) // always downward
                ).normalized;


            float dist = Random.Range(750f, 850f); // tweak for canvas scale
            Vector2 target = rt.anchoredPosition + dir * dist;

            float jumpPower = Random.Range(150f, 300f);
            float duration = Random.Range(0.7f, 1.1f);

            // jump out
            rt.DOJumpAnchorPos(target, jumpPower, 1, duration)
                .SetEase(Ease.OutQuad);

            // random spin
            float z = Random.Range(-160f, 160f);
            rt.DORotate(new Vector3(0, 0, z), duration, RotateMode.FastBeyond360);

            // fade out
            genBtn.GetComponent<Button>().image.DOFade(0, duration);
        }






        //TODO DELETE -- testing 
        //yield return new WaitForSeconds(2f);

        //foreach (var genBtn in menuButtons)
        //{
        //    genBtn.TweenToOriginalPosition();
        //    genBtn.EnableButtonInteractions();
        //}
    }
   

    public override void TweenToOriginalPosition()
    {
        button.image.rectTransform
            .DOAnchorPos(GetOriginalPos(), animationDuration)
            .SetEase(Ease.OutBack);

        button.image.rectTransform.DORotate(Vector3.zero, animationDuration);
        button.image.DOFade(1, animationDuration);
    }

    public override void DisableButtonInteractions()
    {
        isInteractable = false;
        button.interactable = false;
    }
    public override void EnableButtonInteractions()
    {
        isInteractable = true;
        button.interactable = true;
    }
}
