using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramPart : MonoBehaviour
{
    [SerializeField]
    private GameObject socket = null, objectToChange = null;

    [SerializeField]
    private Material hoverMaterial = null, oldMaterial = null;

    [SerializeField]
    private CompleteBirdHouse completeBirdHouse = null;

    [System.NonSerialized]
    public GameObject connectedBirdHousePart = null;

    private Renderer objectToChangeRenderer = null;

    // Deactivate socket and set variable
    private void Start()
    {
        socket.SetActive(false);
        objectToChangeRenderer = objectToChange.GetComponent<Renderer>();
    }

    // Activate socket and check whether bird huose is complete
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == other.gameObject.name)
        {
            objectToChangeRenderer.material = hoverMaterial;
            socket.SetActive(true);
            connectedBirdHousePart = other.transform.parent.gameObject;
            completeBirdHouse.CheckIfBirdHouseIsCompleted();
            // Stop tween
            if (other.GetComponent<ReturnBirdhousePartToRack>() != null)
            {
                other.GetComponent<ReturnBirdhousePartToRack>().StopReturnToBase();
            }
            else if (other.GetComponentInParent<ReturnBirdhousePartToRack>() != null)
            {
                other.GetComponentInParent<ReturnBirdhousePartToRack>().StopReturnToBase();
            }
        }
    }

    // Deactivate socket
    private void OnTriggerExit(Collider other)
    {
        if (gameObject.name == other.gameObject.name)
        {
            socket.SetActive(false);
            objectToChangeRenderer.material = oldMaterial;
            connectedBirdHousePart = null;
        }
    }
}
