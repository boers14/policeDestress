using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Painting : MonoBehaviour
{
    [System.NonSerialized]
    public bool canSwitchScene = true;

    // Load scene based on buildindex
    public void LoadScene(int sceneIndex)
    {
        if (canSwitchScene)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}




