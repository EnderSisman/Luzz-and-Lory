using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    [Serializable]
    public class HighscoreEntry
    {
        public string playerName;
        public int score;
        public long entryOrder;

        public HighscoreEntry(
            string playerName,
            int score,
            long entryOrder
        )
        {
            this.playerName = playerName;
            this.score = score;
            this.entryOrder = entryOrder;
        }
    }

    [Serializable]
    public class HighscoreData
    {
        public List<HighscoreEntry> entries =
            new List<HighscoreEntry>();
    }

    [Header("Name Input")]
    [SerializeField] private TMP_InputField nameInput;

    [Header("Save Button")]
    [SerializeField] private GameObject saveButton;

    [Header("Highscore Names")]
    [SerializeField] private TMP_Text[] nameTexts;

    [Header("Highscore Points")]
    [SerializeField] private TMP_Text[] scoreTexts;

    [Header("Settings")]
    [SerializeField] private int maxEntries = 10;

    private const string HighscoreKey = "LuzzAndLoryHighscores";
    private const string EntryOrderKey = "LuzzAndLoryHighscoreEntryOrder";

    private HighscoreData highscoreData;

    private bool currentScoreSaved = false;

    private void Awake()
    {
        LoadHighscores();
    }

    private void Start()
    {
        UpdateHighscoreUI();
    }

    public bool IsTop10Score(int score)
    {
        if (highscoreData == null)
        {
            LoadHighscores();
        }

        if (highscoreData.entries.Count < maxEntries)
        {
            return true;
        }

        SortHighscores();
        
        int lowestTopScore = highscoreData.entries[maxEntries - 1].score;

        return score >= lowestTopScore;
    }

    public void SaveCurrentScore()
    {
        if (currentScoreSaved)
            return;

        if (!nameInput)
            return;

        string playerName = nameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Spieler";
        }

        int currentScore = GameResultData.Score;

        if (!IsTop10Score(currentScore))
            return;

        AddHighscore(playerName, currentScore);

        currentScoreSaved = true;

        nameInput.gameObject.SetActive(false);

        if (saveButton)
        {
            saveButton.SetActive(false);
        }
    }

    private void AddHighscore(string playerName, int score)
    {
        int newOrder = PlayerPrefs.GetInt(EntryOrderKey, 0) + 1;

        PlayerPrefs.SetInt(EntryOrderKey, newOrder);
        HighscoreEntry newEntry = new HighscoreEntry(playerName, score, newOrder);

        highscoreData.entries.Add(newEntry);

        SortHighscores();

        if (highscoreData.entries.Count > maxEntries)
        {
            highscoreData.entries.RemoveRange(maxEntries, highscoreData.entries.Count - maxEntries);
        }

        SaveHighscores();
        UpdateHighscoreUI();
    }

    private void SortHighscores()
    {
        highscoreData?.entries.Sort((a, b) =>
            {
                int scoreCompare = b.score.CompareTo(a.score);
                
                if (scoreCompare != 0)
                {
                    return scoreCompare;
                }

                return b.entryOrder.CompareTo(a.entryOrder);          // Bei gleicher Punktzahl: neuerer Eintrag zuerst
            }
        );
    }

    private void LoadHighscores()
    {
        if (PlayerPrefs.HasKey(HighscoreKey))
        {
            string json = PlayerPrefs.GetString(HighscoreKey);

            highscoreData = JsonUtility.FromJson<HighscoreData>(json);
        }

        if (highscoreData == null)
        {
            highscoreData = new HighscoreData();
        }

        SortHighscores();
    }

    private void SaveHighscores()
    {
        string json = JsonUtility.ToJson(highscoreData);
        PlayerPrefs.SetString(HighscoreKey, json);
        PlayerPrefs.Save();
    }

    private void UpdateHighscoreUI()
    {
        if (highscoreData == null)
            return;

        int amount = Mathf.Min(maxEntries, nameTexts.Length, scoreTexts.Length);

        for (int i = 0; i < amount; i++)
        {
            if (i < highscoreData.entries.Count)
            {
                nameTexts[i].text = highscoreData.entries[i].playerName;
                scoreTexts[i].text = highscoreData.entries[i].score.ToString();
            }
            else
            {
                nameTexts[i].text = "-";
                scoreTexts[i].text = "-";
            }
        }
    }

    public void ResetHighscores()
    {
        PlayerPrefs.DeleteKey(HighscoreKey);
        PlayerPrefs.DeleteKey(EntryOrderKey);
        PlayerPrefs.Save();

        highscoreData = new HighscoreData();
        currentScoreSaved = false;

        UpdateHighscoreUI();
    }
}