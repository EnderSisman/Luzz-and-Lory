using UnityEngine;
using UnityEngine.InputSystem;

public class LuzzThrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform luzzAnchor;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private Transform catchPoint;

    [Header("Throw")]
    [SerializeField] private float throwDistance = 12f;
    [SerializeField] private float throwSpeed = 18f;

    [Header("Flight Arc")]
    [SerializeField] private float arcHeight = 0.5f;

    [Header("Slowdown")]
    [SerializeField] private float slowdownDistance = 3f;
    [SerializeField] private float minimumThrowSpeed = 4f;
    [SerializeField] private float catchPauseDuration = 0.12f;

    [Header("Return")]
    [SerializeField] private float returnStartSpeed = 4f;
    [SerializeField] private float returnSpeed = 22f;
    [SerializeField] private float returnAcceleration = 35f;

    [Header("Rotation")]
    [SerializeField] private float spinSpeed = 720f;
    [SerializeField] private Vector3 spinAxis = Vector3.up;

    private enum LuzzState
    {
        Sitting,
        Flying,
        CatchPause,
        Returning
    }

    private LuzzState state = LuzzState.Sitting;

    private Vector3 targetPosition;
    private Vector3 throwStartPosition;

    private float throwProgress;
    private float currentReturnSpeed;
    private float catchPauseTimer;

    private Glitzie capturedGlitzie;

    private void Update()
    {
        switch (state)
        {
            case LuzzState.Sitting:
                CheckForThrow();
                break;

            case LuzzState.Flying:
                FlyToTarget();
                break;

            case LuzzState.CatchPause:
                CatchPause();
                break;

            case LuzzState.Returning:
                ReturnToLory();
                break;
        }
    }

    private void CheckForThrow()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ThrowLuzz();
        }
    }

    private void ThrowLuzz()
    {
        Vector2 crosshairScreenPosition = RectTransformUtility.WorldToScreenPoint(null, crosshair.position);    // Position des Crosshairs auf dem Bildschirm
        Ray aimRay = mainCamera.ScreenPointToRay(crosshairScreenPosition);                                       // Ray durch das Crosshair
        targetPosition = aimRay.GetPoint(throwDistance);                                                            // Zielpunkt bestimmen                                                            
        throwStartPosition = transform.position;                                                                    // Startposition speichern                                                                
        throwProgress = 0f;                                                                                         // Fortschritt zurücksetzen
        transform.SetParent(null, true);                                                             // Luzz von Lory lösen
        state = LuzzState.Flying;
    }

    private void FlyToTarget()
    {
        SpinLuzz();

        float totalDistance = Vector3.Distance(throwStartPosition, targetPosition);

        float remainingDistance = totalDistance * (1f - throwProgress);

        float currentSpeed = throwSpeed;

        // Kurz vor dem Ziel abbremsen
        if (remainingDistance < slowdownDistance)
        {
            float slowdownFactor = remainingDistance / slowdownDistance;

            currentSpeed = Mathf.Lerp(minimumThrowSpeed, throwSpeed, slowdownFactor);
        }

        // Fortschritt erhöhen
        if (totalDistance > 0f)
        {
            throwProgress += (currentSpeed / totalDistance) * Time.deltaTime;
        }

        throwProgress = Mathf.Clamp01(throwProgress);

        // Grundbewegung Richtung Ziel
        Vector3 position = Vector3.Lerp(throwStartPosition, targetPosition, throwProgress);

        // Leichte Flugkurve
        float arc = Mathf.Sin(throwProgress * Mathf.PI) * arcHeight;
        position += Vector3.up * arc;
        transform.position = position;

        // Ziel erreicht
        if (throwProgress >= 1f)
        {
            transform.position = targetPosition;
            catchPauseTimer = catchPauseDuration;
            state = LuzzState.CatchPause;
        }
    }

    private void CatchPause()
    {
        SpinLuzz();

        catchPauseTimer -= Time.deltaTime;

        if (catchPauseTimer <= 0f)
        {
            currentReturnSpeed = returnStartSpeed;

            state = LuzzState.Returning;
        }
    }

    private void ReturnToLory()
    {
        SpinLuzz();

        // Rückflug beschleunigen
        currentReturnSpeed = Mathf.MoveTowards(currentReturnSpeed, returnSpeed, returnAcceleration * Time.deltaTime);

        // Zur fahrenden Lory zurück
        transform.position = Vector3.MoveTowards(transform.position, luzzAnchor.position, currentReturnSpeed * Time.deltaTime);

        // Lory erreicht
        if (Vector3.Distance(transform.position, luzzAnchor.position) < 0.05f)
        {
            transform.SetParent(luzzAnchor);

            transform.localPosition =
                Vector3.zero;

            transform.localRotation =
                Quaternion.identity;

            // Gefangenen Glitzie abgeben
            if (capturedGlitzie)
            {
                capturedGlitzie.Deliver();
                capturedGlitzie = null;
            }

            state = LuzzState.Sitting;
        }
    }

    private void SpinLuzz()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nicht fangen, wenn Luzz auf Lory sitzt
        if (state == LuzzState.Sitting)
            return;

        // Pro Wurf nur einen Glitzie
        if (capturedGlitzie)
            return;

        Glitzie glitzie = other.GetComponentInParent<Glitzie>();

        if (!glitzie)
            return;

        capturedGlitzie = glitzie;

        capturedGlitzie.Capture(catchPoint);

        Debug.Log("Luzz hat " + capturedGlitzie.Type + " gefangen!");
    }
}