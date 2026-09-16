using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class JsonLoader : MonoBehaviour
{
    public string jsonFileName = "Data.Json";

    void Start()
    {
        // Start loading JSON when the script runs
        StartCoroutine(LoadJsonFromStreamingAssets());
    }

    IEnumerator LoadJsonFromStreamingAssets()
    {
        string url = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        Debug.Log("Loading JSON from: " + url);

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL case: Use UnityWebRequest
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonData = webRequest.downloadHandler.text;
                Debug.Log("Loaded JSON: " + jsonData);
            }
            else
            {
                Debug.LogError("Failed to load JSON: " + webRequest.error);
            }
        }
#else
        // Other platforms: Use direct file read
        if (File.Exists(url))
        {
            string jsonData = File.ReadAllText(url);
            Debug.Log("Loaded JSON: " + jsonData);
        }
        else
        {
            Debug.LogError("JSON file not found: " + url);
        }
        yield return null; // Ensure all paths return a value for the coroutine
#endif
    }
}
