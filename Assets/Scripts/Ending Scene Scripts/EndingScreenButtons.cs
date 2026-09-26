using UnityEngine;

public class EndingScreenButtons : MonoBehaviour
{
    public void RestartLevel()
    {
        GameResultData.Reset();

        if (!ScreenFader.Instance)
        {
            Debug.LogError("Kein ScreenFader gefunden!");
            return;
        }

        Debug.Log("Restart mit Fade");

        ScreenFader.Instance.FadeToScene("Level01_Scene");
    }

    public void GoToMainMenu()
    {
        GameResultData.Reset();

        if (!ScreenFader.Instance)
        {
            Debug.LogError("Kein ScreenFader gefunden!");
            return;
        }

        Debug.Log("Main Menu mit Fade");

        ScreenFader.Instance.FadeToScene("MainMenu_Scene");
    }
}