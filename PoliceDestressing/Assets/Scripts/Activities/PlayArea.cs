using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayArea : MonoBehaviour
{
    [SerializeField]
    private Collider playArea = null;

    private Vector3 startingPos = Vector3.zero, startingRotation = Vector3.zero;

    private bool teleportBack = false, isGrabbed = false, isPlayingTween = false, isInPlayArea = true;

    [SerializeField]
    private float distanceFactor = 0.3f;

    private new Rigidbody rigidbody = null;

    // Set variables
    private void Start()
    {
        startingPos = transform.position;
        startingRotation = transform.eulerAngles;

        GetComponent<XRGrabInteractable>().onSelectEntered.AddListener(IsGrabbed);
        GetComponent<XRGrabInteractable>().onSelectExited.AddListener(UnGrab);

        rigidbody = GetComponent<Rigidbody>();
    }

    // Check whether the object should play the tween
    private void CheckIfReturnToPos()
    {
        if (teleportBack && !isGrabbed && !isPlayingTween)
        {
            // based on distance the object will perform a longer or shorter tween
            float time = Vector3.Distance(transform.position, startingPos) * distanceFactor;
            iTween.MoveTo(gameObject, iTween.Hash("position", startingPos, "time", time, "easetype", iTween.EaseType.linear));
            iTween.RotateTo(gameObject, iTween.Hash("rotation", startingRotation, "time", time, "easetype", iTween.EaseType.linear,
                "oncomplete", "TurnGravityBackOn", "oncompletetarget", gameObject));
            teleportBack = false;
            rigidbody.useGravity = false;
            isPlayingTween = true;
        }
    }

    // Turn on gravity of object as well as restting the velocity
    private void TurnGravityBackOn()
    {
        rigidbody.useGravity = true;
        rigidbody.velocity = Vector3.zero;
        isPlayingTween = false;
    }

    // Set the object to being in the play area
    private void OnTriggerEnter(Collider other)
    {
        if (other == playArea)
        {
            teleportBack = false;
            isInPlayArea = true;
        }
    }

    // If the object is not being grabbed and left the play area, play the tween
    private void OnTriggerExit(Collider other)
    {
        if (other == playArea)
        {
            teleportBack = true;
            isInPlayArea = false;
            CheckIfReturnToPos();
        }
    }

    // If the object is playing the tween, stop playing the tween and set to being grabbed
    private void IsGrabbed(XRBaseInteractor interactor)
    {
        isGrabbed = true;
        if (isPlayingTween)
        {
            if (!isInPlayArea)
            {
                teleportBack = true;
            }
            iTween.Stop(gameObject);
            TurnGravityBackOn();
        }
    }

    // If the object is not being grabbed and left the play area, play the tween.
    private void UnGrab(XRBaseInteractor interactor)
    {
        isGrabbed = false;
        rigidbody.useGravity = true;
        CheckIfReturnToPos();
    }
}
