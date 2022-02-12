using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    // Set the player postion equal to the postion of this spawnposition object
    void Start()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPosition");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 positionSpawnPoint = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        player.transform.position = positionSpawnPoint;
    }
}
