using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoryPathFollower : MonoBehaviour
{
    [System.Serializable]
    public class SegmentSpeedOverride
    {
        [Tooltip("Für welchen Streckenabschnitt die Geschwindigkeit geändert werden soll. " + "Beispiel: Waypoint Index 50 = Strecke von Waypoint 49 zu Waypoint 50.")]
        public int waypointIndex;

        [Tooltip("1 = normal, 0.5 = halb so schnell, 2 = doppelt so schnell.")]
        public float speedMultiplier = 1f;
    }

    [Header("Path")]
    [SerializeField] private Transform[] waypoints;

    [Header("Segment Speed Overrides")]
    [Tooltip("Nur die einzelnen Streckenabschnitte eintragen, die schneller oder langsamer sein sollen.")]
    [SerializeField] private List<SegmentSpeedOverride> segmentSpeedOverrides = new List<SegmentSpeedOverride>();

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
    private float calculatedBaseSpeed;
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
        CalculateBaseSpeedForLevelDuration();
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

            FinishLevel();
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

    private void CalculateBaseSpeedForLevelDuration()
    {
        if (waypoints.Length == 0 || levelDuration <= 0f)
            return;

        float weightedDistance = 0f;

        Vector3 previousPosition = transform.position;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(previousPosition, waypoints[i].position);
            float multiplier = GetSpeedMultiplierForSegment(i);
            
            weightedDistance += distance / multiplier;
            previousPosition = waypoints[i].position;
        }

        calculatedBaseSpeed = weightedDistance / levelDuration;
    }

    private float GetSpeedMultiplierForSegment(int waypointIndex)
    {
        if (segmentSpeedOverrides == null || segmentSpeedOverrides.Count == 0)
        {
            return 1f;
        }

        foreach (SegmentSpeedOverride speedOverride in segmentSpeedOverrides)
        {
            if (speedOverride == null)
                continue;

            if (speedOverride.waypointIndex == waypointIndex)
            {
                return Mathf.Max(0.01f, speedOverride.speedMultiplier);
            }
        }

        return 1f;
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;

        float currentSpeed = calculatedBaseSpeed * GetSpeedMultiplierForSegment(currentWaypointIndex);

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, currentSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.05f)
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

        Debug.Log("LEVEL BEENDET");

        if (!sceneChangeStarted)
        {
            sceneChangeStarted = true;
            StartCoroutine(ChangeToEndingScene());
        }
    }

    private IEnumerator ChangeToEndingScene()
    {
        yield return new WaitForSecondsRealtime(
            endingDelay
        );

        if (GlitzieManager.Instance)
        {
            GlitzieManager.Instance.SaveResults();
        }

        if (ScreenFader.Instance)
        {
            ScreenFader.Instance.FadeToScene("Ending_Scene");
        }
        else
        {
            SceneManager.LoadScene("Ending_Scene");
        }
    }
}