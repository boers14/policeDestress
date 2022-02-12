using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStones : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> possibleStones = new List<GameObject>();

    [System.NonSerialized]
    public List<GameObject> allCurrentStones = new List<GameObject>();

    private List<List<GameObject>> allStonesLists = new List<List<GameObject>>();

    [SerializeField]
    private SwipeSelection swipeSelection = null;

    [SerializeField]
    private Transform spawnPos = null;

    [SerializeField]
    private int maxAmountOfStones = 50, stonesSpawnedEveryTime = 5;

    [SerializeField]
    private float stoneScale = 1;

    private Color32 regularColor = Color.white;

    [SerializeField]
    private Color32 cantSpawnColor = Color.red;

    private bool canSpawn = true;

    private List<Collider> collidingStones = new List<Collider>();

    private List<int> currentIndexes = new List<int>();

    // Set variables and fill object pool
    private void Start()
    {
        regularColor = GetComponent<Renderer>().material.color;

        for (int i = 0; i < possibleStones.Count; i++)
        {
            List<GameObject> stoneList = new List<GameObject>();
            AddNewStonesToList(stoneList, possibleStones[i], maxAmountOfStones / 5);
            allStonesLists.Add(stoneList);
            currentIndexes.Add(0);
        }
    }

    // Spawn stones if there are no colliding stones
    private void FixedUpdate()
    {
        if (collidingStones.Count == 0 && canSpawn)
        {
            for (int  i = 0; i < stonesSpawnedEveryTime - (swipeSelection.page * 2); i++)
            {
                SpawnStone();
            }
        }
        collidingStones.Clear();
    }

    // Add stones to collidingStones list
    private void OnTriggerEnter(Collider other)
    {
        if (!collidingStones.Contains(other) && other.tag == "StoneForMapping")
        {
            collidingStones.Add(other);
        }
    }

    // Add stones to collidingStones list
    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    // Spawn a stone if there are less stones then the max limit
    private void SpawnStone()
    {
        if (allCurrentStones.Count < maxAmountOfStones)
        {
            // Add new stones to object pool if there are no more stonesin the needed list
            if (currentIndexes[swipeSelection.page] + 1 > allStonesLists[swipeSelection.page].Count - 1)
            {
                AddNewStonesToList(allStonesLists[swipeSelection.page], possibleStones[swipeSelection.page], 5);
            }

            // Instantiate stone of list based on index
            GameObject newBrick = allStonesLists[swipeSelection.page][currentIndexes[swipeSelection.page]];
            newBrick.transform.position = spawnPos.position;
            newBrick.transform.rotation = Quaternion.identity;
            newBrick.SetActive(true);
            allCurrentStones.Add(newBrick);
            currentIndexes[swipeSelection.page]++;
        } else
        {
            // If the stone are over the max change color
            StartCoroutine(ColorSpawner());
        }
    }

    // Change color of spawner and turn of spawning
    private IEnumerator ColorSpawner()
    {
        canSpawn = false;
        GetComponent<Renderer>().material.color = cantSpawnColor;
        yield return new WaitForSeconds(5f);
        GetComponent<Renderer>().material.color = regularColor;
    }

    // Deactivate all stones and reset the indexes. Make sure that it can spawn again.
    public void DeleteAllStones()
    {
        for (int i = 0; i < allCurrentStones.Count; i++)
        {
            allCurrentStones[i].SetActive(false);
            allCurrentStones[i].transform.position = new Vector3(-100, -100, -100);
        }

        for (int i = 0; i < currentIndexes.Count; i++)
        {
            currentIndexes[i] = 0;
        }

        allCurrentStones.Clear();
        canSpawn = true;
    }

    // Add stones to the neccesary list
    private void AddNewStonesToList(List<GameObject> stoneList, GameObject stoneType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newBrick = Instantiate(stoneType, new Vector3(-100, -100, -100), Quaternion.identity);
            newBrick.transform.localScale *= stoneScale;
            newBrick.SetActive(false);
            stoneList.Add(newBrick);
        }
    }
}
