using System.Collections;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    [SerializeField] private GameObject exitGame;
    [SerializeField] private CanvasGroup bankRoomUI;
    [SerializeField] private CanvasGroup sheriffRoomUI;
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup exitCanvasGroup;
    private Coroutine fadeCoroutine;
    private bool isVisible = true;

    private void Awake()
    {
        exitCanvasGroup = exitGame.GetComponent<CanvasGroup>();

        if (!exitCanvasGroup)
            exitCanvasGroup = exitGame.AddComponent<CanvasGroup>();

        exitCanvasGroup.alpha = 1f;
        exitCanvasGroup.interactable = true;
        exitCanvasGroup.blocksRaycasts = true;
    }

    private void Update()
    {
        bool bankVisible = bankRoomUI && bankRoomUI.alpha > 0.01f;
        bool sheriffVisible = sheriffRoomUI && sheriffRoomUI.alpha > 0.01f;
        bool shouldBeVisible = !bankVisible && !sheriffVisible;

        if (shouldBeVisible != isVisible)
        {
            isVisible = shouldBeVisible;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeExitButton(shouldBeVisible));
        }
    }

    private IEnumerator FadeExitButton(bool show)
    {
        float startAlpha = exitCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;
        float time = 0f;

        if (show)
        {
            exitCanvasGroup.interactable = true;
            exitCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            exitCanvasGroup.interactable = false;
            exitCanvasGroup.blocksRaycasts = false;
        }

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);

            exitCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        exitCanvasGroup.alpha = targetAlpha;

        fadeCoroutine = null;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}