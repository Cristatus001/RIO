using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ButtonData
{
    public string id;
    public List<string> images;
}

[System.Serializable]
public class ButtonDataCollection
{
    public List<ButtonData> buttons;
}

public class ImageManager : MonoBehaviour
{
    public GameObject imageContainer; // Parent object for displaying images
    public GameObject imagePrefab;   // Prefab for displaying an image
    private ButtonDataCollection data;

    void Start()
    {
        // Load JSON file
        string path = Path.Combine(Application.streamingAssetsPath, "imageData.json");
        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<ButtonDataCollection>(json);
    }

    public void OnButtonClick(string buttonId)
    {
        // Clear existing images
        foreach (Transform child in imageContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Find button data
        ButtonData buttonData = data.buttons.Find(b => b.id == buttonId);
        if (buttonData != null)
        {
            foreach (string imageName in buttonData.images)
            {
                // Load and display image (assuming images are in Resources folder)
                Sprite image = Resources.Load<Sprite>(imageName);
                if (image != null)
                {
                    GameObject newImage = Instantiate(imagePrefab, imageContainer.transform);
                    newImage.GetComponent<UnityEngine.UI.Image>().sprite = image;
                }
                else
                {
                    Debug.LogWarning($"Image {imageName} not found in Resources.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Button ID {buttonId} not found in JSON.");
        }
    }
}
