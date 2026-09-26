using System.Collections;
using UnityEngine;

public class RoundStartPopup : MonoBehaviour
{
    [SerializeField] private float startDelay = 0.3f;
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float wobbleDuration = 0.12f;
    [SerializeField] private float displayDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.5f;
    
    [SerializeField] private float overshootScale = 1.2f;
    [SerializeField] private float undershootScale = 0.9f;

    private Vector3 normalScale;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        normalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();

        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 1f;
    }

    private void Start()
    {
        StartCoroutine(PlayPopup());
    }

    private IEnumerator PlayPopup()
    {
        yield return new WaitForSeconds(startDelay);

        yield return ScaleTo(normalScale * overshootScale, popDuration);
        yield return ScaleTo(normalScale * undershootScale, wobbleDuration);
        yield return ScaleTo(normalScale * 1.05f, wobbleDuration);
        yield return ScaleTo(normalScale, wobbleDuration);

        yield return new WaitForSeconds(displayDuration);

        yield return FadeOut();

        gameObject.SetActive(false);
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, t);

            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}