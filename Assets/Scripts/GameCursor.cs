using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCursor : MonoBehaviour
{
    public static GameCursor Instance;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;

    [Header("Hotspot")]
    [SerializeField] private Vector2 hotspot;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetCustomCursor();
    }

    private IEnumerator Start()
    {
        yield return null;

        ApplyCursorForCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
            Instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(ApplyCursorNextFrame(scene));
    }

    private IEnumerator ApplyCursorNextFrame(Scene scene)
    {
        yield return null;

        ApplyCursorForScene(scene);
    }

    private void ApplyCursorForCurrentScene()
    {
        ApplyCursorForScene(SceneManager.GetActiveScene());
    }

    private void ApplyCursorForScene(Scene scene)
    {
        if (scene.name == "Level01_Scene")
            HideCursor();
        else
            ShowCursor();
    }

    private void SetCustomCursor()
    {
        if (!cursorTexture)
            return;

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetCustomCursor();
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}