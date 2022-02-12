using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeCanvasSize : MonoBehaviour
{
    [SerializeField]
    private List<BoxCollider> canvasses = new List<BoxCollider>();

    private List<Vector3> originalSize = new List<Vector3>();

    [SerializeField]
    private float size = 4.97f;

    // Set variables
    private void Start()
    {
        for (int i = 0; i < canvasses.Count; i++)
        {
            originalSize.Add(canvasses[i].size);
        }

        GetComponent<XRGrabInteractable>().onSelectEntered.AddListener(ChangeSizeOfCanvas);
    }

    // Change the hitbox of the canvas, based on the original size and the size reduction
    private void ChangeSizeOfCanvas(XRBaseInteractor interactor)
    {
        for (int i = 0; i < canvasses.Count; i++)
        {
            Vector3 newSize = Vector3.zero;
            newSize.x = originalSize[i].x - size;
            newSize.z = originalSize[i].z - size;
            newSize.y = originalSize[i].y;
            canvasses[i].size = newSize;
        }
    }
}
