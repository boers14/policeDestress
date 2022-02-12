using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject paintPrefab = null;

    private List<GameObject> activePaintSpots = new List<GameObject>(), disabledPaintSpots = new List<GameObject>();

    [SerializeField]
    private float upwardLayer = 0.03f, addedLayer = 0.001f, disBetweenPaint = 0.005f;

    public int paintLayer { get; set; } = 0;

    private GameObject prevPaintSpot = null;

    private bool firstTouch = true;

    private Vector3 lastPos = Vector3.zero;

    [SerializeField]
    private LayerMask mask;

    private int prevBrushIndex = -1;

    // Fill the object pool with paint
    private void Start()
    {
        AddPaintSpotsToObjectPool(500);
    }

    // Adjust paint layer based on the previous brush and make the brush able to paint
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brush"))
        {
            other.GetComponentInChildren<PaintBrush>().canPaint = true;
            if (prevBrushIndex != -1 && prevBrushIndex != other.GetComponentInChildren<PaintBrush>().brushIndex)
            {
                paintLayer++;
            }
            prevBrushIndex = other.GetComponentInChildren<PaintBrush>().brushIndex;
        }
    }

    // Set the brush to not being able to paint
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Brush"))
        {
            other.GetComponentInChildren<PaintBrush>().canPaint = false;
            firstTouch = true;
        }
    }

    // remove paint if in contact with the eraser
    private void OnTriggerStay(Collider other)
    {
        RemovePaint(other);
    } 
    
    // if the eraser is grabbed remove paint that is in the collision course of the eraser
    private void RemovePaint(Collider other)
    {
        if (other.CompareTag("Eraser"))
        {
            if (other.GetComponent<Eraser>().isGrabbed)
            {
                for (int i = activePaintSpots.Count - 1; i >= 0; i--)
                {
                    MeshRenderer mesh = activePaintSpots[i].GetComponent<MeshRenderer>();
                    if (other.transform.position.x >= activePaintSpots[i].transform.position.x - mesh.bounds.size.x / 2 &&
                            other.transform.position.x <= activePaintSpots[i].transform.position.x + mesh.bounds.size.x / 2 &&
                            other.transform.position.y >= activePaintSpots[i].transform.position.y - mesh.bounds.size.y / 2 &&
                            other.transform.position.y <= activePaintSpots[i].transform.position.y + mesh.bounds.size.y / 2)
                    {
                        // remove paint from active paint list and reset its stats
                        activePaintSpots[i].GetComponent<Paint>().upwardLayer = 0;
                        activePaintSpots[i].SetActive(false);
                        disabledPaintSpots.Add(activePaintSpots[i]);
                        activePaintSpots.RemoveAt(i);
                    }
                }
            }
        }
    }

    // Spawn paint from the object pool
    public void SpawnPaint(Vector3 position, Color32 color, float brushSize)
    {
        // check whether the brush moved enough from its last position
        if (position.x >= lastPos.x - disBetweenPaint && position.x <= lastPos.x + disBetweenPaint 
            && position.y >= lastPos.y - disBetweenPaint && position.y <= lastPos.y + disBetweenPaint && !firstTouch)
        { 
            return; 
        }

        // Add new paint spots to object pool
        if (disabledPaintSpots.Count == 0)
        {
            AddPaintSpotsToObjectPool(5);
        }

        // Instantiate paint
        GameObject go = disabledPaintSpots[0];
        disabledPaintSpots.RemoveAt(0);
        go.SetActive(true);

        go.transform.position = position;
        go.transform.rotation = transform.rotation;

        Vector3 size = Vector3.one * brushSize;
        go.transform.localScale = size;
        go.GetComponent<Renderer>().material.color = color;

        // Check if its not the first time editing the canvas after switching color/ brush
        if (prevPaintSpot != null && !firstTouch)
        {
            // Check if paint is colliding with the other paint spot
            bool changeSize = true;
            MeshRenderer prevPaintMesh = prevPaintSpot.GetComponent<MeshRenderer>();
            MeshRenderer paintMesh = go.GetComponent<MeshRenderer>();
            if (go.transform.position.x + paintMesh.bounds.size.x / 2 >= prevPaintSpot.transform.position.x - prevPaintMesh.bounds.size.x / 2 &&
                go.transform.position.x - paintMesh.bounds.size.x / 2 <= prevPaintSpot.transform.position.x + prevPaintMesh.bounds.size.x / 2 &&
                go.transform.position.y + paintMesh.bounds.size.y / 2 >= prevPaintSpot.transform.position.y - prevPaintMesh.bounds.size.y / 2 &&
                go.transform.position.y - paintMesh.bounds.size.y / 2 <= prevPaintSpot.transform.position.y + prevPaintMesh.bounds.size.y / 2)
            {
                changeSize = false;
            }

            // edit size to collide with the other paint spot
            if (changeSize)
            {
                // rotate towards the other paintspot
                CalulateRotationOfPaint(go);

                // based on distance adjust the size
                float dist = Vector3.Distance(go.transform.position, prevPaintSpot.transform.position);
                Vector3 scale = go.transform.localScale;
                scale.z += dist / (paintMesh.bounds.size.z / brushSize);
                go.transform.localScale = scale;

                // check if the size isnt going over the canvas
                if (!Physics.Raycast(go.GetComponent<Paint>().rayCastPos.position, -go.transform.up, out RaycastHit hit1, Mathf.Infinity, mask))
                {
                    // adjust size to fit on canvas
                    int downScaleAmount = 0;
                    while (!Physics.Raycast(go.GetComponent<Paint>().rayCastPos.position, -go.transform.up, out RaycastHit hit,
                        Mathf.Infinity, mask))
                    {
                        scale.z -= disBetweenPaint;
                        go.transform.localScale = scale;
                        downScaleAmount++;
                        if (downScaleAmount >= 10000)
                        {
                            break;
                        }
                    }

                    // Remove paint if it takes too much change in size. it likely bugged.
                    if (downScaleAmount >= 10000)
                    {
                        go.SetActive(false);
                        disabledPaintSpots.Add(go);
                        return;
                    }
                }
            }
        }

        if (firstTouch)
        {
            firstTouch = false;
        }

        // Change layer of paint, set layer to the base added layer
        float extraUpwardLayer = addedLayer;
        float highestUpwardLayer = 0;

        // Check if the paintlayer is not the same of colliding paint
        for (int i = activePaintSpots.Count - 1; i >= 0; i--)
        {
            if (activePaintSpots[i].GetComponent<Paint>().paintLayer != paintLayer)
            {
                MeshRenderer mesh = activePaintSpots[i].GetComponent<MeshRenderer>();
                if (go.transform.position.x >= activePaintSpots[i].transform.position.x - mesh.bounds.size.x / 2 &&
                        go.transform.position.x <= activePaintSpots[i].transform.position.x + mesh.bounds.size.x / 2 &&
                        go.transform.position.y >= activePaintSpots[i].transform.position.y - mesh.bounds.size.y / 2 &&
                        go.transform.position.y <= activePaintSpots[i].transform.position.y + mesh.bounds.size.y / 2)
                {
                    // Adjust the upward layer of the paint to the highest upward layer of colliding paints
                    if (activePaintSpots[i].GetComponent<Paint>().upwardLayer > highestUpwardLayer)
                    {
                        highestUpwardLayer = activePaintSpots[i].GetComponent<Paint>().upwardLayer;
                    }
                }
            }
        }

        // if there are no colliding paints, make it the base upward layer
        if (highestUpwardLayer == 0)
        {
            extraUpwardLayer = upwardLayer;
        }

        // place the paint upwards based on layer
        extraUpwardLayer += highestUpwardLayer;
        go.transform.position += go.transform.up * extraUpwardLayer;
        go.GetComponent<Paint>().upwardLayer = extraUpwardLayer;
        go.GetComponent<Paint>().paintLayer = paintLayer;

        activePaintSpots.Add(go);
        prevPaintSpot = go;
        lastPos = position;
    }

    // Calculate rotation of paint based on the previous pain spot
    private void CalulateRotationOfPaint(GameObject currPaintSpot)
    {
        Vector3 ownPos = new Vector3(currPaintSpot.transform.position.x, currPaintSpot.transform.position.y, currPaintSpot.transform.position.z);
        float angleRad = Mathf.Atan2(lastPos.z - ownPos.z, lastPos.y - ownPos.y);
        float angleDeg = ((180 / Mathf.PI) * angleRad);

        Vector3 heading = lastPos - currPaintSpot.transform.position;
        float dot = Vector3.Dot(heading, currPaintSpot.transform.right);
        if (dot <= 0)
        {
            angleDeg += 10;
        }

        currPaintSpot.transform.Rotate(0, angleDeg, 0, Space.Self);
    }

    // This function is added to the lever
    // Disable all paint and add them to the disabled paint list. Reset the paintlayer.
    public void ClearPaint()
    {
        for (int i = 0; i < activePaintSpots.Count; i++)
        {
            activePaintSpots[i].GetComponent<Paint>().upwardLayer = 0;
            activePaintSpots[i].SetActive(false);
            disabledPaintSpots.Add(activePaintSpots[i]);
        }
        activePaintSpots.Clear();

        paintLayer = 0;
    }

    // Add new paint to the object pool
    private void AddPaintSpotsToObjectPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject paint = Instantiate(paintPrefab, new Vector3(-100, -100, -100), Quaternion.identity);
            paint.SetActive(false);
            disabledPaintSpots.Add(paint);
        }
    }
}


   
