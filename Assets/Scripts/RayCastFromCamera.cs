using UnityEngine;

public class RaycastFromCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        // Find the main camera component attached to the player capsule
        mainCamera = GetComponentInChildren<Camera>();

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    private void Update()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the main camera through the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast and check if it hits any object
            if (Physics.Raycast(ray, out hit))
            {
                // Get the GameObject that was hit
                GameObject hitObject = hit.collider.gameObject;

                // Output the name of the GameObject to the console
                Debug.Log("Hit object: " + hitObject.name);

                GOInteraction aGOI = hit.collider.gameObject.GetComponent<GOInteraction>();
                if (aGOI)
                {
                    aGOI.Interaction = true;
                }
            }

            // Draw a line to visualize the raycast
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 1f);
        }
    }
}
