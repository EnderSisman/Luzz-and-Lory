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
            goldText.text = goldCount.ToString();

        if (rubinText)
            rubinText.text = rubinCount.ToString();

        if (saphirText)
            saphirText.text = saphirCount.ToString();

        if (smaragdText)
            smaragdText.text = smaragdCount.ToString();

        if (diamantText)
            diamantText.text = diamantCount.ToString();
    }

    public void SaveResults()
    {
        GameResultData.GoldCount = goldCount;
        GameResultData.RubyCount = rubinCount;
        GameResultData.SaphireCount = saphirCount;
        GameResultData.EmeraldCount = smaragdCount;
        GameResultData.DiamondCount = diamantCount;

        GameResultData.Score = score;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetGoldCount()
    {
        return goldCount;
    }

    public int GetRubinCount()
    {
        return rubinCount;
    }

    public int GetSaphirCount()
    {
        return saphirCount;
    }

    public int GetSmaragdCount()
    {
        return smaragdCount;
    }

    public int GetDiamantCount()
    {
        return diamantCount;
    }
}