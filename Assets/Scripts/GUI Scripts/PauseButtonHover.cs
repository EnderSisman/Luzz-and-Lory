using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Hover Rotation")]
    [SerializeField] private float maxAngle = 6f;
    [SerializeField] private float wobbleSpeed = 4f;
    [SerializeField] private float returnSpeed = 8f;

    [Header("Hover Scale")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float scaleSpeed = 8f;

    private RectTransform rectTransform;

    private bool isHovered;
    private float hoverTimer;

    private Vector3 normalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform)
            normalScale = rectTransform.localScale;
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
            Vector3 targetScale = normalScale * hoverScale;
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, scaleSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, Quaternion.identity, returnSpeed * Time.unscaledDeltaTime);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, normalScale, scaleSpeed * Time.unscaledDeltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        hoverTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ResetHover();
    }

    private void OnDisable()
    {
        ResetHover();

        if (rectTransform)
        {
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = normalScale;
        }
    }

    private void ResetHover()
    {
        isHovered = false;
        hoverTimer = 0f;
    }
}