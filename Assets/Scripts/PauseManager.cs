using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject steuerungPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Audio")]
    [SerializeField] private AudioSource levelMusic;
    [SerializeField] private AudioSource levelSFX;

    [Header("Pause Sounds")]
    [SerializeField] private AudioSource pauseAudioSource;
    [SerializeField] private AudioClip pauseOpenSound;
    [SerializeField] private AudioClip pauseCloseSound;

    [Header("Audio Button")]
    [SerializeField] private Image audioButtonImage;
    [SerializeField] private Sprite iconLaut;
    [SerializeField] private Sprite iconLeise;

    [Header("Gameplay")]
    [SerializeField] private LuzzThrow luzzThrow;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private bool isPaused;
    private bool isMuted;
    private bool isChangingScene;

    private void Start()
    {
        Time.timeScale = 1f;

        if (steuerungPanel)
            steuerungPanel.SetActive(false);

        if (pausePanel)
            pausePanel.SetActive(false);

        isMuted = AudioSettingsData.IsMuted;

        if (levelMusic)
            levelMusic.mute = isMuted;

        if (levelSFX)
            levelSFX.mute = isMuted;

        if (pauseAudioSource)
            pauseAudioSource.mute = isMuted;

        HideCursor();
        UpdateAudioIcon();
    }

    private void Update()
    {
        if (isChangingScene)
            return;

        if (ScreenFader.Instance && ScreenFader.Instance.IsFading)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isChangingScene)
            return;

        if (ScreenFader.Instance && ScreenFader.Instance.IsFading)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused || isChangingScene)
            return;

        if (ScreenFader.Instance && ScreenFader.Instance.IsFading)
            return;

        isPaused = true;

        PlayPauseSound(pauseOpenSound);

        Time.timeScale = 0f;

        if (levelMusic)
            levelMusic.Pause();

        if (levelSFX)
            levelSFX.Pause();

        if (luzzThrow)
            luzzThrow.enabled = false;

        if (thirdPersonCamera)
            thirdPersonCamera.enabled = false;

        if (steuerungPanel)
            steuerungPanel.SetActive(true);

        if (pausePanel)
            pausePanel.SetActive(true);

        ShowCursor();
    }

    public void ResumeGame()
    {
        if (!isPaused || isChangingScene)
            return;

        isPaused = false;

        Time.timeScale = 1f;

        PlayPauseSound(pauseCloseSound);

        if (levelMusic)
        {
            levelMusic.mute = AudioSettingsData.IsMuted;
            levelMusic.UnPause();
        }

        if (levelSFX)
        {
            levelSFX.mute = AudioSettingsData.IsMuted;
            levelSFX.UnPause();
        }

        if (luzzThrow)
            luzzThrow.enabled = true;

        if (thirdPersonCamera)
            thirdPersonCamera.enabled = true;

        if (steuerungPanel)
            steuerungPanel.SetActive(false);

        if (pausePanel)
            pausePanel.SetActive(false);

        HideCursor();
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted;

        AudioSettingsData.IsMuted = isMuted;

        if (levelMusic)
            levelMusic.mute = isMuted;

        if (levelSFX)
            levelSFX.mute = isMuted;

        if (pauseAudioSource)
            pauseAudioSource.mute = isMuted;

        UpdateAudioIcon();
    }

    private void UpdateAudioIcon()
    {
        if (!audioButtonImage)
            return;

        audioButtonImage.sprite = AudioSettingsData.IsMuted ? iconLeise : iconLaut;
    }

    private void PlayPauseSound(AudioClip clip)
    {
        if (!pauseAudioSource || !clip)
            return;

        pauseAudioSource.mute = AudioSettingsData.IsMuted;
        pauseAudioSource.PlayOneShot(clip);
    }

    public void RestartLevel()
    {
        if (isChangingScene)
            return;

        isChangingScene = true;
        isPaused = false;

        Time.timeScale = 1f;

        GameResultData.Reset();

        HideCursor();

        if (levelMusic)
            levelMusic.Stop();

        if (levelSFX)
            levelSFX.Stop();

        if (pauseAudioSource)
            pauseAudioSource.Stop();

        if (ScreenFader.Instance)
            ScreenFader.Instance.FadeToScene("Level01_Scene");
        else
            SceneManager.LoadScene("Level01_Scene");
    }

    public void GoToMainMenu()
    {
        if (isChangingScene)
            return;

        isChangingScene = true;
        isPaused = false;

        Time.timeScale = 1f;

        GameResultData.Reset();

        if (levelMusic)
            levelMusic.Stop();

        if (levelSFX)
            levelSFX.Stop();

        if (pauseAudioSource)
            pauseAudioSource.Stop();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ScreenFader.Instance)
            ScreenFader.Instance.FadeToScene("MainMenu_Scene");
        else
            SceneManager.LoadScene("MainMenu_Scene");
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}