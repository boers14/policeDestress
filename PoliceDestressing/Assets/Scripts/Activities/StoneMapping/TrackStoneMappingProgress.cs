using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackStoneMappingProgress : MonoBehaviour
{
    [System.NonSerialized]
    public PatternSelection switchPattern = null;

    [System.NonSerialized]
    public VisualizeStoneMappingProgress trackProgress = null;

    [System.NonSerialized]
    public bool completedThePattern = false;

    private List<MapTrackerChecker> mapTrackerCheckers = new List<MapTrackerChecker>();

    private float timer = 0, cooldown = 2.05f;

    private bool checkIfTurnOnParticles = false, particlesAreOn = false;

    // Fill the mapTrackerCheckers list with all MapTrackerCheckers
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<MapTrackerChecker>() != null)
            {
                mapTrackerCheckers.Add(transform.GetChild(i).GetComponent<MapTrackerChecker>());
            }
        }
    }

    // Check if should play particles
    private void FixedUpdate()
    {
        // Check if it should even try
        if (checkIfTurnOnParticles)
        {
            timer += Time.fixedDeltaTime;

            // if the particles arent on and the time is right
            if (timer >= cooldown && !particlesAreOn)
            {
                particlesAreOn = true;
                // turn on all particles of undone mapTrackerCheckers
                for (int i = 0; i < mapTrackerCheckers.Count; i++)
                {
                    if (!mapTrackerCheckers[i].isDone)
                    {
                        mapTrackerCheckers[i].particleSystem.Play();
                    } else
                    {
                        mapTrackerCheckers[i].particleSystem.Stop();
                    }
                }
            }
        }
    }

    // Check whether the pattern is complete if its not complete yet
    public void TrackProgress()
    {
        if (!completedThePattern)
        {
            // Count completed mapTrackerCheckers
            int amountOfCheckPointsCompleted = 0;

            for (int i = 0; i < mapTrackerCheckers.Count; i++)
            {
                if (mapTrackerCheckers[i].isDone)
                {
                    amountOfCheckPointsCompleted++;
                }
            }

            // Update progress bar
            float percentage = (float)amountOfCheckPointsCompleted / (float)transform.childCount;
            trackProgress.UpdateProgressBar(percentage, transform.childCount, mapTrackerCheckers);

            // Check whether particles should be on
            if (percentage >= 0.95f && percentage <= 1)
            {
                // turn off done mapTrackerCheckers particles
                timer = 0;
                checkIfTurnOnParticles = true;
                TurnOffParticles(false);
            }
            else
            {
                // turn of all mapTrackerCheckers particles
                if (checkIfTurnOnParticles)
                {
                    TurnOffParticles(true);
                }

                checkIfTurnOnParticles = false;
            }

            // Start coroutine of map selection if map is complete
            if (amountOfCheckPointsCompleted == transform.childCount)
            {
                StartCoroutine(SelectNextMap());
                completedThePattern = true;
            }
        }
    }

    // Select next pattern
    private IEnumerator SelectNextMap()
    {
        yield return new WaitForSeconds(7.5f);
        switchPattern.SelectRandomPattern();
    }

    // Turn off all mapTrackerCheckers particles or only completed mapTrackerCheckers particles
    private void TurnOffParticles(bool checkAll)
    {
        particlesAreOn = false;
        for (int i = 0; i < mapTrackerCheckers.Count; i++)
        {
            if (checkAll)
            {
                mapTrackerCheckers[i].particleSystem.Stop();
            } else
            {
                if (mapTrackerCheckers[i].isDone)
                {
                    mapTrackerCheckers[i].particleSystem.Stop();
                }
            }
        }
    }
}
