using System.Collections;
using System.Collections.Generic; // Import for List
using UnityEngine;
using DG.Tweening; // Import DoTween
using UnityEngine.EventSystems; // For click detection

public class PanelController : MonoBehaviour
{
    public RectTransform panelTransform; // Reference to the RectTransform of the entire panel
    public float startOffsetY = 800f;    // How far above the screen the panel starts from
    public float animationTime = 0.6f;    // Duration of the animation
    public List<Transform> ignoredChildren; // List of child elements to ignore clicks on

    private Vector3 originalPosition; // To store the original position of the panel
    private bool isAnimatingBack = false; // To track if we are animating back

    public GameObject settingButton;

    private void Start()
    {
        // Store the original position of the panel
        originalPosition = panelTransform.anchoredPosition;
        // Start with the panel off-screen
        panelTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y - 200 + startOffsetY);

        // Animate the panel into view
        panelTransform.DOAnchorPosY(originalPosition.y, animationTime)
            .SetEase(Ease.OutBack);
    }

    private void OnEnable()
    {
        // Animate the panel into view when enabled
        panelTransform.DOAnchorPosY(originalPosition.y, animationTime)
            .SetEase(Ease.OutBack);
    }

    private void Update()
    {
        // Detect a left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the click was outside the panel area
            if (!IsPointerOverUI())
            {
                // Clicked outside, start reverse animation
                if (!isAnimatingBack)
                {
                    isAnimatingBack = true;
                    settingButton.SetActive(true);
                    StartCoroutine(ReversePanelAnimation());
                }
            }
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Check if the pointer is over the panel
        if (RectTransformUtility.RectangleContainsScreenPoint(panelTransform, eventData.position))
        {
            Debug.Log("Pointer is over the panel.");
            foreach (Transform child in ignoredChildren)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(child.GetComponent<RectTransform>(), eventData.position, Camera.main))
                {
                    Debug.Log("Pointer is over an ignored child.");
                    return true; // Pointer is over an ignored child
                }
            }
            return true; // Pointer is over the panel, but not over ignored children
        }

        Debug.Log("Pointer is outside the panel.");
        return false; // Pointer is outside the panel
    }


    private IEnumerator ReversePanelAnimation()
    {
        // Animate the panel back up off-screen
        panelTransform.DOAnchorPosY(originalPosition.y + 200 + startOffsetY, animationTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                // Reset panel position after animation
                ResetPanel();
                isAnimatingBack = false; // Reset once animation completes
            });

        yield return null;
    }

    private void ResetPanel()
    {
        // Reset the panel position to original
        panelTransform.anchoredPosition = originalPosition + new Vector3(0, startOffsetY, 0);
        panelTransform.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GoToHome()
    {

    }
}
