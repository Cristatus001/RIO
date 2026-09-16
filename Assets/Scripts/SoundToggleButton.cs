using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnSprite;   // Assign the "sound on" sprite in the inspector
    public Sprite soundOffSprite;  // Assign the "sound off" sprite in the inspector

    private Button button;
    private bool isSoundOn;  // Local state tracking

    void Start()
    {
        button = GetComponent<Button>();

        // Initialize the sound state based on AudioManager's current state
        //isSoundOn = AudioManager.instance.isMusicOn;

        // Add the listener for the button click
        button.onClick.AddListener(ToggleSound);

        // Initialize the button sprite based on the sound state
        UpdateButtonSprite();
    }

    void ToggleSound()
    {
        // Toggle the sound state and inform the AudioManager
        isSoundOn = !isSoundOn;

        //AudioManager.instance.ToggleMusic(isSoundOn);

        // Update the button sprite
        UpdateButtonSprite();
    }

    void UpdateButtonSprite()
    {
        if (isSoundOn)
        {
            button.image.sprite = soundOnSprite;  // Show the sound on sprite
        }
        else
        {
            button.image.sprite = soundOffSprite; // Show the sound off sprite
        }
    }
}