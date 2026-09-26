using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoundButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private AudioSource musicSource;

    [Header("Icons")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite iconLaut;
    [SerializeField] private Sprite iconLeise;

    [Header("Hover Target")]
    [SerializeField] private RectTransform hoverTarget;

    [Header("Hover Wobble")]
    [SerializeField] private float maxAngle = 6f;
    [SerializeField] private float wobbleSpeed = 4f;
    [SerializeField] private float returnSpeed = 8f;

    [Header("Hover Scale")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float scaleSpeed = 8f;

    private bool isHovered;
    private bool isReturning;
    private float hoverTimer;

    private Vector3 normalScale;

    private void Awake()
    {
        if (hoverTarget)
            normalScale = hoverTarget.localScale;
    }

    private void Start()
    {
        ApplyMuteState();
        UpdateIcon();
    }

    private void Update()
    {
        if (!hoverTarget)
            return;

        if (isHovered)
        {
            hoverTimer += Time.unscaledDeltaTime;
            float angle = Mathf.Sin(hoverTimer * wobbleSpeed) * maxAngle;
            hoverTarget.localRotation = Quaternion.Euler(0f, 0f, angle);
            Vector3 targetScale = normalScale * hoverScale;
            hoverTarget.localScale = Vector3.Lerp(hoverTarget.localScale, targetScale, scaleSpeed * Time.unscaledDeltaTime);
        }
        else if (isReturning)
        {
            hoverTarget.localRotation = Quaternion.Lerp(hoverTarget.localRotation, Quaternion.identity, returnSpeed * Time.unscaledDeltaTime);
            hoverTarget.localScale = Vector3.Lerp(hoverTarget.localScale, normalScale, scaleSpeed * Time.unscaledDeltaTime);

            if (Quaternion.Angle(hoverTarget.localRotation, Quaternion.identity) < 0.1f && Vector3.Distance(hoverTarget.localScale, normalScale) < 0.01f)
            {
                hoverTarget.localRotation = Quaternion.identity;
                hoverTarget.localScale = normalScale;
                isReturning = false;
            }
        }
    }

    public void ToggleSound()
    {
        AudioSettingsData.IsMuted = !AudioSettingsData.IsMuted;

        ApplyMuteState();
        UpdateIcon();
    }

    public void ResetHoverImmediate()
    {
        isHovered = false;
        isReturning = false;
        hoverTimer = 0f;

        if (hoverTarget)
            hoverTarget.localRotation = Quaternion.identity;
    }

    private void ApplyMuteState()
    {
        if (musicSource)
            musicSource.mute = AudioSettingsData.IsMuted;
    }

    private void UpdateIcon()
    {
        if (!buttonImage)
            return;

        buttonImage.sprite = AudioSettingsData.IsMuted ? iconLeise : iconLaut;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        isReturning = false;
        hoverTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isReturning = true;
        hoverTimer = 0f;
    }
}