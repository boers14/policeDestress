using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintColor : MonoBehaviour
{
    [SerializeField]
    private Color32 selectedColor = Color.white;

    // Change color to selected color
    private void Start()
    {
        GetComponent<Renderer>().material.color = selectedColor;
    }
}
