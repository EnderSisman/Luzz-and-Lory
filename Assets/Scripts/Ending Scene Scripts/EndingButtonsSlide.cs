using System.Collections;
using UnityEngine;

public class EndingButtonsSlide : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private RectTransform restartSign;
    [SerializeField] private RectTransform mainMenuSign;

    [Header("Slide")]
    [SerializeField] private float startOffsetY = -300f;
    [SerializeField] private float slideDuration = 0.6f;
    [SerializeField] private float delayBetweenButtons = 0.2f;

    private Vector2 restartTargetPosition;
    private Vector2 mainMenuTargetPosition;

    private bool initialized = false;
    private bool animationStarted = false;

    private void Awake()
    {
        if (!restartSign || !mainMenuSign)
            return;

        restartTargetPosition = restartSign.anchoredPosition;        // Zielpositionen merken
        mainMenuTargetPosition = mainMenuSign.anchoredPosition;
        
        restartSign.anchoredPosition = restartTargetPosition + new Vector2(0f, startOffsetY);   // Beide Schilder nach unten setzen
        mainMenuSign.anchoredPosition = mainMenuTargetPosition + new Vector2(0f, startOffsetY);

        initialized = true;
    }

    public void StartSlideAnimation()
    {
        if (!initialized || animationStarted)
            return;

        animationStarted = true;

        StartCoroutine(SlideButtons());
    }

    private IEnumerator SlideButtons()
    {
        StartCoroutine(SlideIn(restartSign, restartTargetPosition));
        yield return new WaitForSecondsRealtime(delayBetweenButtons);
        StartCoroutine(SlideIn(mainMenuSign, mainMenuTargetPosition));
    }

    private IEnumerator SlideIn(RectTransform target, Vector2 endPosition)
    {
        Vector2 startPosition = target.anchoredPosition;

        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / slideDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            target.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedProgress);

            yield return null;
        }

        target.anchoredPosition = endPosition;
    }
}