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

        SetCustomCursor();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SetCustomCursor()
    {
        if (!cursorTexture)
            return;

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetCustomCursor();

        if (scene.name == "Level01_Scene")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}