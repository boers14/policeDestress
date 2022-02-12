using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeactivateCollisionsOnGrab : MonoBehaviour
{
    // If the collider is on the child give it the tag: IsColliderOfObject
    [SerializeField]
    private bool grabCollidersFromChilderen = false;

    private List<Collider> allColliders = new List<Collider>();

    // Set variabels
    private void Start()
    {
        InitializeColliders();

        GetComponent<XRGrabInteractable>().onSelectEntered.AddListener(OnGrab);
        GetComponent<XRGrabInteractable>().onSelectExited.AddListener(OnDrop);
        GetComponent<XRGrabInteractable>().onSelectCanceled.AddListener(OnDrop);
    }

    // Set objects to triggers on grab
    private void OnGrab(XRBaseInteractor interactor)
    {
        EnableColliders(true);
    }

    // Untrigger objects on drop
    private void OnDrop(XRBaseInteractor interactor)
    {
        EnableColliders(false);
    }

    // Go throught all objects and set them to the desired trigger state
    private void EnableColliders(bool isTrigger)
    {
        for (int i = 0; i < allColliders.Count; i++)
        {
            allColliders[i].isTrigger = isTrigger;
        }
    }

    // Grab neccesary colliders
    private void InitializeColliders()
    {
        allColliders.Clear();

        if (grabCollidersFromChilderen)
        {
            // Look through all childeren using recursion
            LoopThroughtChilderen(transform);
        }
        else
        {
            // grab all collider on the object
            foreach (Collider c in GetComponents<Collider>())
            {
                allColliders.Add(c);
            }
        }
    }

    // Look throught all childeren and fill the colliders list with childeren with the tag IsColliderOfObject
    private void LoopThroughtChilderen(Transform child)
    {
        for (int i = 0; i < child.transform.childCount; i++)
        {
            if (child.transform.GetChild(i).tag == "IsColliderOfObject")
            {
                foreach (Collider c in child.transform.GetChild(i).GetComponents<Collider>())
                {
                    allColliders.Add(c);
                }
            }

            // Look throught all childeren of childeren
            LoopThroughtChilderen(child.transform.GetChild(i));
        }
    }
}
