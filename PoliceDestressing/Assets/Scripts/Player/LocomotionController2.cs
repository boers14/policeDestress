using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController2 : MonoBehaviour
{
    [SerializeField]
    private XRController leftTeleportRay = null, rightTeleportRay = null;

    [SerializeField]
    private InputHelpers.Button teleportActivationButton = InputHelpers.Button.None;

    [SerializeField]
    private float activationThreshold = 0.1f;

    [SerializeField]
    private XRRayInteractor leftInteractorRay = null, rightInteractorRay = null;

    private bool enableLeftTeleport = true, enableRightTeleport = true;

    private void Update()
    {
        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int index = 0;
        bool validTarget = false;

        // If its the lefthand check if the visual of the ray should be on or off
        if(leftTeleportRay)
        {
            bool isLeftInteractorRayHovering = leftInteractorRay.TryGetHitInfo(out pos, out norm, out index, out validTarget);
            leftTeleportRay.gameObject.SetActive(enableLeftTeleport && CheckIfActivated(leftTeleportRay) && !isLeftInteractorRayHovering);
        }

        // If its the righthand check if the visual of the ray should be on or off
        if (rightTeleportRay)
        {
            bool isRightInteractorRayHovering = rightInteractorRay.TryGetHitInfo(out pos, out norm, out index, out validTarget);
            rightTeleportRay.gameObject.SetActive(enableRightTeleport && CheckIfActivated(rightTeleportRay) && !isRightInteractorRayHovering);
        }
    }

    // Check if the teleport button is pressed
    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    }
}
