using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToBirdHouseComponents : MonoBehaviour
{
    // Add this object to the list of birhouse objects in the birdhouse spawner
    private void Start()
    {
        BirdHouseSpawner birdHouseSpawner = GameObject.FindGameObjectWithTag("BirdHouseSpawner").GetComponent<BirdHouseSpawner>();
        birdHouseSpawner.AddObjectToBirdHouse(gameObject);
    }
}
