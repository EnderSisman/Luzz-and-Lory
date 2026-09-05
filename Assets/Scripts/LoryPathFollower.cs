using UnityEngine;

public class LoryPathFollower : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] waypoints;

    [Header("Level")]
    [SerializeField] private float levelDuration = 60f;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;
    private bool reachedEnd = false;

    private float totalPathDistance;
    private float calculatedMoveSpeed;
    private float elapsedTime;

    // Fortschritt zwischen 0 und 1
    public float Progress01
    {
        get
        {
            if (levelDuration <= 0f)
                return 0f;

            return Mathf.Clamp01(elapsedTime / levelDuration);
        }
    }

    private void Start()
    {
        CalculateTotalPathDistance();

        if (levelDuration > 0f)
        {
            calculatedMoveSpeed = totalPathDistance / levelDuration;
        }
    }

    private void Update()
    {
        if (reachedEnd || waypoints.Length == 0)
            return;

        elapsedTime += Time.deltaTime;

        MoveToNextWaypoint();

        if (elapsedTime >= levelDuration)
        {
            elapsedTime = levelDuration;
        }
    }

    private void CalculateTotalPathDistance()
    {
        if (waypoints.Length == 0)
            return;

        totalPathDistance = Vector3.Distance(transform.position, waypoints[0].position);

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            totalPathDistance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }
    }

    private void MoveToNextWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;

        // Lory bewegen
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, calculatedMoveSpeed * Time.deltaTime);

        // Lory drehen
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Wegpunkt erreicht
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.05f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                reachedEnd = true;
                elapsedTime = levelDuration;

                OnReachedEnd();
            }
        }
    }

    private void OnReachedEnd()
    {
        Debug.Log("Lory hat das Streckenende erreicht.");
    }
}