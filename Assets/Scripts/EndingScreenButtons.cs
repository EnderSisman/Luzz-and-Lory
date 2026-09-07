using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScreenButtons : MonoBehaviour
{
    public void RestartLevel()
    {
        GameResultData.Reset();

        SceneManager.LoadScene("Level01_Scene");
    }

    public void GoToMainMenu()
    {
        GameResultData.Reset();

        SceneManager.LoadScene("MainMenu_Scene");
    }
}