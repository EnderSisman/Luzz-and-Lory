using UnityEngine;

public class HighscoreFloat : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] private float floatHeight = 12f;
    [SerializeField] private float floatSpeed = 1.5f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private float timer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (!rectTransform)
            return;

        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (!rectTransform)
            return;

        timer += Time.unscaledDeltaTime;

        float offsetY = -Mathf.Sin(timer * floatSpeed) * floatHeight;

        rectTransform.anchoredPosition = startPosition + new Vector2(0f, offsetY);
    }
}