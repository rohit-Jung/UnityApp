using UnityEngine;

public class DoorAnimationTrigger : MonoBehaviour
{
    // Reference to the animator component of the parent door object
    private Animator doorAnimator;

    private void Start()
    {
        // Get the animator component of the parent door object
        doorAnimator = transform.parent.GetComponent<Animator>();

        // Check if the animator component is found
        if (doorAnimator == null)
        {
            Debug.LogError("Animator component not found on the parent object.");
        }
        else
        {
            // Deactivate the animator initially
            doorAnimator.enabled = false;
        }
    }

    // Called when something collides with the cube
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
        {
            // Activate the animator
            doorAnimator.enabled = true;

            // Play the animation
            doorAnimator.Play("Door Open", 0, 0f); 
        }
    }
}
