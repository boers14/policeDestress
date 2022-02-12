using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHousePlateau : MonoBehaviour
{
    // All landing spots for the bird. Should be childed to this object and tagged with BirdLandingSpot
    [System.NonSerialized]
    public List<Transform> plateauLandingSpots = new List<Transform>();
}
