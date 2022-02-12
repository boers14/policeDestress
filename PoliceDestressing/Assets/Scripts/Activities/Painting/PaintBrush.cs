using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintBrush : MonoBehaviour
{
    private PaintCanvas paintCanvas = null;

    [SerializeField]
    private float brushSize = 0, rayDist = 2;

    [SerializeField]
    private LayerMask layer;

    [SerializeField]
    private ChangePaintMaterial changeMaterial = null;

    [SerializeField]
    private BoxCollider brushBoxCollider = null;

    [SerializeField]
    private Transform actualBrush = null;

    public int brushIndex = 0;

    public bool canPaint = false;

    private bool isGrabbed = false;

    private Vector3 hitPoint = Vector3.zero;

    private List<Vector3> extraRays = new List<Vector3>(), primaryRays = new List<Vector3>();

    private int sign = 1;

    private float minDist = 0;

    // Set variables
    private void Start()
    {
        GetComponentInParent<XRGrabInteractable>().onSelectEntered.AddListener(Grabbed);
        GetComponentInParent<XRGrabInteractable>().onSelectExited.AddListener(UnGrab);

        // Make primary rays list
        for (int i = 0; i < 5; i++)
        {
            Vector3 extraDirection = Vector3.zero;
            if (i < 3 && i > 0)
            {
                extraDirection = CalculateExtraDirection("x", 2.5f);
            }
            else if (i > 0)
            {
                extraDirection = CalculateExtraDirection("y", 1.55f);
            }
            primaryRays.Add(extraDirection);
        }

        // Make secondary rays list
        for (int i = 0; i < 4; i++)
        {
            Vector3 extraDirection = Vector3.zero;
            if (i < 2)
            {
                extraDirection = CalculateExtraDirection("x", 4);
            }
            else
            {
                extraDirection = CalculateExtraDirection("y", 2.5f);
            }
            extraRays.Add(extraDirection);
        }
    }

    // Check if can paint, if so paint
    private void FixedUpdate()
    {
        if (canPaint && changeMaterial.hasColor && isGrabbed)
        {
            bool hitPaint = CheckIfHitPaint();
            if (hitPaint)
            {
                StartPainting();
            }
        }
    }

    // check if one of the primary rays hit a canvas
    private bool CheckIfHitPaint()
    {
        bool hitPaint = false;
        minDist = 10000;

        for (int i = 0; i < primaryRays.Count; i++)
        {
            if (Physics.Raycast(transform.position, transform.forward + primaryRays[i], out RaycastHit primaryRay, rayDist, layer))
            {
                if (primaryRay.transform.GetComponent<PaintCanvas>() != null)
                {
                    // This is a check for hitting the front of an object
                    if (primaryRay.normal - primaryRay.transform.up == Vector3.zero)
                    {
                        hitPaint = true;
                        // Calculate a minimum distance to overcome
                        minDist = CheckDist(minDist, primaryRay.distance, primaryRay.point, primaryRay);
                    }
                }
            }
        }

        return hitPaint;
    }

    // Spawn paint on hit canvas
    private void StartPainting()
    {
        // Check if one of the extra rays has a smaller distance
        for (int i = 0; i < extraRays.Count; i++)
        {
            if (Physics.Raycast(transform.position, transform.forward + extraRays[i], out RaycastHit extraRay, rayDist, layer))
            {
                if (extraRay.transform.GetComponent<PaintCanvas>() != null)
                {
                    // This is a check for hitting the front of an object
                    if (extraRay.normal - extraRay.transform.up == Vector3.zero)
                    {
                        minDist = CheckDist(minDist, extraRay.distance, extraRay.point, extraRay);
                    }
                }
            }
        }
        
        paintCanvas.SpawnPaint(hitPoint, changeMaterial.color, brushSize);
    }

    // Calculate the added vector based on the axis and the strenght of the extra direction
    private Vector3 CalculateExtraDirection(string axis, float extraDirection)
    {
        Vector3 newDirection = Vector3.zero;
        switch (axis)
        {
            case "x":
                newDirection.x += (brushBoxCollider.size.x * actualBrush.localScale.x * extraDirection) * sign;
                break;
            case "y":
                newDirection.y += (brushBoxCollider.size.y * actualBrush.localScale.y * extraDirection) * sign;
                break;
            default:
                print("no axis");
                break;
        }
        sign *= -1;

        return newDirection;
    }

    // Check if the distance is smaller then current minsDist
    private float CheckDist(float minDist, float otherDist, Vector3 otherHitPoint, RaycastHit hit)
    {
        // The offset (+ 0.05f) is because else extra rays felt too clunky
        if (otherDist + 0.05f < minDist)
        {
            minDist = otherDist;
            hitPoint = otherHitPoint;
            paintCanvas = hit.transform.GetComponent<PaintCanvas>();
        }

        return minDist;
    }

    // Set to grabbed
    private void Grabbed(XRBaseInteractor interactor)
    {
        isGrabbed = true;
    }

    // Set to dropped
    private void UnGrab(XRBaseInteractor interactor)
    {
        isGrabbed = false;
    }
}
