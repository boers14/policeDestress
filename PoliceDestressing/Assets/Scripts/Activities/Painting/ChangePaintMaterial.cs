using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangePaintMaterial : MonoBehaviour
{    
    [System.NonSerialized]
    public Color32 color = Color.white;

    [SerializeField]
    private List<PaintCanvas> canvasses = new List<PaintCanvas>();

    public bool hasColor { get; set; } = false;

    private new Renderer renderer = null;

    // Set variable
    private void Start()
    {
        renderer = GetComponentInParent<Renderer>();
    }

    // Change the color of the paint to the color of the colliding object and state it has color
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ColorChange"))
        {
            Renderer paintRenderer = other.GetComponent<Renderer>();
            color = paintRenderer.material.color;
            renderer.material.color = paintRenderer.material.color;
            hasColor = true;

            // Change paint layer of canvasses
            for (int i = 0; i < canvasses.Count; i++)
            {
                canvasses[i].paintLayer++;
            }
        }
    }
}