using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrackerChecker : MonoBehaviour
{
    public bool isDone = false;

    private List<GameObject> listOfCollidingStones = new List<GameObject>();

    [SerializeField]
    private Color32 colorToTransformInto = Color.cyan;

    private Material ownMaterial = null;

    [System.NonSerialized]
    public new ParticleSystem particleSystem = null;

    private float alpaTweenTime = 0.5f;

    private Color32 startTweenColor = Color.white;

    [System.NonSerialized]
    public Color32 endTweenColor = Color.white;

    // Set variables and make sure it is not visible
    private void Start()
    {
        ownMaterial = GetComponent<Renderer>().material;
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
        ChangeMaterialAlpha(0, true, false);
    }

    // On collision enter of a stone change the alpha and track the progress
    private void OnCollisionEnter(Collision collision)
    {
        if (!transform.parent.GetComponent<TrackStoneMappingProgress>().completedThePattern)
        {
            if (collision.transform.tag == "StoneForMapping")
            {
                if (listOfCollidingStones.Count == 0)
                {
                    ChangeMaterialAlpha(255);
                    isDone = true;
                    transform.parent.GetComponent<TrackStoneMappingProgress>().TrackProgress();
                }
                collision.rigidbody.velocity = Vector3.zero;
                listOfCollidingStones.Add(collision.gameObject);
            }
        }
    }

    // On collision exit of a stone check if should change the alpha and track the progress
    private void OnCollisionExit(Collision collision)
    {
        if (!transform.parent.GetComponent<TrackStoneMappingProgress>().completedThePattern)
        {
            if (collision.transform.tag == "StoneForMapping")
            {
                listOfCollidingStones.Remove(collision.gameObject);
                if (listOfCollidingStones.Count == 0)
                {
                    ChangeMaterialAlpha(0);
                    isDone = false;
                    transform.parent.GetComponent<TrackStoneMappingProgress>().TrackProgress();
                }
            }
        }
    }

    // Change the alpha with a tween or instantly. Color can also instantly be adjusted
    private void ChangeMaterialAlpha(byte alphaValue, bool changeColor = false, bool tweenAlpha = true)
    {
        Color32 ownColor = ownMaterial.color;
        if (changeColor)
        {
            ownColor = colorToTransformInto;
        }

        ownColor.a = alphaValue;

        if (tweenAlpha)
        {
            TweenColor(ownColor);
        }
        else
        {
            ownMaterial.color = ownColor;
        }
    }

    // tween the color of the object
    public void TweenColor(Color32 endColor)
    {
        startTweenColor = ownMaterial.color;
        endTweenColor = endColor;
        iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", alpaTweenTime, "onupdate", "UpdateColor"));
    }

    // update color of object every frame of the tween
    private void UpdateColor(float val)
    {
        Color32 newColor = ownMaterial.color;
        newColor.r = (byte)(((1f - val) * startTweenColor.r) + (val * endTweenColor.r));
        newColor.b = (byte)(((1f - val) * startTweenColor.b) + (val * endTweenColor.b));
        newColor.g = (byte)(((1f - val) * startTweenColor.g) + (val * endTweenColor.g));
        newColor.a = (byte)(((1f - val) * startTweenColor.a) + (val * endTweenColor.a));
        ownMaterial.color = newColor;
    }
}
