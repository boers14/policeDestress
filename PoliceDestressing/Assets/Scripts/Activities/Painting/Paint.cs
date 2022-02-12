using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    // Check position of paint
    public float upwardLayer { get; set; } = 0;

    // To diffirentiate paint
    public int paintLayer { get; set; } = 0;

    // Position for raycasting
    public Transform rayCastPos = null;
}
