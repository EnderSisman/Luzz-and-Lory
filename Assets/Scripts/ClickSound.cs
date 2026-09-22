using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSound : MonoBehaviour, IPointerClickHandler
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip signClickSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound();
    }

    private void PlaySound()
    {
        if (!audioSource || !signClickSound)
            return;

        audioSource.mute = AudioSettingsData.IsMuted;
        audioSource.PlayOneShot(signClickSound);
    }
}