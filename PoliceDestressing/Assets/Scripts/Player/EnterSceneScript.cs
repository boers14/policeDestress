using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterSceneScript : MonoBehaviour
{
    [SerializeField]
    private List<Painting> paintings = new List<Painting>();

    [SerializeField]
    private List<TeleportationArea> teleportationAreas = new List<TeleportationArea>();

    [SerializeField]
    private float timeDeactivated = 1;

    [SerializeField]
    private LayerMask interaction, noInteraction;

    // Deactivate the teleport area if the player entered a new scene
    private void Start()
    {
        StartCoroutine(DeactivateObjects());
    }

    // Deactivate the objects and enable them after the timeDeactivated
    private IEnumerator DeactivateObjects()
    {
        SetActiveStateObjects(false);
        yield return new WaitForSeconds(timeDeactivated);
        SetActiveStateObjects(true);
    }

    // deactivate or activate objects
    private void SetActiveStateObjects(bool enabled)
    {
        for (int  i = 0; i < paintings.Count; i++)
        {
            paintings[i].canSwitchScene = enabled;
        }

        // for teleportation areas the layer for interacting is switched instead
        for (int i = 0; i < teleportationAreas.Count; i++)
        {
            if (enabled)
            {
                teleportationAreas[i].interactionLayerMask = interaction;
            } else
            {
                teleportationAreas[i].interactionLayerMask = noInteraction;
            }
        }
    }
}
