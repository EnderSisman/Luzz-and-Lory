using UnityEngine;

public class MiniLoryBounce : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LoryPathFollower lory;

    [Header("Bounce")]
    [SerializeField] private float firstJumpHeight = 0.4f;
    [SerializeField] private float secondJumpHeight = 0.2f;

    [SerializeField] private float firstJumpDuration = 0.18f;
    [SerializeField] private float secondJumpDuration = 0.14f;

    [SerializeField] private float pauseBetweenLoops = 0.12f;

    private Vector2 startPosition;

    private enum BounceState
    {
        FirstUp,
        FirstDown,
        SecondUp,
        SecondDown,
        Pause
    }

    private BounceState state = BounceState.FirstUp;

    private float timer = 0f;
    private float pauseTimer = 0f;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        // Wenn das Level beendet ist, nicht mehr bouncen
        if (lory && lory.Progress01 >= 1f)
        {
            transform.localPosition = startPosition;
            return;
        }

        switch (state)
        {
            case BounceState.FirstUp:
                MoveUp(firstJumpHeight, firstJumpDuration, BounceState.FirstDown);
                break;

            case BounceState.FirstDown:
                MoveDown(firstJumpHeight, firstJumpDuration, BounceState.SecondUp);
                break;

            case BounceState.SecondUp:
                MoveUp(secondJumpHeight, secondJumpDuration, BounceState.SecondDown);
                break;

            case BounceState.SecondDown:
                MoveDown(secondJumpHeight, secondJumpDuration, BounceState.Pause);
                break;

            case BounceState.Pause:
                Pause();
                break;
        }
    }

    private void MoveUp(float height, float duration, BounceState nextState)
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        transform.localPosition = Vector2.Lerp(startPosition, startPosition + Vector2.up * height, t);

        if (t >= 1f)
        {
            timer = 0f;
            state = nextState;
        }
    }

    private void MoveDown(float height, float duration, BounceState nextState)
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        transform.localPosition = Vector2.Lerp(startPosition + Vector2.up * height, startPosition, t);

        if (t >= 1f)
        {
            timer = 0f;
            state = nextState;
        }
    }

    private void Pause()
    {
        transform.localPosition = startPosition;
        pauseTimer += Time.deltaTime;

        if (pauseTimer >= pauseBetweenLoops)
        {
            pauseTimer = 0f;
            state = BounceState.FirstUp;
        }
    }
}