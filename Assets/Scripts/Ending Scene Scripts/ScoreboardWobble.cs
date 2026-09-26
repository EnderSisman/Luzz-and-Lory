using UnityEngine;

public class ScoreboardWobble : MonoBehaviour
{
    [Header("Wobble")]
    [SerializeField] private float maxAngle = 3f;
    [SerializeField] private float wobbleSpeed = 1.5f;

    private RectTransform rectTransform;
    private float timer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!rectTransform)
            return;

        timer += Time.unscaledDeltaTime;

        float angle = Mathf.Sin(timer * wobbleSpeed) * maxAngle;
        
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}