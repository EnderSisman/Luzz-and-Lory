using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoryPathFollower : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] waypoints;

    [Header("Level")]
    [SerializeField] private float levelDuration = 60f;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Level End")]
    [SerializeField] private float endingDelay = 1f;

    private int currentWaypointIndex = 0;

    private bool reachedEnd = false;
    private bool sceneChangeStarted = false;

    private float totalPathDistance;
    private float calculatedMoveSpeed;
    private float elapsedTime;

    public bool LevelFinished => reachedEnd;

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
            calculatedMoveSpeed =
                totalPathDistance / levelDuration;
        }
    }

    private void Update()
    {
        if (reachedEnd || waypoints.Length == 0)
            return;

        elapsedTime += Time.deltaTime;

        MoveToNextWaypoint();

        // Levelzeit erreicht
        if (elapsedTime >= levelDuration)
        {
            elapsedTime = levelDuration;
            FinishLevel();
        }
    }

    private void CalculateTotalPathDistance()
    {
        if (waypoints.Length == 0)
            return;

        totalPathDistance =
            Vector3.Distance(
                transform.position,
                waypoints[0].position
            );

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            totalPathDistance +=
                Vector3.Distance(
                    waypoints[i].position,
                    waypoints[i + 1].position
                );
        }
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length)
            return;

        Transform targetWaypoint =
            waypoints[currentWaypointIndex];

        Vector3 direction =
            targetWaypoint.position - transform.position;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetWaypoint.position,
                calculatedMoveSpeed * Time.deltaTime
            );

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }

        if (Vector3.Distance(
            transform.position,
            targetWaypoint.position
        ) < 0.05f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                FinishLevel();
            }
        }
    }

    private void FinishLevel()
    {
        if (reachedEnd)
            return;

        reachedEnd = true;
        elapsedTime = levelDuration;

        Debug.Log("LEVEL BEENDET - Szenenwechsel wird gestartet.");

        if (!sceneChangeStarted)
        {
            sceneChangeStarted = true;
            StartCoroutine(ChangeToEndingScene());
        }
    }

    private IEnumerator ChangeToEndingScene()
    {
        // Realtime, damit es auch bei Time.timeScale = 0 funktioniert
        yield return new WaitForSecondsRealtime(endingDelay);

        Debug.Log("Lade Ending_Scene...");

        SceneManager.LoadScene("Ending_Scene");
    }
}