using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public KeyCode captureKey = KeyCode.C; // Change this to the key you want to use for capturing
    public KeyCode viewKey = KeyCode.V; // Change this to the key you want to use for viewing images
    public KeyCode exitKey = KeyCode.Z; // Change this to the key you want to use for viewing images
    public TextMeshProUGUI captureText; // Reference to the TextMeshPro text element for displaying capture message
    public GameObject canvasPrefab; // Reference to the canvas prefab
    public Transform imagesParent; // Parent transform for the canvas instances
    public float captureDuration = 1f; // Duration to display the captured image

    private List<GameObject> capturedImages = new List<GameObject>(); // List to store captured image canvases
    private int currentImageIndex = -1; // Index of the currently displayed image (-1 means no image is displayed)
    private Coroutine viewCoroutine; // Coroutine reference for viewing images
    private bool isViewingImages = false; // Flag to indicate if currently viewing images
    private Camera mainCamera;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>(); // Get the camera component from the children
        captureText.gameObject.SetActive(false); // Disable capture text initially
    }

    void Update()
    {
        if (!isViewingImages)
        {
            // Capture frames when not viewing images
            if (Input.GetKeyDown(captureKey))
            {
                CaptureFrame();
            }
        }
        else
        {
            // Exit image viewing mode
            if (Input.GetKeyDown(exitKey))
            {
                StopViewingImages();
            }
        }

        // Toggle image viewing mode using the view key
        if (Input.GetKeyDown(viewKey))
        {
            ToggleImageView();
        }

        // Navigate through images using arrow keys when viewing images
        if (isViewingImages)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextImage();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousImage();
            }
        }
    }

    void CaptureFrame()
    {
        // Capture frame
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Create a new canvas
        GameObject canvasObj = Instantiate(canvasPrefab, imagesParent);
        canvasObj.SetActive(true); // Activate the canvas

        // Set the captured photo as the texture of the canvas's RawImage
        RawImage rawImage = canvasObj.GetComponentInChildren<RawImage>();
        rawImage.texture = texture;

        // Show capture message
        captureText.gameObject.SetActive(true);
        captureText.text = "Frame Captured!";
        captureText.transform.position = new Vector3(Screen.width / 2, Screen.height * 0.9f, 0); // Position the text above the image canvas

        // Scale down the canvas temporarily to indicate picture capture
        StartCoroutine(ScaleCanvas(rawImage.gameObject, 0.8f, 0.1f));
        audioManager.PlaySFX(audioManager.ShutterClick);
        Debug.Log("Frame captured!");


        // Add the canvas to the list of captured images
        capturedImages.Add(canvasObj);

        // Start coroutine to deactivate the capture message and the canvas after captureDuration seconds
        StartCoroutine(DeactivateAfterDelay(captureText.gameObject, captureDuration));
        StartCoroutine(DeactivateAfterDelay(canvasObj, captureDuration));
    }

    IEnumerator ScaleCanvas(GameObject canvasObj, float scaleFactor, float duration)
    {
        Vector3 originalScale = canvasObj.transform.localScale;
        Vector3 targetScale = originalScale * scaleFactor;
        float startTime = Time.time;

        // Adjust the pivot of the canvas to its center
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.pivot = new Vector2(0.5f, 0.5f);

        while (Time.time < startTime + duration)
        {
            float progress = (Time.time - startTime) / duration;
            canvasObj.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        canvasObj.transform.localScale = targetScale;

        // Reset the pivot of the canvas to its default value (bottom-left corner)
        canvasRect.pivot = new Vector2(0, 0);
    }


    IEnumerator DeactivateAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Deactivate the game object after delay
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }

    void ToggleImageView()
    {
        if (isViewingImages)
        {
            StopViewingImages();
        }
        else
        {
            StartViewingImages();
        }
    }

    void StartViewingImages()
    {
        isViewingImages = true;

        if (capturedImages.Count == 0)
        {
            Debug.Log("No captured images to view.");
            return;
        }

        viewCoroutine = StartCoroutine(ViewImages());
    }

    void StopViewingImages()
    {
        isViewingImages = false;

        if (viewCoroutine != null)
        {
            StopCoroutine(viewCoroutine);
            viewCoroutine = null;
        }

        // Hide all images
        foreach (var image in capturedImages)
        {
            image.SetActive(false);
        }

        // Hide capture message
        captureText.gameObject.SetActive(false);

        // Reset current image index
        currentImageIndex = -1;
    }

    IEnumerator ViewImages()
    {
        for (int i = 0; i < capturedImages.Count; i++)
        {
            currentImageIndex = i;
            capturedImages[i].SetActive(true);
            captureText.gameObject.SetActive(true);
            captureText.text = "Viewing Image " + (currentImageIndex + 1) + " of " + capturedImages.Count;

            yield return new WaitForSeconds(captureDuration);

            capturedImages[i].SetActive(false);
        }

        // Reset current image index
        currentImageIndex = -1;

        // Hide capture message
        captureText.gameObject.SetActive(false);

        // Stop viewing images
        StopViewingImages();
    }

    void NextImage()
    {
        DisableCurrentImage();
        currentImageIndex = (currentImageIndex + 1) % capturedImages.Count;
        capturedImages[currentImageIndex].SetActive(true);
        captureText.text = "Viewing Image " + (currentImageIndex + 1) + " of " + capturedImages.Count;
    }

    void PreviousImage()
    {
        DisableCurrentImage();
        currentImageIndex = (currentImageIndex - 1 + capturedImages.Count) % capturedImages.Count;
        capturedImages[currentImageIndex].SetActive(true);
        captureText.text = "Viewing Image " + (currentImageIndex + 1) + " of " + capturedImages.Count;
    }

    void DisableCurrentImage()
    {
        if (currentImageIndex != -1)
        {
            capturedImages[currentImageIndex].SetActive(false);
        }
    }
}
