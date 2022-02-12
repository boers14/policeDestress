using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHouseFoodPanel : MonoBehaviour
{
    private List<BirdBehaviour> allBirds = new List<BirdBehaviour>();

    public CompleteBirdHouse completeBirdHouse { get; set; } = null;

    private BirdBehaviour randomBird = null;

    public bool isOccupied { get; set; } = false;

    private List<GameObject> birdFoods = new List<GameObject>();

    // Fill the all birds list
    private void Start()
    {
        GetAllBirds();
    }

    // If the birdhouse is complete and it is not occupied set the object to being occupied
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BirdFood")
        {
            if (completeBirdHouse != null)
            {
                birdFoods.Add(other.gameObject);
                if (!isOccupied)
                {
                    SetOccupied();
                }
            }
        }
    }

    // Set the object to being occupied, call a bird and make food edible
    private void SetOccupied()
    {
        isOccupied = true;
        CallBird();
        birdFoods[0].GetComponent<BirdHouseFood>().SetFoodToEdible(randomBird, this);
        birdFoods.RemoveAt(0);
    }

    // Remove food from the list
    private void OnTriggerExit(Collider other)
    {
        if (birdFoods.Contains(other.gameObject))
        {
            birdFoods.Remove(other.gameObject);
        }
    }

    // Get all objects with the bird tag and add them to the list
    private void GetAllBirds()
    {
        allBirds.Clear();
        GameObject[] birds = GameObject.FindGameObjectsWithTag("Bird");
        for (int i = 0; i < birds.Length; i++)
        {
            allBirds.Add(birds[i].GetComponent<BirdBehaviour>());
        }
    }

    // Grab a random bird and let it return to a homebase on the foodplateau
    private void CallBird()
    {
        randomBird = allBirds[Random.Range(0, allBirds.Count - 1)];
        Transform foodPanel = null;

        // Set the homebase to the closests spot on the birdHousePlateau
        float dis = -1;
        for (int i = 0; i < completeBirdHouse.birdHousePlateau.plateauLandingSpots.Count; i++)
        {
            float newDis = Vector3.Distance(randomBird.transform.position, completeBirdHouse.birdHousePlateau.plateauLandingSpots[i].position);
            if (dis == -1 || newDis < dis)
            {
                dis = newDis;
                foodPanel = completeBirdHouse.birdHousePlateau.plateauLandingSpots[i];
            }
        }

        randomBird.homeTarget = foodPanel;
        randomBird.returnToBase = true;
        randomBird.baseTimer = 0;
    }

    // Start the unoccupie on this object
    public void StartUnoccupie()
    {
        StartCoroutine(UnOccupie());
    }

    // Unoccpie the object and check if there is a next object in the foodpanel
    private IEnumerator UnOccupie()
    {
        yield return new WaitForSeconds(0.5f);
        isOccupied = false;
        if (birdFoods.Count > 0)
        {
            SetOccupied();
        }
    }
}
