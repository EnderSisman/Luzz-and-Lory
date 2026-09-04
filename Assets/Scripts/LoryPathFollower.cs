using UnityEngine;

public class LoryPathFollower : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] waypoints;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;
    private bool reachedEnd = false;

    private void Start(){
        MoveToNextWaypoint();
    }

    private void Update()
    {
        if (reachedEnd || waypoints.Length == 0)
            return;
    }

    private void MoveToNextWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        Vector3 direction = targetWaypoint.position - transform.position;

        // Lory bewegen
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Lory in Fahrtrichtung drehen
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
                OnReachedEnd();
            }
        }
    }

    private void OnReachedEnd()
    {
        Debug.Log("Lory hat das Streckenende erreicht.");
    }
}