using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PanelManager1 : MonoBehaviour
{
    public string jsonFilePath = "Assets/Resources/PanelData.json";

    private ParentPanelData panelData;

    void Start()
    {
        LoadPanelData();
    }

    void LoadPanelData()
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            panelData = JsonUtility.FromJson<ParentPanelData>(jsonContent);
        }
        else
        {
            Debug.LogError($"JSON file not found at path: {jsonFilePath}");
        }
    }

    public void ShowPanel(string panelType)
    {
        ChildPanelData[] selectedPanel = panelType == "Sexual" ? panelData.SexualPanel : panelData.AsexualPanel;

        foreach (var child in selectedPanel)
        {
            GameObject childPanel = GameObject.Find(child.childName);

            if (childPanel != null)
            {
                Text textComponent = childPanel.GetComponentInChildren<Text>();
                if (textComponent != null) textComponent.text = child.text;

                Image imageComponent = childPanel.GetComponentInChildren<Image>();
                if (imageComponent != null) imageComponent.sprite = LoadSprite(child.imagePath);
            }
            else
            {
                Debug.LogError($"Child panel not found: {child.childName}");
            }
        }
    }

    Sprite LoadSprite(string path)
    {
        Texture2D texture = new Texture2D(2, 2);
        if (File.Exists(path))
        {
            byte[] imageData = File.ReadAllBytes(path);
            texture.LoadImage(imageData);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError($"Image file not found at path: {path}");
            return null;
        }
    }
}
