using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a countdown timer with visual and audio feedback.
/// </summary>
public class CountdownTimer : MonoBehaviour
{
    public float timerDuration = 10f; // Total duration of the timer in seconds
    public float currentTime;
    public TextMeshProUGUI timerText; // Reference to the TextMeshPro text element
    public GameObject circularBorder; // Reference to the circular border image
    public float heartbeatSpeed = 1f; // Speed of the heartbeat animation
    public float maxScaleIncrease = 1.2f; // Maximum scale for the heartbeat effect

    public GameObject timer;

    public OrganismManager org;

    private void Start()
    {
        currentTime = timerDuration; // Initialize the timer with the duration
        UpdateTimerDisplay(); // Update the timer display at the start
        StartCoroutine(MoveTimerToCorner()); // Move the timer to the corner
    }

    /// <summary>
    /// Starts the countdown timer and updates the display every second.
    /// </summary>
    public IEnumerator StartCountdown()
    {
        while (currentTime > 0)
        {
            UpdateTimerDisplay(); // Update the timer display
            yield return new WaitForSeconds(1f); // Wait for one second
            currentTime--; // Decrease the timer by 1 second
        }

        currentTime = 0; // Ensure the timer ends at 0
        if(currentTime == 0)
        {
            org.FlipAllCardsMultipleTimes();
            
        }
        UpdateTimerDisplay(); // Final update to show "0"
        circularBorder.SetActive(false);
        StartCoroutine(DisplayTimeUpMessage()); // Show "Time's up!" message
    }

    public void MakeTimerZero()
    {
        currentTime = 0;
        timerText.text = "";
        circularBorder.SetActive(false);
        
    }

    /// <summary>
    /// Updates the timer display and circular border based on the remaining time.
    /// </summary>
    private void UpdateTimerDisplay()
    {
        // Update the timer text with an integer value
        timerText.text = Mathf.Ceil(currentTime).ToString();

        // Change the color of the timer based on the remaining time
        if (currentTime > 6)
        {
            timerText.color = Color.green; // Green color
        }
        else if (currentTime > 4)
        {
            timerText.color = Color.yellow; // Yellow color
        }
        else
        {
            timerText.color = Color.red; // Red color
        }

        // Update the circular border fill amount
       /* if (circularBorder != null)
        {
            circularBorder.fillAmount = currentTime / timerDuration;
        }*/
    }

    /// <summary>
    /// Creates a heartbeat effect by scaling the timer text.
    /// </summary>
    private IEnumerator HeartbeatEffect()
    {
        Vector3 originalScale = timerText.rectTransform.localScale; // Store the original scale of the text
        Vector3 targetScale = originalScale * maxScaleIncrease; // Target scale for the heartbeat effect

        while (true)
        {
            // Animate the scale of the text to create a heartbeat effect
            float elapsedTime = 0f;
            while (elapsedTime < heartbeatSpeed)
            {
                timerText.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / heartbeatSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < heartbeatSpeed)
            {
                timerText.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, (elapsedTime / heartbeatSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Adjust the speed of the heartbeat effect based on remaining time
            if (currentTime <= 4)
            {
                heartbeatSpeed = 0.3f; // Faster heartbeat when time is critical
            }
            else if (currentTime <= 6)
            {
                heartbeatSpeed = 0.5f; // Moderate heartbeat
            }
            else
            {
                heartbeatSpeed = 1f; // Normal heartbeat speed
            }
        }
    }

    /// <summary>
    /// Displays a "Time's up!" message when the timer ends.
    /// </summary>
    private IEnumerator DisplayTimeUpMessage()
    {
        //timerText.text = "Time's up!";
        timerText.color = Color.red; // Make sure the text remains red when the timer ends
        timerText.rectTransform.localScale = Vector3.one; // Reset the scale of the text

       /* if (circularBorder != null)
        {
            circularBorder.fillAmount = 0f; // Ensure the border is empty when time is up
        }*/

        yield return new WaitForSeconds(4f); // Wait for one second

        timerText.text = ""; // Clear the text after displaying "Time's up!" for one second
    }

    /// <summary>
    /// Moves the timer object to the corner of the screen and scales it down.
    /// </summary>
    IEnumerator MoveTimerToCorner()
    {
        RectTransform timerRect = timer.GetComponent<RectTransform>();
        RectTransform canvasRect = timerRect.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        yield return new WaitForSeconds(1f); // Wait for one second before moving the timer

        // Calculate target position dynamically using canvas size
        float paddingX = 150f; // Adjust padding from screen edges
        float paddingY = 100f;

        float targetX = (-canvasRect.rect.width / 2) + paddingX;
        float targetY = (canvasRect.rect.height / 2) - paddingY;
        Vector2 targetPosition = new Vector2(targetX, targetY);

        // Move the timer smoothly to the calculated position
        timerRect.DOAnchorPos(targetPosition, 1f).OnComplete(() =>
        {
            StartCoroutine(StartCountdown()); // Start the countdown after moving to the corner
        });

        // Scale down the timer
        timerRect.DOScale(Vector3.one * 0.2f, 1f); // Scale down to 50%
    }

}
