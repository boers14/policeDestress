using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Eraser : MonoBehaviour
{
    public bool isGrabbed { get; set; } = false;

    // Set functions
    private void Start()
    {
        GetComponentInParent<XRGrabInteractable>().onSelectEntered.AddListener(Grabbed);
        GetComponentInParent<XRGrabInteractable>().onSelectExited.AddListener(UnGrab);
    }

    // Update grabbed when grabbed
    private void Grabbed(XRBaseInteractor interactor)
    {
        isGrabbed = true;
    }

    // Update grabbed when dropped
    private void UnGrab(XRBaseInteractor interactor)
    {
        isGrabbed = false;
    }
}
