using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdHouseHammer : MonoBehaviour
{
    private Vector3 prevPos = Vector3.zero;

    private float speedOfSwing = 0, audioCooldown = 0.2f, audioTimer = 5;

    [SerializeField]
    private float neededSpeedOfSwing = 0.5f, minimumNeededSpeed = 0.01f;

    private new AudioSource audio = null;

    // Set audio source
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Calculate speed of swing based on prevpos and current position
    private void FixedUpdate()
    {
        speedOfSwing = Vector3.Distance(transform.position, prevPos);
        prevPos = transform.position;
        audioTimer -= Time.fixedDeltaTime;
    }

    // Play a sound if the cillding object has a MeshRenderer
    private void OnTriggerEnter(Collider other)
    {
        if (speedOfSwing >= minimumNeededSpeed)
        {
            if (other.GetComponentInChildren<MeshRenderer>() != null || other.GetComponent<MeshRenderer>() != null)
            {
                PlaySound();
            }
        }
    }

    // Play a sound if the timer is below 0 and adjust pitch based on speed
    private void PlaySound()
    {
        if (audioTimer <= 0)
        {
            audio.pitch = neededSpeedOfSwing / speedOfSwing;
            if (audio.pitch < 1)
            {
                audio.pitch = 1;
            } else if (audio.pitch > 2)
            {
                audio.pitch = 1.5f;
            }
            audio.Play();
            audioTimer = audioCooldown;
        }
    }
}
