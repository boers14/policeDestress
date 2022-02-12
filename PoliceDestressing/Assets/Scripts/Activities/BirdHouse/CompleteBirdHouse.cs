using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CompleteBirdHouse : MonoBehaviour
{
    public bool birdHouseCompleted { get; set; } = false;

    private int amountOfColliders = 0, amountOfCollidersSatisfied = 0;

    private GameObject workBench = null;

    [System.NonSerialized]
    public BirdHousePlateau birdHousePlateau = null;

    [SerializeField]
    private int amountOfBirdHouseFood = 0;

    [SerializeField]
    private GameObject birdFood = null;

    [SerializeField]
    private new ParticleSystem particleSystem = null;

    private List<GameObject> allPartsToConnect = new List<GameObject>();

    private List<Vector3> scales = new List<Vector3>();

    private Vector3 particlePos = Vector3.zero;

    // Set variables
    private void Start()
    {
        workBench = GameObject.FindGameObjectWithTag("WorkBench");
        particlePos = particleSystem.transform.position;
    }

    // Check if bird house is complete
    public void CheckIfBirdHouseIsCompleted()
    {
        if (!birdHouseCompleted)
        {
            // Reset counters
            amountOfColliders = 0;
            amountOfCollidersSatisfied = 0;

            CountAllCompletedColliders(transform);

            if (amountOfColliders == amountOfCollidersSatisfied)
            {
                // Start completion
                StartCoroutine(StartBirdHouseCompletion());
            }
            else
            {
                birdHouseCompleted = false;
            }
        }
    }

    // Count all childs with HologrampPart  +  extra count for colliders with a connected birdhouse part
    private void CountAllCompletedColliders(Transform child)
    {
        // Loop throught children
        for (int i = 0; i < child.childCount; i++)
        {
            if (child.GetChild(i).GetComponent<HologramPart>() != null)
            {
                amountOfColliders++;
                if (child.GetChild(i).GetComponent<HologramPart>().connectedBirdHousePart != null)
                {
                    amountOfCollidersSatisfied++;
                }
            }

            // Loop throught childeren of children
            CountAllCompletedColliders(child.GetChild(i));
        }
    }

    // Complete the birdhouse
    private IEnumerator StartBirdHouseCompletion()
    {
        allPartsToConnect.Clear();
        birdHouseCompleted = true;
        // Loop trought all childeren to get the connected birdhouse parts
        GetAllConnectedParts(transform);
        
        // Deactivate grabinteractebles
        for (int i = 0; i < allPartsToConnect.Count; i++)
        {
            DeactivateGrabInteractables(allPartsToConnect[i].transform);
            DeactivateAllGrabInteractables(allPartsToConnect[i].transform);
        }

        yield return new WaitForSeconds(0.5f);

        CompleteTheBirdHouse();
    }

    // Complete the birdhouse
    private void CompleteTheBirdHouse()
    {
        name = "Complete birdhouse";
        gameObject.layer = 3;
        transform.localScale = Vector3.one;

        // Destroy all hologram parts
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).tag == "Hologram")
            {
                Transform child = transform.GetChild(i);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        // Make a list of copied game objects
        List<GameObject> copiedGameObjects = GetCopiedGameObjects();
        // Parent these gameobjects, reset scale and destoy rigibodies
        for (int i = 0; i < copiedGameObjects.Count; i++)
        {
            copiedGameObjects[i].transform.SetParent(transform);
            copiedGameObjects[i].transform.localScale = scales[i];
            CheckForRigidBody(copiedGameObjects[i].transform);
            DestroyAllRigidBodiesAndGrabInteractables(copiedGameObjects[i].transform);
        }

        // Find the birdhouse foodspot and birdhouse plateau
        SetPlateauAndFoodPanel(transform);

        gameObject.AddComponent<Rigidbody>();

        float addedY = 0;

        // Spawn birdfood
        for (int i = 0; i < amountOfBirdHouseFood; i++)
        {
            Instantiate(birdFood, workBench.transform.position + new Vector3(0, birdFood.transform.localScale.y / 100 + addedY, 0),
                Quaternion.identity);
            addedY += birdFood.transform.localScale.y / 100;
        }

        particleSystem.transform.position = particlePos;
        particleSystem.Play();
    }

    // Loop trought all childeren to get the connected birdhouse parts
    private void GetAllConnectedParts(Transform child)
    {
        for (int i = 0; i < child.childCount; i++)
        {
            // Set to connected birdhouse layer
            child.GetChild(i).gameObject.layer = 3;
            if (child.GetChild(i).GetComponent<HologramPart>() != null)
            {
                // Add scales to list, add connected part to list, reset position
                scales.Add(child.GetChild(i).GetComponent<HologramPart>().connectedBirdHousePart.transform.localScale);
                allPartsToConnect.Add(child.GetChild(i).GetComponent<HologramPart>().connectedBirdHousePart);
                child.GetChild(i).GetComponent<HologramPart>().connectedBirdHousePart.transform.rotation = child.GetChild(i).parent.rotation;
                child.GetChild(i).GetComponent<HologramPart>().connectedBirdHousePart.transform.position = child.GetChild(i).parent.position;
            }

            // Repeat process for all childs
            GetAllConnectedParts(child.GetChild(i));
        }
    }

    // Check for rigidbodies and grab interactebles in all childs
    private void DestroyAllRigidBodiesAndGrabInteractables(Transform child)
    {
        for (int i = 0; i < child.childCount; i++)
        {
            CheckForRigidBody(child.GetChild(i));
            DestroyAllRigidBodiesAndGrabInteractables(child.GetChild(i));
        }
    }

    // Remove all availeble grabinteractebles and rigibodies and set objects to the correct layer
    private void CheckForRigidBody(Transform obj)
    {
        obj.gameObject.layer = 3;
        if (obj.GetComponent<Rigidbody>() != null)
        {
            if (obj.GetComponent<XRGrabInteractable>() != null)
            {
                if (obj.GetComponent<DeactivateCollisionsOnGrab>() != null)
                {
                    Destroy(obj.GetComponent<DeactivateCollisionsOnGrab>());
                }
                Destroy(obj.GetComponent<XRGrabInteractable>());
            }
            Destroy(obj.GetComponent<Rigidbody>());
        }
    }

    // Check for in all childs for grabinteractables
    private void DeactivateAllGrabInteractables(Transform child)
    {
        for (int i = 0; i < child.childCount; i++)
        {
            DeactivateGrabInteractables(child.GetChild(i));
            DeactivateAllGrabInteractables(child.GetChild(i));
        }
    }

    // Deactivate all grab functions so the objects wont move anymore when grabbing or dropping
    private void DeactivateGrabInteractables(Transform obj)
    {
        if (obj.GetComponent<XRGrabInteractable>() != null)
        {
            obj.GetComponent<XRGrabInteractable>().trackPosition = false;
            obj.GetComponent<XRGrabInteractable>().trackRotation = false;
            obj.GetComponent<XRGrabInteractable>().throwOnDetach = false;
            obj.GetComponent<XRGrabInteractable>().gravityOnDetach = false;
        }
    }

    // Copy the allparts to connect list
    private List<GameObject> GetCopiedGameObjects()
    {
        List<GameObject> copiedGameObject = new List<GameObject>();
        BirdHouseSpawner birdHouseSpawner = GameObject.FindGameObjectWithTag("BirdHouseSpawner").GetComponent<BirdHouseSpawner>();

        for (int i = 0; i < allPartsToConnect.Count; i++)
        {
            GameObject copy = Instantiate(allPartsToConnect[i], allPartsToConnect[i].transform.position, allPartsToConnect[i].transform.rotation);
            // Add object list to the complete birshouse list of the birdhouse spawner and deactivate old objects
            birdHouseSpawner.AddObjectToBirdHouse(gameObject);
            copiedGameObject.Add(copy);
            allPartsToConnect[i].SetActive(false);
        }

        return copiedGameObject;
    }

    // Find the birdhouse foodspot and birdhouse plateau
    private void SetPlateauAndFoodPanel(Transform child)
    {
        for (int i = 0; i < child.childCount; i++)
        {
            // remove all ReturnBirdhousePartToRacks 
            if (child.GetChild(i).GetComponent<ReturnBirdhousePartToRack>() != null)
            {
                Destroy(child.GetChild(i).GetComponent<ReturnBirdhousePartToRack>());
            }

            // Set all colliders to not be a trigger
            if (child.GetChild(i).GetComponent<Collider>() != null)
            {
                child.GetChild(i).GetComponent<Collider>().isTrigger = false;
            }
            
            if (child.GetChild(i).GetComponent<BirdHousePlateau>() != null)
            {
                birdHousePlateau = child.GetChild(i).GetComponent<BirdHousePlateau>();
                // Fill the plateauLandingSpots in BirdHousePlateau
                for (int j = 0; j < birdHousePlateau.transform.childCount; j++)
                {
                    if (birdHousePlateau.transform.GetChild(j).tag == "BirdLandingSpot")
                    {
                        birdHousePlateau.plateauLandingSpots.Add(birdHousePlateau.transform.GetChild(j));
                    }
                }
            }

            // Set complete birdhouse and make a trigger of the collider of the BirdHouseFoodPanel
            if (child.GetChild(i).GetComponent<BirdHouseFoodPanel>() != null)
            {
                child.GetChild(i).GetComponent<BirdHouseFoodPanel>().completeBirdHouse = this;
                child.GetChild(i).GetComponent<Collider>().isTrigger = true;
            }


            // Check next child
            SetPlateauAndFoodPanel(child.GetChild(i));
        }
    }
}
