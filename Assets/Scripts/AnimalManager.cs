using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    private List<GameObject> children = new List<GameObject>();
    private GOInteraction myGOI;
    private Animator animalAnimator;

    // Reference to the player's camera
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Information"))
            {
                children.Add(child.gameObject);
            }
        }

        myGOI = GetComponent<GOInteraction>();
        if (myGOI == null)
        {
            Debug.Log("No GOInteraction attached to this object.");
        }

        // Get the Animator component of the parent GameObject
        animalAnimator = GetComponent<Animator>();

        // Default children to inactive.
        foreach (GameObject child in children)
        {
            child.SetActive(false);
            // Flip the texture by scaling in the y-axis
            Vector3 scale = child.transform.localScale;
            scale.y *= -1f;
            scale.x *= -1f;
            child.transform.localScale = scale;
        }

        // Ensure that a camera is assigned to playerCamera
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not assigned to AnimalManager. Please assign the player camera in the Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myGOI.Interaction)
        {
            foreach (GameObject child in children)
            {
                if (child.activeSelf)
                {
                    child.SetActive(false);
                    // Disable the entire animator component
                    animalAnimator.enabled = true;
                }
                else
                {
                    child.SetActive(true);
                    // Enable the animator component
                    animalAnimator.enabled = false;
                }
            }
            myGOI.Interaction = false;
        }

        // Rotate the "Information" cube to match the player's camera rotation
        if (playerCamera != null)
        {
            foreach (GameObject child in children)
            {
                child.transform.rotation = playerCamera.transform.rotation;
            }
        }
    }
}
