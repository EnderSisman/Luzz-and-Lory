using UnityEngine;
using UnityEngine.InputSystem;

public class BankDoor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MainMenuCameraController cameraController;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                cameraController.GoToBankDoor();
            }
        }
    }
}