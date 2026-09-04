using UnityEngine;
using TMPro;

public class GlitzieManager : MonoBehaviour
{
    public static GlitzieManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text rubinText;
    [SerializeField] private TMP_Text saphirText;
    [SerializeField] private TMP_Text smaragdText;
    [SerializeField] private TMP_Text diamantText;

    private int goldCount;
    private int rubinCount;
    private int saphirCount;
    private int smaragdCount;
    private int diamantCount;

    // Wird intern gezählt, aber nicht im Gameplay angezeigt
    private int score;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddGlitzie(Glitzie.GlitzieType type, int points)
    {
        switch (type)
        {
            case Glitzie.GlitzieType.Gold:
                goldCount++;
                break;

            case Glitzie.GlitzieType.Ruby:
                rubinCount++;
                break;

            case Glitzie.GlitzieType.Saphire:
                saphirCount++;
                break;

            case Glitzie.GlitzieType.Emerald:
                smaragdCount++;
                break;

            case Glitzie.GlitzieType.Diamond:
                diamantCount++;
                break;
        }

        score += points;

        UpdateUI();

        Debug.Log("Aktueller Score: " + score);
    }

    private void UpdateUI()
    {
        if (goldText)
            goldText.text = "x" + goldCount;

        if (rubinText)
            rubinText.text = "x" + rubinCount;

        if (saphirText)
            saphirText.text = "x" + saphirCount;

        if (smaragdText)
            smaragdText.text = "x" + smaragdCount;

        if (diamantText)
            diamantText.text = "x" + diamantCount;
    }

    public int GetScore()
    {
        return score;
    }
}