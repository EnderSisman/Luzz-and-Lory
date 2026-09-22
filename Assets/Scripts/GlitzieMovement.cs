using UnityEngine;

public class GlitzieMovement : MonoBehaviour
{
    public enum MovementType
    {
        Wander,
        Circle,
        Jump
    }

    [Header("Movement Type")]
    [SerializeField] private MovementType movementType = MovementType.Wander;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("General")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float minPauseTime = 0.3f;
    [SerializeField] private float maxPauseTime = 1.2f;
    [SerializeField] private float targetReachedDistance = 0.2f;

    [Header("Circle")]
    [SerializeField] private float circleRadius = 2f;
    [SerializeField] private float circleSpeed = 60f;
    [SerializeField] private bool clockwise = true;

    [Header("Jump")]
    [SerializeField] private float jumpRadius = 3f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.6f;
    [SerializeField] private float jumpPause = 0.3f;

    private Vector3 startPosition;

    // Wander
    private Vector3 wanderTarget;
    private bool isWaiting;
    private float waitTimer;

    // Circle
    private float circleAngle;

    // Jump
    private Vector3 jumpStart;
    private Vector3 jumpTarget;
    private float jumpTimer;
    private float jumpPauseTimer;
    private bool isJumping;

    // Animation
    private string currentAnimation;

    private void Start()
    {
        startPosition = transform.position;

        switch (movementType)
        {
            case MovementType.Wander:
                ChooseNewWanderTarget();
                PlayAnimation("Walk");
                break;

            case MovementType.Circle:
                circleAngle = 0f;
                PlayAnimation("Walk");
                break;

            case MovementType.Jump:
                PrepareNextJump();
                PlayAnimation("Idle");
                break;
        }
    }

    private void Update()
    {
        switch (movementType)
        {
            case MovementType.Wander:
                WanderMovement();
                break;

            case MovementType.Circle:
                CircleMovement();
                break;

            case MovementType.Jump:
                JumpMovement();
                break;
        }
    }

    // --------------------------------------------------
    // WANDER GLITZIE
    // --------------------------------------------------

    private void WanderMovement()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                ChooseNewWanderTarget();
                PlayAnimation("Walk");
            }

            return;
        }

        Vector3 direction = wanderTarget - transform.position;
        direction.y = 0f;

        RotateTowards(direction);

        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, wanderTarget) <= targetReachedDistance)
        {
            isWaiting = true;
            waitTimer = Random.Range(minPauseTime, maxPauseTime);

            PlayAnimation("Idle");
        }
    }

    private void ChooseNewWanderTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;

        wanderTarget = new Vector3(startPosition.x + randomPoint.x, startPosition.y, startPosition.z + randomPoint.y);
    }

    // --------------------------------------------------
    // CIRCLE GLITZIE
    // --------------------------------------------------

    private void CircleMovement()
    {
        float directionMultiplier = clockwise ? -1f : 1f;
        circleAngle += circleSpeed * directionMultiplier * Time.deltaTime;
        float angleRadians = circleAngle * Mathf.Deg2Rad;

        Vector3 newPosition = new Vector3(startPosition.x + Mathf.Cos(angleRadians) * circleRadius, startPosition.y, startPosition.z + Mathf.Sin(angleRadians) * circleRadius);
        Vector3 tangent = new Vector3(-Mathf.Sin(angleRadians) * directionMultiplier, 0f, Mathf.Cos(angleRadians) * directionMultiplier);

        if (tangent.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tangent);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position = newPosition;

        PlayAnimation("Walk");
    }

    // --------------------------------------------------
    // JUMP GLITZIE
    // --------------------------------------------------

    private void JumpMovement()
    {
        if (!isJumping)
        {
            jumpPauseTimer -= Time.deltaTime;

            if (jumpPauseTimer <= 0f)
            {
                StartJump();
            }

            return;
        }

        jumpTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(jumpTimer / jumpDuration);
        Vector3 position = Vector3.Lerp(jumpStart, jumpTarget, progress);

        float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
        position.y += height;

        Vector3 direction = jumpTarget - jumpStart;
        direction.y = 0f;

        RotateTowards(direction);

        transform.position = position;

        if (progress >= 1f)
        {
            transform.position = jumpTarget;

            isJumping = false;

            PrepareNextJump();
        }
    }

    private void StartJump()
    {
        jumpStart = transform.position;

        Vector2 randomPoint = Random.insideUnitCircle * jumpRadius;

        jumpTarget = new Vector3(startPosition.x + randomPoint.x, startPosition.y, startPosition.z + randomPoint.y);

        jumpTimer = 0f;
        isJumping = true;

        PlayAnimation("Jump");
    }

    private void PrepareNextJump()
    {
        jumpPauseTimer = jumpPause;
        isJumping = false;

        PlayAnimation("Idle");
    }

    // --------------------------------------------------
    // ROTATION GLITZIE
    // --------------------------------------------------

    private void RotateTowards(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // --------------------------------------------------
    // ANIMATION
    // --------------------------------------------------

    private void PlayAnimation(string animationName)
    {
        if (!animator)
            return;

        if (currentAnimation == animationName)
            return;

        currentAnimation = animationName;
        animator.Play(animationName, 0, 0f);
    }
}