using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    [SerializeField] private LoryPathFollower lory;
    [SerializeField] private Slider progressSlider;

    private void Update()
    {
        if (!lory || !progressSlider)
            return;

        progressSlider.value = lory.Progress01;
    }
}