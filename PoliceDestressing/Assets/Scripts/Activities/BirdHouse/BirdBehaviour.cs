using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class BirdBehaviour : MonoBehaviour
{
    [SerializeField]
    private float idleSpeed = 0, turnSpeed = 0, switchSeconds = 0, idleRatio = 0, verticalTilt = 48, randomBaseOffset = 5, startUpTimer = 0,
            distanceCheck = 0.5f;

    [SerializeField]
    private Vector2 animSpeedMinMax = Vector2.zero, moveSpeedMinMax = Vector2.zero, changeAnimEveryFromTo = Vector2.zero,
        changeTargetEveryFromTo = Vector2.zero, radiusMinMax = Vector2.zero, yMinMax = Vector2.zero, offsetReturnToBaseTimerCooldown = Vector2.zero,
        offsetStartTimer = Vector2.zero;

    [SerializeField]
    private Transform flyingTarget = null;

    [System.NonSerialized]
    public Transform homeTarget = null;

    [System.NonSerialized]
    public bool returnToBase = false, isInteractingWithFood = false;

    [System.NonSerialized]
    public BirdHouseFood birdFood = null;

    public float returnToBaseTimerCooldown = 0;

    [System.NonSerialized]
    public float baseTimer = 0;

    private Animator animator = null;

    private new Rigidbody rigidbody = null;

    private float changeTarget = 0, changeAnim = 0, timeSinceAnim = 0, currentAnim = 0, prevSpeed = 0, speed = 0, zTurn = 0, prevz = 0, 
        turnSpeedBackUp = 0, distanceFromBase = 0, distanceFromTarget = 0, yDistancing = 10, returnToBaseTimer = 0;

    private Vector3 rotateTarget = Vector3.zero, position = Vector3.zero, direction = Vector3.zero;

    private Quaternion lookRotation = Quaternion.identity;

    private bool stopDistancing = false, checkIfShouldStopReturningToBase = false, startUpOfFlying = false;

    private List<Transform> allBranches = new List<Transform>();

    private int frameCounter = 0;

    private void Start()
    {
        GameObject[] transformsOfBranches = GameObject.FindGameObjectsWithTag("Branch");
        for (int i = 0; i < transformsOfBranches.Length; i++)
        {
            allBranches.Add(transformsOfBranches[i].transform);
        }
        homeTarget = allBranches[Random.Range(0, allBranches.Count - 1)];

        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        turnSpeedBackUp = turnSpeed;
        direction = Quaternion.Euler(transform.eulerAngles) * transform.forward;

        if (baseTimer < 0)
        {
            rigidbody.velocity = idleSpeed * direction;
        }

        returnToBaseTimer = returnToBaseTimerCooldown + Random.Range(offsetReturnToBaseTimerCooldown.x, offsetReturnToBaseTimerCooldown.y);
        rigidbody.freezeRotation = true;

        animator.SetBool("FlyingAgain", true);
    }

    private void FixedUpdate()
    {
        // If the bird is at the base then the timer will count down
        if (baseTimer > 0)
        {
            baseTimer -= Time.fixedDeltaTime;
            bool switchedTarget = false;

            // If the base moved then let the bird fly again
            if (Vector3.Distance(homeTarget.position, rigidbody.position) >= GetComponent<Collider>().bounds.size.y)
            {
                baseTimer = 0;
                switchedTarget = true;
                if (isInteractingWithFood)
                {
                    SetBirdToFlyingState(returnToBaseTimerCooldown + Random.Range(offsetReturnToBaseTimerCooldown.x, 
                        offsetReturnToBaseTimerCooldown.y), false);
                    birdFood.SetFoodToNotBeingEaten();
                } else
                {
                    SetBirdToFlyingState(0.5f, false);
                }
            }

            // When the based timer reached 0, start flying again
            if (baseTimer <= 0 && !switchedTarget)
            {
                isInteractingWithFood = false;
                SetBirdToFlyingState(returnToBaseTimerCooldown + Random.Range(offsetReturnToBaseTimerCooldown.x, 
                    offsetReturnToBaseTimerCooldown.y), true);
            }
            return;
        }

        // The start up flying animation
        if (startUpOfFlying)
        {
            frameCounter++;
            if (frameCounter >= 21)
            {
                startUpOfFlying = false;
                animator.SetBool("FinishedEating", false);
                animator.SetBool("FlyingAgain", true);
                frameCounter = 0;
            }
            return;
        }

        // Calculate distances
        distanceFromBase = Vector3.Distance(homeTarget.position, rigidbody.position);
        distanceFromTarget = Vector3.Distance(flyingTarget.position, rigidbody.position);

        // If missed the base target, calculate a new base stop
        if (checkIfShouldStopReturningToBase)
        {
            if (distanceFromBase > 10)
            {
                baseTimer = 0.001f;
            }
        }

        // Allow drastic turns close to base to ensure target can be reached
        if (returnToBase && distanceFromBase < 7f)
        {
            if (turnSpeed != 300 && rigidbody.velocity.magnitude != 0f)
            {
                checkIfShouldStopReturningToBase = true;
                stopDistancing = true;
                turnSpeed = 300;
            } 
            else if (distanceFromBase <= 5)
            {
                // Stop with flying and start resting on the base
                if (distanceFromBase <= GetComponent<Collider>().bounds.size.y / 2)
                {
                    animator.SetBool("IsEating", true);
                    animator.SetBool("FlyingAgain", false);
                    rigidbody.velocity = Vector3.zero;
                    baseTimer = startUpTimer + Random.Range(offsetStartTimer.x, offsetStartTimer.y);

                    if (isInteractingWithFood)
                    {
                        birdFood.SetFoodToBeingEaten();
                    }
                }
                else // Fly directly to the base when close
                {
                    rigidbody.velocity = homeTarget.position - transform.position;
                    if (isInteractingWithFood)
                    {
                        transform.LookAt(birdFood.transform.position);
                    } else
                    {
                        transform.LookAt(homeTarget.position);
                    }
                }
                return;
            }
        }

        // Time for new animation speed
        if (changeAnim < 0)
        {
            currentAnim = ChangeAnim(currentAnim);
            changeAnim = Random.Range(changeAnimEveryFromTo.x, changeAnimEveryFromTo.y);
            timeSinceAnim = 0;
            prevSpeed = speed;
            if (currentAnim == 0)
            {
                speed = idleSpeed;
            } else
            {
                speed = Mathf.Lerp(moveSpeedMinMax.x, moveSpeedMinMax.y, (currentAnim - animSpeedMinMax.x) / (animSpeedMinMax.y - animSpeedMinMax.x));
            }
        }

        // Create a ray directions list for the bird too shoot rays to
        int sign = 1;
        List<Vector3> rayDirections = new List<Vector3>();
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                AddDirectionToList(rayDirections, transform.forward, sign, false);
            }
            else
            {
                if (i < 3)
                {
                    AddDirectionToList(rayDirections, transform.right, sign);
                }
                else
                {
                    AddDirectionToList(rayDirections, transform.up, sign);
                }
                sign *= -1;
            }
        }

        // Check if the rays hit anything, if so add them to the hitpoints list
        List<Vector3> hitPoints = new List<Vector3>();
        bool tooClose = false;
        for (int i = 0; i < rayDirections.Count; i++)
        {
            if (Physics.SphereCast(transform.position, transform.localScale.z, rayDirections[i], out RaycastHit hit, distanceCheck))
            {
                tooClose = true;
                hitPoints.Add(hit.point);
            }
        }

        // Timer for a new target position or if its to close
        if (changeTarget < 0 || tooClose)
        {
            rotateTarget = ChangeDirection(transform.position, tooClose, hitPoints);

            if (tooClose)
            {
                changeTarget = 0.01f;
            } else if (returnToBase)
            {
                changeTarget = 0.2f;
            } else
            {
                changeTarget = Random.Range(changeTargetEveryFromTo.x, changeTargetEveryFromTo.y);
            }
        }

        // Turn when approaching height limits and the bird isnt almost at base
        if (!checkIfShouldStopReturningToBase)
        {
            if (transform.position.y < yMinMax.x + yDistancing || transform.position.y > yMinMax.y - yDistancing)
            {
                if (transform.position.y < yMinMax.x + yDistancing)
                {
                    rotateTarget.y = 1;
                }
                else
                {
                    rotateTarget.y = -1;
                }
            }
        }

        // Clamp z rotation
        zTurn = Mathf.Clamp(Vector3.SignedAngle(rotateTarget, direction, Vector3.up), -45, 45);

        // Update times
        changeAnim -= Time.fixedDeltaTime;
        changeTarget -= Time.fixedDeltaTime;
        timeSinceAnim += Time.fixedDeltaTime;
        returnToBaseTimer -= Time.fixedDeltaTime;

        // Rotate towards target
        if (rotateTarget != Vector3.zero)
        {
            lookRotation = Quaternion.LookRotation(rotateTarget, Vector3.up);
        }

        Vector3 rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime).eulerAngles;
        transform.eulerAngles = rotation;

        // Rotate on z-axis to tilt body towards turn direction
        float temp = prevz;
        if (prevz < zTurn)
        {
            prevz += Mathf.Min(turnSpeed * Time.fixedDeltaTime, zTurn - prevz);
        } else
        {
            prevz -= Mathf.Min(turnSpeed * Time.fixedDeltaTime, prevz - zTurn);
        }

        // Min and max rotation on z-axis
        prevz = Mathf.Clamp(prevz, -45, 45);

        // Remove temp if transform is rotated back earlier in FixedUpdate
        transform.eulerAngles += new Vector3(0, 0, prevz - temp);

        // Move bird
        direction = Quaternion.Euler(transform.eulerAngles) * Vector3.forward;
        if (returnToBase && distanceFromBase < idleSpeed)
        {
            rigidbody.velocity = Mathf.Min(idleSpeed, distanceFromBase) * direction;
        }
        else
        {
            rigidbody.velocity = Mathf.Lerp(prevSpeed, speed, Mathf.Clamp(timeSinceAnim / switchSeconds, 0, 1)) * direction;
        }

        // Hard-limit the height, in case the limit is breached despite of the turnaround attempt
        if (transform.position.y < yMinMax.x || transform.position.y > yMinMax.y)
        {
            position = transform.position;
            position.y = Mathf.Clamp(position.y, yMinMax.x, yMinMax.y);
            transform.position = position;
        }

        // Let the bird return to the home target if the timer reached 0, let it fly lower by lowering the y distancing
        if (returnToBaseTimer <= 0)
        {
            returnToBase = true;
            yDistancing = 0.5f;
        }
    }

    // Select a new animation speed randomly
    private float ChangeAnim(float currentAnim)
    {
        float newState = 0;

        if (Random.Range(0f, 1f) < idleRatio && !stopDistancing)
        {
            newState = 0;
        } else if (!stopDistancing)
        {
            newState = Random.Range(animSpeedMinMax.x, animSpeedMinMax.y);
        } else
        {
            newState = animSpeedMinMax.y;
        }

        if (newState != currentAnim)
        {
            animator.SetFloat("flySpeed", newState);
            if (newState == 0)
            {
                animator.speed = 1;
            } else
            {
                animator.speed = newState;
            }
        }

        return newState;
    }

    // Select a new direction to fly in randomly if its no too close
    private Vector3 ChangeDirection(Vector3 position, bool tooClose, List<Vector3> hitPoints)
    {
        Vector3 newDir = Vector3.zero;

        if (tooClose && !stopDistancing)
        {
            // Choose the opposite direction of what th collision points are if it detected those
            turnSpeed = turnSpeedBackUp * 2;
            for (int i = 0; i < hitPoints.Count; i++)
            {
                if (hitPoints[i] != Vector3.zero)
                {
                    newDir += hitPoints[i] - position;
                }
            }

            newDir *= -1;

            if (newDir == Vector3.zero)
            {
                print("why stress?");
                newDir = -transform.right / (verticalTilt / 2f);
                newDir += transform.up / (verticalTilt / 2f);
            }
        }
        else
        {
            // Reset heightend turn speed if its not almost at base
            if (!checkIfShouldStopReturningToBase)
            {
                turnSpeed = turnSpeedBackUp;
            }

            if (returnToBase)
            {
                Vector3 yOffset = homeTarget.position;
                yOffset.y += Random.Range(-randomBaseOffset, randomBaseOffset);
                newDir = yOffset - position;
            }
            else if (distanceFromTarget > radiusMinMax.y)
            {
                newDir = flyingTarget.position - position;
            }
            else if (distanceFromTarget < radiusMinMax.x)
            {
                newDir = position - flyingTarget.position;
            }
            else
            {
                // 360-degree freedom of choice on the horizontal plane
                float angleXZ = Random.Range(-Mathf.PI, Mathf.PI);
                // Limited max steepness of ascent/descent in the vertical direction
                float angleY = Random.Range(-Mathf.PI / verticalTilt, Mathf.PI / verticalTilt);

                // Calculate direction
                newDir = Mathf.Sin(angleXZ) * Vector3.forward + Mathf.Cos(angleXZ) * Vector3.right + Mathf.Sin(angleY) * Vector3.up;
            }
        }

        return newDir.normalized;
    }

    // Set all the stats fot the flying state of the bird
    public void SetBirdToFlyingState(float flyTime, bool switchTarget)
    {
        // Set all return to base variables to false and reset yDistancing
        stopDistancing = false;
        returnToBase = false;
        yDistancing = 10;
        checkIfShouldStopReturningToBase = false;

        // Set a time to fly and reset turn speed
        returnToBaseTimer = flyTime;
        turnSpeed = turnSpeedBackUp;

        // Change animation
        animator.SetBool("IsEating", false);
        animator.SetBool("FinishedEating", true);
        startUpOfFlying = true;

        // Select new home target
        if (switchTarget)
        {
            homeTarget = allBranches[Random.Range(0, allBranches.Count - 1)];
        }
    }

    // Add a ray direction and its oblique part to the list of directions
    private void AddDirectionToList(List<Vector3> rayDirections, Vector3 direction, int sign, bool addOblique = true)
    {
        direction *= sign;
        rayDirections.Add(direction);

        if (addOblique)
        {
            rayDirections.Add(direction + transform.forward);
        }
    }
}
