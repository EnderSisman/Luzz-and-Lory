using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SaloonDoor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

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
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        GameResultData.Reset();

        SceneManager.LoadScene("Level01_Scene");
    }
}