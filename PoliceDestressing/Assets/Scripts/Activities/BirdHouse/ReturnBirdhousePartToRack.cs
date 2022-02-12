using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnBirdhousePartToRack : MonoBehaviour
{
    private bool moveToBasePos = false, grabbed = false;

    private Vector3 startPos = Vector3.zero, startRot = Vector3.zero;

    private new Rigidbody rigidbody = null;

    // Set variables
    private void Start()
    {
        startPos = transform.position;
        startRot = transform.eulerAngles;
        GetComponent<XRGrabInteractable>().onSelectEntered.AddListener(isGrabbed);
        GetComponent<XRGrabInteractable>().onSelectExited.AddListener(unGrabbed);
        rigidbody = GetComponent<Rigidbody>();
    }

    // Stop object from tweening and activate gravity again
    public void StopReturnToBase()
    {
        moveToBasePos = false;
        rigidbody.useGravity = true;
        iTween.Stop(gameObject);
    }

    // Start tween if not grabbed and is colliding with the ground
    private void StartReturnToRack()
    {
        if (!grabbed && moveToBasePos)
        {
            MoveToTarget();
            moveToBasePos = false;
        }
    }

    // Turn of gravity and start tween to original position
    private void MoveToTarget()
    {
        rigidbody.useGravity = false;
        iTween.MoveTo(gameObject, iTween.Hash("position", startPos, "time", 1f, "easetype",
                                                iTween.EaseType.linear));
        iTween.RotateTo(gameObject, iTween.Hash("rotation", startRot, "time", 1f, 
                                                "easetype", iTween.EaseType.linear,
                                                "oncomplete", "OnDestinationArrived", "oncompletetarget", gameObject));
    }

    // Set end stats of tween
    private void OnDestinationArrived()
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = startPos;
        transform.eulerAngles = startRot;
    }

    // Check if it should tween on collision of floor
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer ("Floor"))
        {
            moveToBasePos = true;
            StartReturnToRack();
        }
    }

    // Deactivate floor start tween condition
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            moveToBasePos = false;
        }
    }

    // Set to being grabbed and stop the tween
    private void isGrabbed(XRBaseInteractor interactor)
    {
        grabbed = true;
        iTween.Stop(gameObject);
    }

    // Set to not being grabbed, turn on gravity and check if it should tween
    private void unGrabbed(XRBaseInteractor interactor)
    {
        grabbed = false;
        rigidbody.useGravity = true;
        StartReturnToRack();
    }
}
