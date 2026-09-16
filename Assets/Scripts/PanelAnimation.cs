using System.Collections;
using UnityEngine;
using DG.Tweening;  // Import DoTween

public class PanelAnimation : MonoBehaviour
{
    public RectTransform[] childPanels;  // Array for child panels (supports 1 or more children)
    public float[] endPositionsY;        // Array for individual Y end positions of the child panels

    public float fallTime = 0.6f;        // Smooth fall time for a cartoon-like effect
    public float bounceTime = 0.7f;      // Elastic-like bounce time
    public float delayBetweenPanels = 0.15f;  // Slight delay between child panel animations
    public float startOffsetY = 800f;    // How far above screen the panels start from

    public float bounceStrength = 20f;   // Strength of the bounce back effect

    private Vector3[] initialPositions;  // To store the initial positions of each child panel

    void OnEnable()
    {
        // Initialize array to store original positions
        initialPositions = new Vector3[childPanels.Length];

        // Move each panel up and animate
        for (int i = 0; i < childPanels.Length; i++)
        {
            // Store the original position
            initialPositions[i] = childPanels[i].anchoredPosition;

            // Move panel up by startOffsetY (off-screen)
            childPanels[i].anchoredPosition = new Vector2(initialPositions[i].x, initialPositions[i].y + startOffsetY);
        }

        // If there are two panels, make the second one fall first
        if (childPanels.Length == 2)
        {
            // Animate second child first (index 1), then the first child (index 0)
            StartCoroutine(AnimatePanel(1));  // Second child falls first
            StartCoroutine(AnimatePanel(0, delayBetweenPanels));  // First child falls after a delay
        }
        else if (childPanels.Length == 1)
        {
            // If only one panel, animate it immediately
            StartCoroutine(AnimatePanel(0));
        }
    }

    IEnumerator AnimatePanel(int index, float delay = 0f)
    {
        // Apply a delay between child panel animations
        yield return new WaitForSeconds(delay);

        // Create the fall animation with smooth, subtle easing
        childPanels[index].DOAnchorPosY(endPositionsY[index], fallTime)
            .SetEase(Ease.InQuad)  // Smooth fall with slight acceleration, perfect for a clean look
            .OnComplete(() =>
            {
                // Then apply a soft bounce animation after falling
                childPanels[index].DOAnchorPosY(endPositionsY[index] + bounceStrength, bounceTime)
                    .SetEase(Ease.OutElastic, 0.5f, 0.3f);  // Elastic bounce effect for a cartoonish, smooth look
                    /*.OnComplete(() =>
                    {
                        // Reset to final position (without overshoot)
                        childPanels[index].DOAnchorPosY(endPositionsY[index], bounceTime * 0.5f)
                            .SetEase(Ease.OutQuad);  // Smoothly settle into final position
                    });*/
            });
    }

    void OnDisable()
    {
        // Reset panels to their initial positions when the parent panel is disabled
        for (int i = 0; i < childPanels.Length; i++)
        {
            childPanels[i].anchoredPosition = initialPositions[i];
        }
    }
}





