using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SwipeSelection : MonoBehaviour
{
    [SerializeField]
    private GameObject pageObject = null;

    [SerializeField]
    private float minimumMovement = 0.05f, minimumDistance = 0.5f;

    private Transform leftHand = null, rightHand = null;

    [System.NonSerialized]
    public Renderer pageRenderer = null;

    private Vector3 prevLeftHandPos = Vector3.zero, prevRightHandPos = Vector3.zero;

    private float timer = 0, cooldown = 0.4f;

    public int page { get; set; } = 0;

    private bool isSelected = false;

    [SerializeField]
    private List<Material> pageMaterials = new List<Material>();

    // Initialize objects
    public virtual void Start()
    {
        FetchHands();

        pageRenderer = pageObject.GetComponent<Renderer>();
        GetComponent<XRGrabInteractable>().onFirstHoverEntered.AddListener(SetSelected);
        GetComponent<XRGrabInteractable>().onLastHoverExited.AddListener(UnSelect);

        // Set renderers to the first page
        FlipPage(0);
    }

    private void FixedUpdate()
    {
        // if there are missing hands, dont do the other funtions
        if (leftHand == null || rightHand == null)
        {
            FetchHands();
            return;
        }

        // Calculate if the hands should flip pages
        CalculateHandDirection(leftHand.position, prevLeftHandPos);
        CalculateHandDirection(rightHand.position, prevRightHandPos);

        // Set variables for calculations
        prevRightHandPos = rightHand.position;
        prevLeftHandPos = leftHand.position;
        timer -= Time.fixedDeltaTime;
    }

    // Grab hands based on tags
    private void FetchHands()
    {
        if (GameObject.FindGameObjectWithTag("LeftHand") != null)
        {
            leftHand = GameObject.FindGameObjectWithTag("LeftHand").transform;
        }
        if (GameObject.FindGameObjectWithTag("RightHand") != null)
        {
            rightHand = GameObject.FindGameObjectWithTag("RightHand").transform;
        }
    }

    // Calculate if hands are close enough, then check direction speed should be checked
    private void CalculateHandDirection(Vector3 pos, Vector3 prevPos)
    {
        float distance = Vector3.Distance(transform.position, pos);
        if (isSelected && timer <= 0 && distance <= minimumDistance)
        {
            Vector3 handDirection = pos - prevPos;
            if (transform.eulerAngles.z >= 75 && transform.eulerAngles.z <= 105 || transform.eulerAngles.z >= 255 && transform.eulerAngles.z <= 285)
            {
                CheckDirection(handDirection.y);
            }
            else if (transform.eulerAngles.y > 45 && transform.eulerAngles.y < 135 || transform.eulerAngles.y > 225 && transform.eulerAngles.y < 315)
            {
                CheckDirection(handDirection.z);
            }
            else
            {
                CheckDirection(handDirection.x);
            }
        }
    }

    // Check if the direction went is bigger then the minimum movement nessecary
    private void CheckDirection(float direction)
    {
        if (direction >= minimumMovement)
        {
            FlipPage(-1);
        }
        else if (direction <= -minimumMovement)
        {
            FlipPage(1);
        }
    }

    // Edit page number & render other materials based on page
    public virtual void FlipPage(int direction)
    {
        page += direction;

        if (page < 0)
        {
            page = pageMaterials.Count - 1;
        }
        else if (page > pageMaterials.Count - 1)
        {
            page = 0;
        }

        pageRenderer.material = pageMaterials[page];
        timer = cooldown;
    }

    // Check if object is selected (this is not grabbed) by a ray
    private void SetSelected(XRBaseInteractor interactor)
    {
        isSelected = true;
    }

    // Set object to not be selected anymore
    private void UnSelect(XRBaseInteractor interactor)
    {
        isSelected = false;
    }
}
