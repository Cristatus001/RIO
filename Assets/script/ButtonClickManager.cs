using UnityEngine;
using UnityEngine.UI;

public class ButtonClickManager : MonoBehaviour
{
    public int uniqueButtonPressCount = 0;
    private string buttonKeyPrefix = "ButtonPressed_";

    private void Start()
    {
        // Set the unique button press count to zero
        uniqueButtonPressCount = 0;

        // Load the saved unique button press count
        uniqueButtonPressCount = PlayerPrefs.GetInt("UniqueButtonPressCount", 0);

        // Update the button outlines for previously clicked buttons
        UpdateButtonOutlines();
    }

    // This method should be called when any button is clicked
    public void OnButtonClick(Button button)
    {
        string buttonName = button.name;
        string buttonKey = buttonKeyPrefix + buttonName;

        // Check if the button has already been clicked
        if (!PlayerPrefs.HasKey(buttonKey))
        {
            // Increment the count for a unique button press
            uniqueButtonPressCount++;
            PlayerPrefs.SetInt("UniqueButtonPressCount", uniqueButtonPressCount);

            // Mark this button as clicked in PlayerPrefs
            PlayerPrefs.SetInt(buttonKey, 1);

            // Change the outline color of the button to green
            SetButtonOutlineColor(button, Color.green);

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }
    }

    // Updates the outlines of previously clicked buttons
    private void UpdateButtonOutlines()
    {
        // Find all buttons in the scene
        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            string buttonKey = buttonKeyPrefix + button.name;

            // If the button was previously clicked, change its outline color to green
            if (PlayerPrefs.HasKey(buttonKey))
            {
                SetButtonOutlineColor(button, Color.green);
            }
            else
            {
                // Ensure the default outline is cleared if not clicked
                SetButtonOutlineColor(button, Color.clear);
            }
        }
    }

    // Helper method to set the outline color of a button
    private void SetButtonOutlineColor(Button button, Color color)
    {
        Outline outline = button.GetComponent<Outline>();

        if (outline == null)
        {
            // Add an Outline component if it doesn't already exist
            outline = button.gameObject.AddComponent<Outline>();
        }

        // Set the outline color to the specified color
        outline.effectColor = color;

        // Adjust outline visibility
        outline.enabled = color != Color.clear;
    }

    // Clear PlayerPrefs data only when the application quits
    private void OnApplicationQuit()
    {
        ClearButtonData();
    }

    // Clear PlayerPrefs data when explicitly called, if needed
    private void ClearButtonData()
    {
        // Remove all button-related data from PlayerPrefs
        PlayerPrefs.DeleteKey("UniqueButtonPressCount");

        // Find all buttons in the scene to clear their individual states
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            string buttonKey = buttonKeyPrefix + button.name;
            PlayerPrefs.DeleteKey(buttonKey);
        }

        // Save the changes to PlayerPrefs
        PlayerPrefs.Save();
    }
}