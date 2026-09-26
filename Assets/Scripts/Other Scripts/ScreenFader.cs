using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.7f;

    private bool isFading;

    public bool IsFading => isFading;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator Start()
    {
        isFading = true;
        yield return null;
        yield return Fade(1f, 0f);

        canvasGroup.blocksRaycasts = false;
        isFading = false;
    }

    public void FadeToScene(string sceneName)
    {
        if (isFading)
            return;

        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;
        canvasGroup.blocksRaycasts = true;
        yield return Fade(canvasGroup.alpha, 1f);

        canvasGroup.alpha = 1f;
        yield return new WaitForEndOfFrame();

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);

        while (!loading.isDone)
        {
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Canvas.ForceUpdateCanvases();

        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return Fade(1f, 0f);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        isFading = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        canvasGroup.alpha = startAlpha;

        double startTime = Time.realtimeSinceStartupAsDouble;

        while (true)
        {
            double elapsed = Time.realtimeSinceStartupAsDouble - startTime;
            float progress = Mathf.Clamp01((float)(elapsed / fadeDuration));

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            if (progress >= 1f)
                break;

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}