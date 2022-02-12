using UnityEngine;
using System.Collections;

public class ShowPaintTip : MonoBehaviour
{
    [SerializeField]
    private GameObject paint = null;

    // turn off paint
    private void Start()
    {        
        paint.SetActive(false);
    }

    // On colorchange collision set paint active
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ColorChange"))
        {
            paint.SetActive(true);            
        }       
    }  
}