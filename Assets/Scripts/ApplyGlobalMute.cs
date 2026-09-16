using UnityEngine;

public class ApplyGlobalMute : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        if (audioSource)
            audioSource.mute = AudioSettingsData.IsMuted;
    }
}