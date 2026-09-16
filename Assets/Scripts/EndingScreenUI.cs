using System.Collections;
using UnityEngine;
using TMPro;

public class EndingScreenUI : MonoBehaviour
{
    [Header("Glitzie Counts")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text rubyText;
    [SerializeField] private TMP_Text saphireText;
    [SerializeField] private TMP_Text emeraldText;
    [SerializeField] private TMP_Text diamondText;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Highscore")]
    [SerializeField] private HighscoreManager highscoreManager;

    [Header("Highscore Input")]
    [SerializeField] private CanvasGroup nameInputGroup;
    [SerializeField] private CanvasGroup saveButtonGroup;

    [Header("Ending Buttons")]
    [SerializeField] private EndingButtonsSlide endingButtonsSlide;

    [Header("Animation")]
    [SerializeField] private float delayBetweenValues = 0.2f;
    [SerializeField] private float countDuration = 0.8f;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Input Fade")]
    [SerializeField] private float inputAppearDelay = 0.3f;
    [SerializeField] private float inputFadeDuration = 0.5f;

    [Header("Maximum Glitzies")]
    [SerializeField] private int maxGold = 10;
    [SerializeField] private int maxRuby = 10;
    [SerializeField] private int maxSaphire = 10;
    [SerializeField] private int maxEmerald = 10;
    [SerializeField] private int maxDiamond = 10;

    [Header("Completed Colors")]
    [SerializeField] private Color goldCompleteColor = new Color(1f, 0.75f, 0f);
    [SerializeField] private Color rubyCompleteColor = new Color(1f, 0.1f, 0.1f);
    [SerializeField] private Color saphireCompleteColor = new Color(0.2f, 0.4f, 1f);
    [SerializeField] private Color emeraldCompleteColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color diamondCompleteColor = new Color(0.5f, 0.9f, 1f);

    private Color goldDefaultColor;
    private Color rubyDefaultColor;
    private Color saphireDefaultColor;
    private Color emeraldDefaultColor;
    private Color diamondDefaultColor;
    private Color scoreDefaultColor;

    private void Start()
    {
        goldDefaultColor = goldText.color;
        rubyDefaultColor = rubyText.color;
        saphireDefaultColor = saphireText.color;
        emeraldDefaultColor = emeraldText.color;
        diamondDefaultColor = diamondText.color;
        scoreDefaultColor = scoreText.color;

        PrepareText(goldText);
        PrepareText(rubyText);
        PrepareText(saphireText);
        PrepareText(emeraldText);
        PrepareText(diamondText);
        PrepareText(scoreText);

        HideCanvasGroup(nameInputGroup);
        HideCanvasGroup(saveButtonGroup);

        StartCoroutine(ShowResults());
    }

    private IEnumerator ShowResults()
    {
        yield return CountUp(goldText, GameResultData.GoldCount, maxGold, goldDefaultColor, goldCompleteColor);
        yield return new WaitForSecondsRealtime(delayBetweenValues);
        
        yield return CountUp(rubyText, GameResultData.RubyCount, maxRuby, rubyDefaultColor, rubyCompleteColor);
        yield return new WaitForSecondsRealtime(delayBetweenValues);

        yield return CountUp(saphireText, GameResultData.SaphireCount, maxSaphire, saphireDefaultColor, saphireCompleteColor);
        yield return new WaitForSecondsRealtime(delayBetweenValues);

        yield return CountUp(emeraldText, GameResultData.EmeraldCount, maxEmerald, emeraldDefaultColor, emeraldCompleteColor);
        yield return new WaitForSecondsRealtime(delayBetweenValues);

        yield return CountUp(diamondText, GameResultData.DiamondCount, maxDiamond, diamondDefaultColor, diamondCompleteColor);
        yield return new WaitForSecondsRealtime(delayBetweenValues);

        yield return CountUpScore(scoreText, GameResultData.Score);
        yield return new WaitForSecondsRealtime(inputAppearDelay);

        bool reachedTop10 = false;

        if (highscoreManager)
        {
            reachedTop10 = highscoreManager.IsTop10Score(GameResultData.Score);
        }

        if (reachedTop10)
        {
            StartCoroutine(FadeInCanvasGroup(nameInputGroup));
            yield return FadeInCanvasGroup(saveButtonGroup);
        }

        if (endingButtonsSlide)
        {
            endingButtonsSlide.StartSlideAnimation();
        }
    }

    private IEnumerator CountUp(TMP_Text text, int targetValue, int maxValue, Color defaultColor, Color completeColor)
    {
        if (!text)
            yield break;

        float timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / countDuration);
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, progress));

            text.text = currentValue.ToString();

            float alpha = Mathf.Clamp01(timer / fadeDuration);

            Color currentColor = defaultColor;
            currentColor.a = alpha;
            text.color = currentColor;

            yield return null;
        }

        text.text = targetValue.ToString();

        if (targetValue >= maxValue)
        {
            completeColor.a = 1f;
            text.color = completeColor;
        }
        else
        {
            defaultColor.a = 1f;
            text.color = defaultColor;
        }
    }

    private IEnumerator CountUpScore(TMP_Text text, int targetValue)
    {
        if (!text)
            yield break;

        float timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.unscaledDeltaTime;
            
            float progress = Mathf.Clamp01(timer / countDuration);
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, progress));

            text.text = currentValue.ToString();

            float alpha = Mathf.Clamp01(timer / fadeDuration);
            Color currentColor = scoreDefaultColor;
            currentColor.a = alpha;

            text.color = currentColor;

            yield return null;
        }

        text.text = targetValue.ToString();
        scoreDefaultColor.a = 1f;
        text.color = scoreDefaultColor;
    }

    private void PrepareText(TMP_Text text)
    {
        if (!text)
            return;

        text.text = "0";
        Color color = text.color;
        color.a = 0f;
        text.color = color;
    }

    private void HideCanvasGroup(CanvasGroup group)
    {
        if (!group)
            return;

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup group)
    {
        if (!group)
            yield break;

        float timer = 0f;

        while (timer < inputFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            group.alpha = Mathf.Clamp01(timer / inputFadeDuration);
            yield return null;
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
}