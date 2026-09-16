using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Check if an instance of the MusicManager already exists
        if (instance == null)
        {
            instance = this; // Assign this as the instance
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed when loading a new scene
        }
        else
        {
            Destroy(gameObject); // Destroy this duplicate instance
        }
    }
}
