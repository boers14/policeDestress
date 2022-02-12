using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class isEnabled : MonoBehaviour
{
    private XRSocketInteractor socket = null;

    // Set variable
    private void Start()
    {
        socket = gameObject.GetComponent<XRSocketInteractor>();
    }

    // Enable socket
    private void OnTriggerStay(Collider other)
    {        
        if (other.name == gameObject.name)
        {            
            socket.enabled = true;
        }       
    }

    // Disable socket
    private void OnTriggerExit(Collider other)
    {
        if (other.name == gameObject.name)
        {            
            socket.enabled = false;
        }
    }
}
