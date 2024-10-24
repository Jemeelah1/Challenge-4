using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    public float turboBoostMultiplier = 2f; // Turbo boost multiplier
    public GameObject turboBoostEffect; // Reference to the particle effect
    private bool isBoosting = false; // Tracks if player is boosting

    private float boostDuration = 2f; // Duration for the boost in seconds
    private float speedBoost = 1000; // Speed boost amount

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);

        // Check if the player presses spacebar and is not already boosting
        if (Input.GetKeyDown(KeyCode.Space) && !isBoosting)
        {
            StartCoroutine(TurboBoost()); // Start the turbo boost coroutine
        }
    }

    // Coroutine to handle the turbo boost
    IEnumerator TurboBoost()
    {
        isBoosting = true; // Player is now boosting
        speed *= turboBoostMultiplier; // Increase player speed

        // Activate the turbo boost particle effect
        turboBoostEffect.SetActive(true);

        yield return new WaitForSeconds(boostDuration); // Wait for the boost to end

        // Reset player speed and deactivate the boost effect
        speed /= turboBoostMultiplier;
        turboBoostEffect.SetActive(false);
        isBoosting = false; // Player is no longer boosting
    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =  transform.position - other.gameObject.transform.position; 
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }
        }
    }
}
