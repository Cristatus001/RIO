using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource buttonAudioSource; // Public AudioSource to assign in the Inspector
    public AudioClip buttonClickSound;    // Public AudioClip to assign the sound

    public void PlaySound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound); // Play the sound using the assigned AudioSource
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is not assigned!");
        }
    }
}
