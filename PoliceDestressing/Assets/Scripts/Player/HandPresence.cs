using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    [SerializeField]
    private InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.None;

    [SerializeField]
    private GameObject handModelPrefab = null;

    private InputDevice targetDevice;
    private GameObject spawnedHandModel = null;
    private Animator handAnimator = null;

    private void Start()
    {
        Tryinitialize();
    }
    
    // Initialize the hand controller/ model
    private void Tryinitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        // Set the hand model for the controller 
        if (devices.Count > 0)
        {
            targetDevice = devices[0];            
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    private void UpdateHandAnimation()
    {
        // If the trigger is pushed, the animator will play.
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        // If the gripbutton is pushed, the animator will play.
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    private void Update()
    {
        // Initilize if the hand is not active else play the animations
        if (!targetDevice.isValid)
        {
            Tryinitialize();
        }
        else
        {
            UpdateHandAnimation();
        }
    }     
}
