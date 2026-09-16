using UnityEngine;
using UnityEngine.UI;

public class Panel_manager : MonoBehaviour
{
    public ButtonClickManager buttonClickManager;
    public GameObject panel;
    public Button panelButton; // Reference to the button inside the panel

    void Start()
    {
        // Load the count from PlayerPrefs
        buttonClickManager.uniqueButtonPressCount = PlayerPrefs.GetInt("UniqueButtonPressCount", 0);

        // Check if the count is exactly 10 to enable the panel
        if (buttonClickManager.uniqueButtonPressCount == 10)
        {
            panel.SetActive(true);
        }

        // Add a listener to the panel button to handle count updates
        if (panelButton != null)
        {
            panelButton.onClick.AddListener(HandlePanelButtonClick);
        }
    }

    private void HandlePanelButtonClick()
    {
        if (buttonClickManager.uniqueButtonPressCount > 10)
        {
            // If count is more than 10, set it to 11
            buttonClickManager.uniqueButtonPressCount = 11;
            PlayerPrefs.SetInt("UniqueButtonPressCount", 11);
        }
        else
        {
            // Reset the count to 11 for consistency if the button is pressed while panel is active
            buttonClickManager.uniqueButtonPressCount = 11;
            PlayerPrefs.SetInt("UniqueButtonPressCount", 11);
        }

        // Save changes and disable the panel
        PlayerPrefs.Save();
        panel.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        // Do not reset the count to zero when the application quits
        PlayerPrefs.SetInt("UniqueButtonPressCount", buttonClickManager.uniqueButtonPressCount);
        PlayerPrefs.Save();
    }
}
