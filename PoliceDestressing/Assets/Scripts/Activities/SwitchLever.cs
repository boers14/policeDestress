using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchLever : MonoBehaviour
{
    private bool canPerformFunction = true, canPlayTweenAgain = true, isPlayingTween = false, isGrabbed = false;

    [SerializeField]
    private AudioSource lever = null;

    [SerializeField]
    private GameObject trigger = null;

    [SerializeField]
    private UnityEvent onLeverSwitchEvent = null;

    private XROffsetGrabInteractable leverXROffsetGrabInteractable = null;

    // Set variables
    private void Start()
    {
        leverXROffsetGrabInteractable = lever.GetComponent<XROffsetGrabInteractable>();
    }

    // Reset all variables so that it can be grabbed and perform the given function again
    private void CheckIfCanPlayTweenAgain()
    {
        if (!canPlayTweenAgain)
        {
            // Only set if its not playing the tween and not being grabbed
            if (!isPlayingTween && !isGrabbed)
            {
                canPlayTweenAgain = true;
                leverXROffsetGrabInteractable.trackPosition = true;
                leverXROffsetGrabInteractable.trackRotation = true;
                canPerformFunction = true;

                // Play tween again if the lever is at the trigger
                if (lever.transform.rotation.x > 280)
                {
                    StartPlayingTween();
                }
            }
        }
    }

    // Invoke function and play sound if the lever collided with the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == trigger && canPerformFunction)
        {
            lever.Play();
            onLeverSwitchEvent.Invoke();
            canPerformFunction = false;
        }
    }

    // Set this function to OnSelectExited on XRGrabInteractible!
    // Checks whether it can play the tween again, if that is the case it plays the tween
    public void StartPlayingTween()
    {
        isGrabbed = false;
        CheckIfCanPlayTweenAgain();
        if (canPlayTweenAgain)
        {
            canPlayTweenAgain = false;
            isPlayingTween = true;
            iTween.RotateBy(lever.gameObject, iTween.Hash("x", 0.2f, "time", 0.2f, "easetype", iTween.EaseType.easeInOutSine,
                "oncomplete", "SetIsNotPlayingTween", "oncompletetarget", gameObject));

            // Disable hand tracking functions
            leverXROffsetGrabInteractable.trackPosition = false;
            leverXROffsetGrabInteractable.trackRotation = false;
        }
    }

    // Set this function to OnSelectEntered on XRGrabInteractible!
    public void IsGrabbed()
    {
        isGrabbed = true;
    }

    // At end of tween checks if it can rest the tween
    private void SetIsNotPlayingTween()
    {
        isPlayingTween = false;
        CheckIfCanPlayTweenAgain();
    }
}
