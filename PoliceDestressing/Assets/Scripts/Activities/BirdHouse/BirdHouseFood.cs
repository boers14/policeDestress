using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BirdHouseFood : MonoBehaviour
{
    private bool birdIsEating = false, hasBeenEatenBefore = false;

    [SerializeField]
    private float moveTweenYDist = -0.2f, baseParticleAmount = 100, particleMultiplier = 2;

    private BirdBehaviour bird = null;

    private float foodDecayTimer = 0, percentageFoodEaten = 0, startFoodDecay = 0;

    private new ParticleSystem particleSystem = null;

    private BirdHouseFoodPanel birdHouseFoodPanel = null;

    [SerializeField]
    private Vector3 foodPosOffset = Vector3.zero, foodRotOffset = Vector3.zero;

    // Set variable
    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Count down the food timer.
    private void FixedUpdate()
    {
        if (birdIsEating)
        {
            foodDecayTimer -= Time.fixedDeltaTime;
            if (foodDecayTimer <= 0)
            {
                // Open up the food panel for the next food and deactivate the food
                birdHouseFoodPanel.StartUnoccupie();
                gameObject.SetActive(false);
            }
            return;
        }
    }

    // Set all the stats for food being eaten and start he particles and tweens
    public void SetFoodToBeingEaten()
    {
        birdIsEating = true;
        if (!hasBeenEatenBefore || foodDecayTimer == 0)
        {
            // for first time eating. Decide timer and emission of particles
            foodDecayTimer = bird.baseTimer - 0.5f;
            startFoodDecay = foodDecayTimer;
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = baseParticleAmount / (foodDecayTimer * particleMultiplier);
        }
        else
        {
            // Set bird timer to own timer and calculate percentage done for the movement over the y-axis for the tween
            bird.baseTimer = foodDecayTimer + 0.5f;
            percentageFoodEaten = foodDecayTimer / startFoodDecay;
            moveTweenYDist *= percentageFoodEaten;
        }

        iTween.ScaleBy(gameObject, iTween.Hash("y", 0, "time", foodDecayTimer, "easetype", iTween.EaseType.linear));
        iTween.MoveBy(gameObject, iTween.Hash("y", moveTweenYDist, "time", foodDecayTimer, "easetype", iTween.EaseType.linear));
        particleSystem.Play();
    }

    // Create a copy of its self and set all stats for working food
    public void SetFoodToEdible(BirdBehaviour bird, BirdHouseFoodPanel birdHouseFoodPanel)
    {
        tag = "Untagged";

        // Create a copy and destroy XrGrabInteractable so it cant be moved and set the correct position
        GameObject copy = Instantiate(gameObject);
        Destroy(copy.GetComponent<XRGrabInteractable>());
        Destroy(copy.GetComponent<Rigidbody>());
        copy.transform.SetParent(birdHouseFoodPanel.transform);
        copy.transform.localPosition = birdHouseFoodPanel.transform.localPosition + foodPosOffset;
        copy.transform.localEulerAngles = birdHouseFoodPanel.transform.localEulerAngles + foodRotOffset;

        // Set the stats for the birdfood component of the copy
        BirdHouseFood copyBirdHouseFood = copy.GetComponent<BirdHouseFood>();
        copyBirdHouseFood.bird = bird;
        copyBirdHouseFood.birdHouseFoodPanel = birdHouseFoodPanel;

        // Set the stats for the bird
        bird.birdFood = copyBirdHouseFood;
        bird.isInteractingWithFood = true;

        // Make the object a trigger
        foreach (Collider c in copyBirdHouseFood.GetComponents<Collider>())
        {
            c.isTrigger = true;
        }

        // Deactivate old object and add the food to the complete birdhouse list of the BirdHouseSpawner
        gameObject.SetActive(false);
        BirdHouseSpawner birdHouseSpawner = GameObject.FindGameObjectWithTag("BirdHouseSpawner").GetComponent<BirdHouseSpawner>();
        birdHouseSpawner.AddObjectToBirdHouse(gameObject);
        birdHouseSpawner.AddObjectToBirdHouse(copy);
    }

    // Stop the particles, tween and timer. Flag it as being eaten for SetFoodToBeingEaten
    public void SetFoodToNotBeingEaten()
    {
        if (gameObject != null)
        {
            birdIsEating = false;
            hasBeenEatenBefore = true;
            iTween.Stop(gameObject);
            particleSystem.Stop();
            particleSystem.Clear();
        }
    }
}
