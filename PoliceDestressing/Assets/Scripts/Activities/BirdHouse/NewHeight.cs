using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHeight : MonoBehaviour
{
    [SerializeField]
    private float newHight = 0;

    //Adjust height to newHeight
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, newHight, transform.position.z);
    }
}
