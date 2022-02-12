using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeStoneMappingProgress : MonoBehaviour
{
    [SerializeField]
    private Transform progressBar = null;

    private Renderer progressBarRenderer = null;

    [SerializeField]
    private float yMovement = 0, zScale = 0;

    [SerializeField]
    private new ParticleSystem particleSystem = null, specialParticleSystem = null;

    private Vector3 originalPos = Vector3.zero, originalScale = Vector3.zero;

    [SerializeField]
    private int perfectRecordLeeway = 5;

    [SerializeField]
    private SpawnStones spawnStones = null;

    [SerializeField]
    private Color32 perfectRecordColor = Color.yellow, normalRecordColor = Color.green;

    // Set variables
    private void Start()
    {
        progressBarRenderer = progressBar.GetComponent<Renderer>();
        originalPos = progressBar.transform.localPosition;
        originalScale = progressBar.transform.localScale;
        // Make sure the bar is at 0%
        UpdateProgressBar(0);
    }

    // Set desired stats of the progress bar
    public void UpdateProgressBar(float percentage, int childCount = -1, List<MapTrackerChecker> mapTrackerCheckers = null)
    {
        // calculate RGB value for color of progress bar
        float greenValue = 0;
        float redValue = 255;

        if (percentage <= 0.5f)
        {
            greenValue = 255 * (percentage * 2);
        }
        else
        {
            redValue = 255 * ((1 - percentage) * 2);
            greenValue = 255;
        }

        progressBarRenderer.material.color = new Color32((byte)redValue, (byte)greenValue, 0, 1);

        // Calculate postion
        Vector3 newPos = originalPos;
        newPos.y = originalPos.y + (yMovement * percentage);
        progressBar.transform.localPosition = newPos;

        // Calculate scale
        Vector3 newScale = originalScale;
        newScale.z = originalScale.z + (zScale * percentage);
        progressBar.transform.localScale = newScale;

        // Celebrate if the pattern is done else stop the celebration
        if (percentage >= 1)
        {
            if (spawnStones.allCurrentStones.Count <= childCount + perfectRecordLeeway)
            {
                PlayCeremony(mapTrackerCheckers, specialParticleSystem, perfectRecordColor);
            } else
            {
                PlayCeremony(mapTrackerCheckers, particleSystem, normalRecordColor);
            }
        }
        else
        {
            particleSystem.Stop();
            specialParticleSystem.Stop();
        }
    }

    // Play desired ceremony based on variables given
    private void PlayCeremony(List<MapTrackerChecker> mapTrackerCheckers, ParticleSystem neededParticleSystem, Color32 neededColor)
    {
        neededParticleSystem.Play();
        for (int i = 0; i < mapTrackerCheckers.Count; i++)
        {
            mapTrackerCheckers[i].TweenColor(neededColor);
        }
    }
}
