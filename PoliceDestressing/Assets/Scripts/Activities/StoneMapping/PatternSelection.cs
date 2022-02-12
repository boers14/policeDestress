using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternSelection : MonoBehaviour
{
    [SerializeField]
    private List<TrackStoneMappingProgress> allAvailablePatternsObjects = new List<TrackStoneMappingProgress>();

    private TrackStoneMappingProgress currentPattern = null;

    [SerializeField]
    private Transform patternPos = null;

    [SerializeField]
    private SpawnStones stoneSpawner = null;

    [SerializeField]
    private SwipeSelection swipeSelection = null;

    [SerializeField]
    private Vector3 paternRotationOffset = Vector3.zero;

    [SerializeField]
    private float patternScale = 1;

    [SerializeField]
    private VisualizeStoneMappingProgress trackProgress = null;

    private int chosenMap = 0;

    // This function is added to the lever
    // Select a new pattern
    public void SetNewPattern()
    {
        // Reset progress bar, remove pattern and remove stones
        trackProgress.UpdateProgressBar(0);

        if (currentPattern != null)
        {
            Destroy(currentPattern.gameObject);
        }

        stoneSpawner.DeleteAllStones();

        // Create new pattern and remember the index
        currentPattern = Instantiate(allAvailablePatternsObjects[swipeSelection.page], patternPos.position, Quaternion.identity);
        currentPattern.transform.eulerAngles += paternRotationOffset;
        currentPattern.transform.localScale *= patternScale;
        currentPattern.switchPattern = this;
        currentPattern.trackProgress = trackProgress;
        chosenMap = swipeSelection.page;
    }

    // Select a pattern with different index then the current index
    public void SelectRandomPattern()
    {
        int randomPattern = Random.Range(0, allAvailablePatternsObjects.Count);
        while (randomPattern == chosenMap)
        {
            randomPattern = Random.Range(0, allAvailablePatternsObjects.Count);
        }
        // edit the page to equal the selected pattern
        swipeSelection.page = randomPattern;
        swipeSelection.FlipPage(0);
        SetNewPattern();
    }
}
