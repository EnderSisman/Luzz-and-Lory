using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 2f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    [Header("Limits")]
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!target || Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitivity;
        pitch -= mouseDelta.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 offset = rotation * new Vector3(0f, height, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }
}