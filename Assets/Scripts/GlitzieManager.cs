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

    [Header("Maximum Glitzies")]
    [SerializeField] private int maxGold = 20;
    [SerializeField] private int maxRubin = 5;
    [SerializeField] private int maxSaphir = 5;
    [SerializeField] private int maxSmaragd = 3;
    [SerializeField] private int maxDiamant = 1;

    [Header("Completed Colors")]
    [SerializeField] private Color goldCompleteColor = new Color(1f, 0.75f, 0f);
    [SerializeField] private Color rubinCompleteColor = new Color(1f, 0.1f, 0.1f);
    [SerializeField] private Color saphirCompleteColor = new Color(0.2f, 0.4f, 1f);
    [SerializeField] private Color smaragdCompleteColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color diamantCompleteColor = new Color(0.5f, 0.9f, 1f);

    private Color goldDefaultColor;
    private Color rubinDefaultColor;
    private Color saphirDefaultColor;
    private Color smaragdDefaultColor;
    private Color diamantDefaultColor;

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
        if (goldText)
            goldDefaultColor = goldText.color;

        if (rubinText)
            rubinDefaultColor = rubinText.color;

        if (saphirText)
            saphirDefaultColor = saphirText.color;

        if (smaragdText)
            smaragdDefaultColor = smaragdText.color;

        if (diamantText)
            diamantDefaultColor = diamantText.color;

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
        {
            goldText.text = goldCount.ToString();
            goldText.color = goldCount >= maxGold ? goldCompleteColor : goldDefaultColor;
        }

        if (rubinText)
        {
            rubinText.text = rubinCount.ToString();
            rubinText.color = rubinCount >= maxRubin ? rubinCompleteColor : rubinDefaultColor;
        }

        if (saphirText)
        {
            saphirText.text = saphirCount.ToString();
            saphirText.color = saphirCount >= maxSaphir ? saphirCompleteColor : saphirDefaultColor;
        }

        if (smaragdText)
        {
            smaragdText.text = smaragdCount.ToString();
            smaragdText.color = smaragdCount >= maxSmaragd ? smaragdCompleteColor : smaragdDefaultColor;
        }

        if (diamantText)
        {
            diamantText.text = diamantCount.ToString();
            diamantText.color = diamantCount >= maxDiamant ? diamantCompleteColor : diamantDefaultColor;
        }
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