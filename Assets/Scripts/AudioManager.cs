using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource backgroundMusic;
    private bool isMuted = false;

    private void Awake()
    {
        // Ensure there's only one instance of AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep the audio manager between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted; // Toggle the mute state
        backgroundMusic.mute = isMuted; // Mute or unmute the audio source
    }

}
