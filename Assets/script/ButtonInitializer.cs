using UnityEngine;
using UnityEngine.UI;

public class ButtonInitializer : MonoBehaviour
{
    public Button button; // Reference to the Button component
    private ButtonSound buttonSound;

    private void Start()
    {
        // Find the ButtonSound script in the scene
        buttonSound = FindObjectOfType<ButtonSound>();

        if (button != null)
        {
            // Add PlaySound as an additional listener, without removing existing listeners
            if (buttonSound != null)
            {
                button.onClick.AddListener(buttonSound.PlaySound);
            }
        }
        else
        {
            Debug.LogWarning("Button is not assigned or found!");
        }
    }
}
