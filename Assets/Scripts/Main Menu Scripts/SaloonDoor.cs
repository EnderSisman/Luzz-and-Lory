using UnityEngine;
using UnityEngine.InputSystem;

public class SaloonDoor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MainMenuCameraController cameraController;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorSwingSound;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (cameraController && cameraController.IsMoving)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform) || transform.IsChildOf(hit.collider.transform))
            {
                PlayDoorSound();

                if (cameraController)
                    cameraController.GoToSaloonDoor();
            }
        }
    }

    private void PlayDoorSound()
    {
        if (!audioSource || !doorSwingSound)
            return;

        audioSource.mute = AudioSettingsData.IsMuted;
        audioSource.PlayOneShot(doorSwingSound);
    }
}