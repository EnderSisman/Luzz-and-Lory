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
    [SerializeField] private float returnStartSpeed = 2f;
    [SerializeField] private float returnSpeed = 22f;
    [SerializeField] private float returnAcceleration = 35f;

    [Header("Rotation")]
    [SerializeField] private float spinSpeed = 720f;
    [SerializeField] private Vector3 spinAxis = Vector3.up;

    [Header("World Collision")]
    [SerializeField] private LayerMask worldLayer;
    [SerializeField] private float collisionRadius = 0.25f;

    [Header("Bounce")]
    [SerializeField] private float bounceDistance = 1f;
    [SerializeField] private float bounceStartSpeed = 14f;
    [SerializeField] private float bounceEndSpeed = 1.5f;

    private enum LuzzState
    {
        Sitting,
        Flying,
        CatchPause,
        Bouncing,
        Returning
    }

    private LuzzState state = LuzzState.Sitting;

    private Vector3 targetPosition;
    private Vector3 throwStartPosition;

    private Vector3 bounceStartPosition;
    private Vector3 bounceTarget;

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

            case LuzzState.Bouncing:
                Bounce();
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
        Vector2 crosshairScreenPosition = RectTransformUtility.WorldToScreenPoint(null, crosshair.position);       // Position des Crosshairs auf dem Bildschirm
        Ray aimRay = mainCamera.ScreenPointToRay(crosshairScreenPosition);                                          // Ray durch das Crosshair
        
        targetPosition = aimRay.GetPoint(throwDistance);                                                               // Zielposition bestimmen                                                             
       
        throwStartPosition = transform.position;
        throwProgress = 0f;

        transform.SetParent(null, true);

        state = LuzzState.Flying;
    }

    private void FlyToTarget()
    {
        SpinLuzz();

        float totalDistance = Vector3.Distance(throwStartPosition, targetPosition);
        float remainingDistance = totalDistance * (1f - throwProgress);
        float currentSpeed = throwSpeed;
        
        if (remainingDistance < slowdownDistance)
        {
            float slowdownFactor = remainingDistance / slowdownDistance;
            currentSpeed = Mathf.Lerp(minimumThrowSpeed, throwSpeed, slowdownFactor);
        }

        if (totalDistance > 0f)
        {
            throwProgress += (currentSpeed / totalDistance) * Time.deltaTime;
        }

        throwProgress = Mathf.Clamp01(throwProgress);
        Vector3 position = Vector3.Lerp(throwStartPosition, targetPosition, throwProgress);

        
        float arc = Mathf.Sin(throwProgress * Mathf.PI) * arcHeight;                     // Leichte Flugkurve

        position += Vector3.up * arc;
        Vector3 movement = position - transform.position;

        if (movement.sqrMagnitude > 0f)
        {
            if (Physics.SphereCast(transform.position, collisionRadius, movement.normalized, out RaycastHit hit, movement.magnitude, worldLayer, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point + hit.normal * collisionRadius;
                bounceStartPosition = transform.position;
                bounceTarget = transform.position + hit.normal * bounceDistance;
                state = LuzzState.Bouncing;
                
                return;
            }
        }
        
        transform.position = position;

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

    private void Bounce()
    {
        SpinLuzz();

        float totalBounceDistance = Vector3.Distance(bounceStartPosition, bounceTarget);
        float travelledBounceDistance = Vector3.Distance(bounceStartPosition, transform.position);
        float bounceProgress = 0f;

        if (totalBounceDistance > 0f)
        {
            bounceProgress = Mathf.Clamp01(travelledBounceDistance / totalBounceDistance);
        }

        float currentBounceSpeed = Mathf.Lerp(bounceStartSpeed, bounceEndSpeed, bounceProgress);

        transform.position = Vector3.MoveTowards(transform.position, bounceTarget, currentBounceSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, bounceTarget) < 0.05f)
        {
            currentReturnSpeed = returnStartSpeed;
            state = LuzzState.Returning;
        }
    }

    private void ReturnToLory()
    {
        SpinLuzz();

        currentReturnSpeed = Mathf.MoveTowards(currentReturnSpeed, returnSpeed, returnAcceleration * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, luzzAnchor.position, currentReturnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, luzzAnchor.position) < 0.05f)
        {
            transform.SetParent(luzzAnchor);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            if (capturedGlitzie)                        // Gefangenen Glitzie abgeben
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
        if (state == LuzzState.Sitting)
            return;

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