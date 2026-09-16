/*using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardController : MonoBehaviour
{
    public GameObject childImage;  // Reference to the child image
    public bool isRotating = false;
    public bool isFlipped = false;  // Make isFlipped public with a private setter
    private MainCardManager cardManager;
    private float rotationSpeed = 180f;  // Speed of rotation
    private float targetRotationAngle = 180f;  // Total rotation angle to complete the flip
    public float currentRotationAngle = 0f;  // Track current rotation angle

    private Vector3 originalPosition;  // To store the original position of the card
    private Vector3 originalScale;  // To store the original scale of the card
    private bool isAnimating = false;  // To track if the card is in the middle of an animation

    private bool isInputBlocked = false;  // To block touch inputs during animations

    // Audio settings
    public AudioClip rightAnswerSound;  // Assign the right answer audio clip in the inspector
    public AudioClip wrongAnswerSound;   // Assign the wrong answer audio clip in the inspector
    private AudioSource audioSource;      // Reference to the audio source component

    private void Start()
    {
        cardManager = FindObjectOfType<MainCardManager>();  // Find and reference the MainCardManager
        originalPosition = transform.localPosition;  // Store the original position of the card
        originalScale = transform.localScale;  // Store the original scale of the card
        audioSource = gameObject.AddComponent<AudioSource>();  // Add audio source component
    }

    private void Update()
    {
        // Rotate both the parent card and the child image if the rotation flag is active
        if (isRotating)
        {
            RotateCard();
        }
    }

    private void RotateCard()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;  // Calculate rotation for this frame
        currentRotationAngle += rotationStep;

        // Rotate both the parent object (card) and its child image simultaneously on the Y-axis
        transform.Rotate(0, rotationStep, 0);
        childImage.transform.Rotate(0, rotationStep, 0);

        // Check if the flip is complete
        if (currentRotationAngle >= targetRotationAngle)
        {
            StopRotation();
        }
    }

    public void StartRotationSequence()
    {
        isRotating = true;
        currentRotationAngle = 0f;  // Reset the rotation angle when starting a new flip
    }

    private void StopRotation()
    {
        isRotating = false;
        if (!isFlipped)
        {
            childImage.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnCardClick()
    {
        // Prevent interactions if the card is already flipped, rotating, animating, input is blocked, or panel delay is active
        if (isFlipped || isRotating || isAnimating || isInputBlocked || cardManager.IsPanelDelayActive()) return;

        if (CompareTag("Right") || CompareTag("Wrong"))
        {
            FlipImage();
            if (CompareTag("Right"))
            {
                childImage.transform.GetChild(0).gameObject.SetActive(true);
                PlaySound(rightAnswerSound);  // Play right answer sound
                cardManager.CheckAllRightCardsFlipped();  // Notify the manager to check if all right cards are flipped
                StartCoroutine(BlockInputDuringAnimation(MoveCardToCenterAndBack()));  // Block input during animation
            }
            if (CompareTag("Wrong"))
            {
                childImage.transform.GetChild(0).gameObject.SetActive(true);
                PlaySound(wrongAnswerSound);  // Play wrong answer sound
                Invoke("DisableCard", 0.5f);  // Call DisableCard instead of DestroyCard
            }
        }
    }

    private void FlipImage()
    {
        isFlipped = true;
        childImage.SetActive(true);  // Ensure the child image is visible before starting the flip
        StartRotationSequence();     // Start the rotation sequence to flip both the parent and child
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);  // Play the audio clip
        }
    }

    private IEnumerator BlockInputDuringAnimation(IEnumerator animationCoroutine)
    {
        isInputBlocked = true;  // Block input
        yield return StartCoroutine(animationCoroutine);  // Wait for the animation to complete
        isInputBlocked = false;  // Unblock input
    }

    private IEnumerator MoveCardToCenterAndBack()
    {
        isAnimating = true;

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the card on top of the hierarchy
        transform.SetAsLastSibling();  // Bring the card to the front

        // Calculate the center position on the Canvas
        Vector3 centerPosition = new Vector3(0, 0, 0);  // This is the local position for the center of the Canvas

        // Smoothly move the card to the center of the Canvas
        float duration = 1f;  // Time to move the card to the center
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;

        // Start scaling and moving the card towards the center
        while (elapsedTime < duration)
        {
            // Move the card towards the center
            transform.localPosition = Vector3.Lerp(startPosition, centerPosition, elapsedTime / duration);

            // Scale the card up
            float scaleFactor = 1 + (elapsedTime / duration);  // Scale factor increases over time
            transform.localScale = originalScale * scaleFactor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the card is exactly at the center and scaled correctly
        transform.localPosition = centerPosition;  // Set to the exact center position
        transform.localScale = originalScale * (1 + 1);  // Set the final scale

        // Wait for a short moment at the center
        yield return new WaitForSeconds(1f);

        // Smoothly move the card back to its original position and scale
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Move the card back to its original position
            transform.localPosition = Vector3.Lerp(centerPosition, originalPosition, elapsedTime / duration);
            // Scale down the card
            float scaleFactor = 1 + (1 - (elapsedTime / duration));  // Scale factor decreases back to original
            transform.localScale = originalScale * scaleFactor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the card is exactly at its original position and scaled correctly
        transform.localPosition = originalPosition;  // Ensure the card is exactly at its original position
        transform.localScale = originalScale;  // Reset the scale to original

        // Unlock the cursor after the animation
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isAnimating = false;  // End the animation sequence, allowing interaction again
    }

    private void DisableCard()
    {
        gameObject.SetActive(false);  // Disable the card instead of destroying it
    }

    public void ResetCard()
    {
        isFlipped = false; // Reset the flip state
        childImage.SetActive(false); // Hide the child image

        // Reset rotation or any other state you need
        currentRotationAngle = 0f; // Reset the current rotation angle
        transform.localRotation = Quaternion.identity; // Reset rotation of the card to original
        transform.localPosition = originalPosition; // Reset the position of the card
        transform.localScale = originalScale; // Reset the scale of the card
    }
}
*/