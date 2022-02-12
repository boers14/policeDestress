using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHouseBook : SwipeSelection
{
    [SerializeField]
    private GameObject pageTwo = null;

    [SerializeField]
    private List<GameObject> allBirdHouses = new List<GameObject>();

    [SerializeField]
    private List<GameObject> allHolograms = new List<GameObject>();

    [SerializeField]
    private List<Material> birdHouseDescriptions = new List<Material>();

    private Renderer pageTwoRenderer;

    [SerializeField]
    private BirdHouseSpawner birdHouseSpawner = null;

    // Inherit from the swipe selection and perform its functions
    // Set Variables
    public override void Start()
    {
        pageTwoRenderer = pageTwo.GetComponent<Renderer>();
        base.Start();
    }

    // Also correctly sets the stats for the birdhouse spawner and second page equal to the given page
    public override void FlipPage(int direction)
    {
        base.FlipPage(direction);
        pageTwoRenderer.material = birdHouseDescriptions[page];
        birdHouseSpawner.birdHouseToSpawn = allBirdHouses[page];
        birdHouseSpawner.hologramToSpawn = allHolograms[page];
    }
}
