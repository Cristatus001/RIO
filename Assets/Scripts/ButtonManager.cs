using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Needed for IEnumerator

public class ButtonManager : MonoBehaviour
{
    public Button[] startButtons; // Array of start buttons
    public Button[] restartButtons; // Array of restart buttons
    public GameObject panelToEnable; // Panel to enable on the 10th unique button press
    private Outline[] startButtonOutlines; // Array to store the outlines of the start buttons

    public int uniqueButtonCount = 0;

    private void Start()
    {
        startButtonOutlines = new Outline[startButtons.Length];

        // Loop through all start buttons to set their outlines and check their states
        for (int i = 0; i < startButtons.Length; i++)
        {
            startButtonOutlines[i] = startButtons[i].GetComponent<Outline>();

            // Check if the start button was pressed in the previous session
            if (PlayerPrefs.GetInt($"StartButtonPressed_{i}", 0) == 1)
            {
                startButtonOutlines[i].effectColor = Color.green;
            }

            // Add listeners to the start and restart buttons
            int index = i; // Capture the index to use inside the lambda expressions
            startButtons[i].onClick.AddListener(() => OnStartButtonPressed(index));
            restartButtons[i].onClick.AddListener(() => OnRestartButtonPressed(index));
        }

        // Reset the panel state on start
        panelToEnable.SetActive(false);
    }

    private void OnStartButtonPressed(int buttonIndex)
    {
        // Change the outline color to green for the pressed button
        startButtonOutlines[buttonIndex].effectColor = Color.green;

        // Save the state so that the color persists after restart
        PlayerPrefs.SetInt($"StartButtonPressed_{buttonIndex}", 1);
    }

    private void OnRestartButtonPressed(int buttonIndex)
    {
        // Check if the button was pressed before to avoid counting it again
        if (PlayerPrefs.GetInt($"RestartButtonPressed_{buttonIndex}", 0) == 0)
        {
            // Increment the count of unique buttons pressed
            PlayerPrefs.SetInt($"RestartButtonPressed_{buttonIndex}", 1);

            // Check how many unique restart buttons have been pressed
            
            for (int i = 0; i < restartButtons.Length; i++)
            {
                uniqueButtonCount += PlayerPrefs.GetInt($"RestartButtonPressed_{i}", 0);
            }

            // Log the current count of unique buttons pressed
            Debug.Log($"Unique Buttons Pressed: {uniqueButtonCount}");

            // Check if the count reaches 10 (all buttons pressed)
            if (uniqueButtonCount >= 10)
            {
                // Reload the current scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                // Start the coroutine to enable the panel after 2 seconds
                StartCoroutine(EnablePanelAfterDelay(2f));
            }
        }
        else
        {
            Debug.Log($"Button {buttonIndex} has already been pressed before.");
        }
    }

    private IEnumerator EnablePanelAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Enable the panel after the delay
        panelToEnable.SetActive(true);
        Debug.Log("Panel Enabled after 2 seconds delay! 10 unique buttons have been pressed.");
    }

    private void OnApplicationQuit()
    {
        // Loop through all start buttons and reset their states when the player quits the application
        for (int i = 0; i < startButtons.Length; i++)
        {
            PlayerPrefs.DeleteKey($"StartButtonPressed_{i}");
            PlayerPrefs.DeleteKey($"RestartButtonPressed_{i}"); // Reset the restart button press states
        }
    }
}
