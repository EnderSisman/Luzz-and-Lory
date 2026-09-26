using UnityEngine;
using UnityEngine.EventSystems;

public class EndingSignHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Hover")]
    [SerializeField] private float maxAngle = 6f;
    [SerializeField] private float wobbleSpeed = 4f;
    [SerializeField] private float returnSpeed = 8f;

    private RectTransform rectTransform;

    private bool isHovered;
    private float hoverTimer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!rectTransform)
            return;

        if (isHovered)
        {
            hoverTimer += Time.unscaledDeltaTime;

            float angle = Mathf.Sin(hoverTimer * wobbleSpeed) * maxAngle;

            rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, Quaternion.identity, returnSpeed * Time.unscaledDeltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        hoverTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}