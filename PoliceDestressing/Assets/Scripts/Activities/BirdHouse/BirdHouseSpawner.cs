using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHouseSpawner : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject birdHouseToSpawn = null, hologramToSpawn = null;

    private List<GameObject> currentBirdHouse = new List<GameObject>();

    [SerializeField]
    private GameObject workBench = null;

    [SerializeField]
    private float addedXValue = 0.0f, addedYValue = 0.0f, addedZValue = 0.0f;

    // This function is added to the lever
    // Spawn the birdhouse components
    public void SpawnBirdHouseComponents()
    {
        // Stop the bird from eating if it was eating
        GameObject[] birds = GameObject.FindGameObjectsWithTag("Bird");
        for (int i = 0; i < birds.Length; i++)
        {
            if (birds[i].GetComponent<BirdBehaviour>().isInteractingWithFood)
            {
                birds[i].GetComponent<BirdBehaviour>().isInteractingWithFood = false;
                birds[i].GetComponent<BirdBehaviour>().SetBirdToFlyingState(birds[i].GetComponent<BirdBehaviour>().returnToBaseTimerCooldown, true);
            }
        }

        // Deactivate current birdhouse
        for (int i = 0; i < currentBirdHouse.Count; i++)
        {
            if (currentBirdHouse[i] != null)
            {
                currentBirdHouse[i].SetActive(false);
            }
        }
        currentBirdHouse.Clear();

        // Spawn the new birdhouse
        Vector3 workbenchPos = workBench.transform.position;
        GameObject birdHouseComponents = Instantiate(birdHouseToSpawn, new Vector3(workbenchPos.x,
            workbenchPos.y, workbenchPos.z), workBench.transform.rotation);
        Vector3 rot = birdHouseComponents.transform.eulerAngles;
        rot.y += 90;
        birdHouseComponents.transform.eulerAngles = rot;
        currentBirdHouse.Add(birdHouseComponents);
    }

    // This function is added to the lever
    // This spawns the hologram on the correct position
    public void SpawnHologram()
    {
        Vector3 hologramPos = workBench.transform.position;
        GameObject hologramComponent = Instantiate(hologramToSpawn, new Vector3(hologramPos.x + addedXValue,
            hologramPos.y + addedYValue, hologramPos.z + addedZValue), workBench.transform.rotation);
        Vector3 rot = hologramComponent.transform.eulerAngles;
        rot.y += 90;
        hologramComponent.transform.eulerAngles = rot;
        currentBirdHouse.Add(hologramComponent);
    }

    // Add objects to current birdhouse list
    public void AddObjectToBirdHouse(GameObject addedObject)
    {
        currentBirdHouse.Add(addedObject);
    }
}
