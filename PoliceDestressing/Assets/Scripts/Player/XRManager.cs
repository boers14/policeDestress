using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRManager : MonoBehaviour
{
    // Make sure the XR manager exists in every scene and if one already exist that it destroys itself
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("XRManager").Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }  
}
