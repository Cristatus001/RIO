using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.Networking;
using System.Linq;

[System.Serializable]
public class Organism
{
    public string name;
    public string type;
    public string image;
    public int id;
    public string question;
    public List<string> options;
    public int correctOption;
    public string correctImage;
    public string semifinalImage;
    public string finalImage;
    public string finalText;

    public List<GridImage> imageGrid;
}

[System.Serializable]
public class OrganismList
{
    public List<Organism> organisms;
}

[System.Serializable]
public class GridImage
{
    public string image;      // Image file name
    public bool isCorrect;    // Whether it's a correct answer
}

public class OrganismManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab for buttons
    public Transform buttonParent;  // Parent object to hold buttons
    private string jsonFileName = "Datafile.json";

    public GameObject nextPanel;       // Reference to the next panel
    public Image OrganismImage;  // Child image in the next panel

    private Organism selectedOrganism; // To store the clicked organism

    public GameObject answerPanel;     // Panel for displaying the question and options
    public GameObject wrongPanel;      // Panel to display wrong choice message
    public TextMeshProUGUI questionText;          // Text component for the question
    public List<TextMeshProUGUI> optionTexts;     // List of Text components for options
    public Button sexualButton;        // Button for Sexual
    public Button asexualButton;       // Button for Asexual

    public GameObject correctPanel;  // Panel for correct answer
    private int correctOptionIndex; // Correct option for the selected organism

    public Image correctPanelImage;

    public GameObject gridParent;       // Parent object for the image grid
    public GameObject cardPrefab;      // Prefab for individual images
    public GameObject imageGridPanel;   // Panel containing the grid of images
    public Vector2 areaSize = new Vector2(500, 500); // Bounds for random positioning
    public float maxRotationAngle = 25f; // Maximum rotation angle for randomization

    private List<GridImage> gridImages; // Stores images for the current organism
    public List<GameObject> instantiatedCards = new List<GameObject>();

    private bool canClickCards = false; // Flag to allow card clicks
    private int correctCardsClicked = 0; // Track number of correct cards clicked
    private int totalCorrectCards; // Track total number of correct cards

    public GameObject finalPanel;
    public Image semifinal_Image;
    public Image final_Image;
    public TextMeshProUGUI final_Text;
    public List<GameObject> dialogueBoxes; // List of dialogue boxes
    public List<GameObject> dialogueBoxes2; // List of dialogue boxes
    public AudioSource audioSource; // Audio feedback source
    public AudioClip correctAudio; // Audio clip for correct answer
    public AudioClip wrongAudio; // Audio clip for wrong answer

    private GameObject currentDialogueBox;

    //public AudioClip wrongAudio; // Audio clip for wrong answer
    public GameObject dialogueBox2; // Dialogue box for feedback
    public AudioSource audioSource2; // Audio feedback source
    public AudioClip correctAudio2; // Audio clip for correct answer
    public CountdownTimer countdownTimer; // Reference to the CountdownTimer script
    public TextMeshProUGUI encouragementText; // Assign this in the Inspector
    //public Image encouragementImage;
    void Start()
    {
        // Ensure the AudioSource for correct and wrong answers are assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (wrongAudioSource == null)
        {
            wrongAudioSource = gameObject.AddComponent<AudioSource>(); // Add a new AudioSource if not assigned
        }
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        StartCoroutine(LoadJsonFromStreamingAssets(path));
        //LoadAndCreateButtons();
        AssignButtonListeners();
    }

    public void OnNextButtonClicked()
    {
        // Activate the grid panel
        imageGridPanel.SetActive(true);

        // Shuffle images
        ShuffleImages();

        // Clear existing cards
        foreach (Transform child in gridParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate 16 cards with randomized positions and rotations
        int imagesLoaded = 0;  // Variable to track number of images loaded
        foreach (var gridImage in gridImages)
        {
            GameObject cardObject = Instantiate(cardPrefab, gridParent.transform);
            instantiatedCards.Add(cardObject);

            // Set random position
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );

            // Set random rotation
            float randomRotation = Random.Range(-maxRotationAngle, maxRotationAngle);
            cardRect.rotation = Quaternion.Euler(0, 0, randomRotation);

            // Start loading image
            StartCoroutine(LoadImageFromStreamingAssets(gridImage.image, cardObject, () => {
                imagesLoaded++;  // Increment the loaded image count
                if (imagesLoaded == gridImages.Count)
                {
                    // All images are loaded, start the timer
                    //StartTimer();
                }
            }));

            // Add click listener
            Button cardButton = cardObject.GetComponent<Button>();
            bool isCorrect = gridImage.isCorrect;
            cardButton.onClick.AddListener(() => OnCardClicked(cardObject, isCorrect));
        }

        // Set the total number of correct cards
        totalCorrectCards = gridImages.FindAll(card => card.isCorrect).Count;
        //FlipAllCardsMultipleTimes();
    }

    private IEnumerator LoadImageFromStreamingAssets(string imageName, GameObject cardObject, System.Action onImageLoaded)
    {
        string imagePath;

#if UNITY_ANDROID && !UNITY_EDITOR
    // Android: Use UnityWebRequest with a URI
    imagePath = Path.Combine(Application.streamingAssetsPath, "Card_Images", imageName);
    UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);
    yield return request.SendWebRequest();
    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"Error loading image: {request.error}");
        onImageLoaded?.Invoke();
        yield break;
    }
    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
#elif UNITY_IOS || UNITY_WEBGL || UNITY_EDITOR
        // iOS, WebGL, Editor: Read directly from the file system
        imagePath = Path.Combine(Application.streamingAssetsPath, "Card_Images", imageName);

        // Use WWW for WebGL or File.ReadAllBytes for iOS/Editor
#if UNITY_WEBGL
        WWW www = new WWW(imagePath);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError($"Error loading image: {www.error}");
            onImageLoaded?.Invoke();
            yield break;
        }
        Texture2D texture = www.texture;
#else
    if (!File.Exists(imagePath))
    {
        Debug.LogError($"Image file not found: {imagePath}");
        onImageLoaded?.Invoke();
        yield break;
    }
    byte[] imageData = File.ReadAllBytes(imagePath);
    Texture2D texture = new Texture2D(2, 2);
    texture.LoadImage(imageData);
#endif
#else
    Debug.LogError("Unsupported platform for loading images.");
    onImageLoaded?.Invoke();
    yield break;
#endif

        // Assign the texture to the GameObject
        Image cardImage = cardObject.GetComponent<Image>();
        cardImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Invoke the callback
        onImageLoaded?.Invoke();
    }



    private void StartTimer()
    {
        //StartCoroutine(countdownTimer.StartCountdown());
        StartCoroutine(WaitForTimerToEnd());
    }

    private IEnumerator WaitForTimerToEnd()
    {
        yield return new WaitForSeconds(countdownTimer.timerDuration);
        FlipAllCardsMultipleTimes();
    }


    public void ShuffleImages()
    {
        for (int i = gridImages.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GridImage temp = gridImages[i];
            gridImages[i] = gridImages[randomIndex];
            gridImages[randomIndex] = temp;
        }
    }

    private void OnCardClicked(GameObject card, bool isCorrect)
    {
        if (!canClickCards || card == null)
        {
            Debug.LogWarning("Card clicks are disabled or the card is null.");
            return; // Ignore clicks if cards can't be clicked or the card is null
        }

        // Ensure the card has at least one child
        if (card.transform.childCount == 0)
        {
            Debug.LogError($"Card {card.name} does not have any children to disable.");
            return;
        }

        // Get the child image
        Transform childImage = card.transform.GetChild(0);
        if (childImage == null)
        {
            Debug.LogError($"Child image not found for card {card.name}.");
            return;
        }

        // Disable the card's child image on click
        childImage.gameObject.SetActive(false);

        // Rotate the card using DOTween
        card.transform.DORotate(new Vector3(0, 180, 0), 1f).OnComplete(() =>
        {
            if (isCorrect)
            {
                // If the correct card was clicked, increase the correct count
                correctCardsClicked++;

                // If all correct cards are clicked, show encouragement message and disable further interactions
                if (correctCardsClicked >= totalCorrectCards)
                {
                    ShowEncouragementMessage();
                    canClickCards = false; // Disable further card clicks
                    Invoke("ShowSemifinalPanel", 2f); // Wait for 2 seconds before showing the semifinal panel
                }
            }
            else
            {
                // If the wrong card was clicked, disable it after 1 second
                StartCoroutine(DisableCardAfterDelay(card, 0.8f));
                // Uncomment this line if you have an audio function for wrong card clicks
                // PlayWrongAudio();
            }
        });

        Debug.Log(isCorrect ? "Correct card clicked!" : "Wrong card clicked!");
    }

    private void ShowEncouragementMessage()
    {
        string[] encouragingMessages =
        {
        "Fantastic", "Superb", "Impressive", "Outstanding", "Remarkable",
        "Terrific", "Marvelous", "Brilliant", "Phenomenal", "Top-notch",
        "Great job", "Well done", "You’ve got this", "Keep it up"
    };

        string randomMessage = encouragingMessages[Random.Range(0, encouragingMessages.Length)];

        if (encouragementText != null && encouragementText.transform.parent != null)
        {
            GameObject parentImage = encouragementText.transform.parent.gameObject;

            // Enable the parent image
            parentImage.SetActive(true);

            // Set the text instantly
            encouragementText.text = randomMessage;

            // Wait for a duration before hiding the parent image
            StartCoroutine(HideEncouragementAfterDelay(parentImage, 1.5f));
        }
    }

    private IEnumerator HideEncouragementAfterDelay(GameObject parentImage, float delay)
    {
        yield return new WaitForSeconds(delay);
        parentImage.SetActive(false);
    }



    private IEnumerator DisableCardAfterDelay(GameObject card, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (card != null)
        {
            // Disable the image and collider, but keep the object in place
            Image cardImage = card.GetComponent<Image>();
            if (cardImage != null) cardImage.enabled = false;

            Collider2D collider = card.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            Debug.Log($"Card {card.name} is disabled but remains in place.");
        }
        else
        {
            Debug.LogWarning("Card is already null when trying to disable it.");
        }
    }



    private void ShowSemifinalPanel()
    {
        finalPanel.SetActive(true); // Activate semifinal panel
    }

    /*void LoadAndCreateButtons()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            OrganismList organismList = JsonUtility.FromJson<OrganismList>(jsonContent);

            foreach (var organism in organismList.organisms)
            {
                CreateButton(organism);
            }
        }
        else
        {
            Debug.LogError($"JSON file not found at: {path}");
        }
    }*/

    IEnumerator LoadJsonFromStreamingAssets(string path)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error loading JSON file: {request.error}");
        }
        else
        {
            string jsonContent = request.downloadHandler.text;
            OrganismList organismList = JsonUtility.FromJson<OrganismList>(jsonContent);

            foreach (var organism in organismList.organisms)
            {
                CreateButton(organism);
            }
        }
    }


    void CreateButton(Organism organism)
    {
        GameObject button = Instantiate(buttonPrefab, buttonParent);
        button.name = organism.name;

        // Set button text
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = organism.name;
        }

        // Set button image
        Image buttonImage = button.transform.GetChild(0).GetComponentInChildren<Image>();
        if (buttonImage != null)
        {
            LoadImageForButton(organism.image, buttonImage);
        }

        // Add additional behavior based on type
        Button btn = button.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            OnButtonClicked(organism);
            Debug.Log($"Clicked: {organism.name}, Type: {organism.type}");
        });
    }

    private void LoadImageForButton(string imageName, Image buttonImage)
    {
        string imagePath = Path.Combine(Application.streamingAssetsPath, imageName);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);

        request.SendWebRequest().completed += (asyncOperation) =>
        {
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading image: {request.error}");
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                buttonImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        };
    }
    void OnButtonClicked(Organism organism)
    {
        selectedOrganism = organism; // Store the selected organism
        Debug.Log($"Clicked: {organism.name}, Type: {organism.type}");

        gridImages = new List<GridImage>();
        foreach (var gridImage in organism.imageGrid)
        {
            gridImages.Add(new GridImage { image = gridImage.image, isCorrect = gridImage.isCorrect });
        }

        // Rotate the button on tap
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (button != null)
        {
            button.transform.DORotate(new Vector3(0, 359, 0), 0.5f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                // Update next panel's image
                UpdateNextPanel();
                UpdatefinalPanel();
            });
        }
        else
        {
            // Update next panel's image
            UpdateNextPanel();
            UpdatefinalPanel();
        }
    }

    void UpdateNextPanel()
    {
        if (selectedOrganism == null || OrganismImage == null) return;

        StartCoroutine(LoadImageForNextPanel(selectedOrganism.image));

        // Show the next panel (assuming it's initially hidden)
        if (nextPanel != null)
        {
            nextPanel.SetActive(true);
        }
    }
    private IEnumerator LoadImageForNextPanel(string imageName)
    {
        string imagePath = Path.Combine(Application.streamingAssetsPath, imageName);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error loading image: {request.error}");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            OrganismImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    void UpdatefinalPanel()
    {
        if (selectedOrganism == null || semifinal_Image == null || final_Image == null) return;

        StartCoroutine(LoadImageForPanel(selectedOrganism.semifinalImage, semifinal_Image));
        StartCoroutine(LoadImageForPanel(selectedOrganism.finalImage, final_Image));

        final_Text.text = selectedOrganism.finalText;
    }
    private IEnumerator LoadImageForPanel(string imageName, Image targetImage)
    {
        string imagePath = Path.Combine(Application.streamingAssetsPath, imageName);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error loading image '{imageName}': {request.error}");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
    void AssignButtonListeners()
    {
        sexualButton.onClick.AddListener(() =>
        {
            OnTypeSelected("sexual", sexualButton, asexualButton);
        });

        asexualButton.onClick.AddListener(() =>
        {
            OnTypeSelected("asexual", asexualButton, sexualButton);
        });
    }

    void OnTypeSelected(string selectedType, Button selectedButton, Button otherButton)
    {
        if (selectedOrganism == null) return;

        // Disable both buttons to prevent multiple selections
        sexualButton.interactable = false;
        asexualButton.interactable = false;

        if (selectedOrganism.type == selectedType)
        {
            // Correct type selected
            selectedButton.image.color = Color.green; // Highlight the correct button in green
            ShowRandomDialogueBox();
            Invoke("ShowAnswerPanel", 2f); // Wait for 2 seconds before showing the answer panel
        }
        else
        {
            // Wrong type selected
            StartCoroutine(BlinkWrongOption(selectedButton)); // Blink the wrong button in red
            PlayWrongAudio();

            // Highlight the correct button in green
            if (selectedOrganism.type == "sexual")
            {
                sexualButton.image.color = Color.green;
            }
            else if (selectedOrganism.type == "asexual")
            {
                asexualButton.image.color = Color.green;
            }

            // Enable the wrong panel after 1 second
            Invoke("ShowWrongPanel", 2f);
        }
    }

    IEnumerator BlinkWrongOption(Button wrongButton)
    {
        int blinkCount = 3; // Number of times to blink
        float blinkDuration = 0.3f; // Duration of each blink

        for (int i = 0; i < blinkCount; i++)
        {
            wrongButton.image.color = Color.clear; // Make it "invisible"
            yield return new WaitForSeconds(blinkDuration);
            wrongButton.image.color = Color.red; // Restore red color
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    void ShowWrongPanel()
    {
        wrongPanel.SetActive(true);
    }

    void ShowRandomDialogueBox()
    {
        if (dialogueBoxes.Count > 0)
        {
            // Deactivate the current dialogue box before selecting a new one
            if (currentDialogueBox != null)
            {
                currentDialogueBox.SetActive(false);
            }

            // Pick a random dialogue box
            int randomIndex = Random.Range(0, dialogueBoxes.Count);
            currentDialogueBox = dialogueBoxes[randomIndex];

            if (currentDialogueBox != null)
            {
                currentDialogueBox.SetActive(true);
            }
        }

        if (audioSource != null && correctAudio != null)
        {
            audioSource.clip = correctAudio;
            audioSource.Play();
        }

        // Hide the dialogue box after 2 seconds
        Invoke("HideCurrentDialogueBox", 2f);
    }

    void HideCurrentDialogueBox()
    {
        if (currentDialogueBox != null)
        {
            currentDialogueBox.SetActive(false);
        }
    }

    void ShowRandomDialogueBox2()
    {
        if (dialogueBoxes.Count > 0)
        {
            // Deactivate the current dialogue box before selecting a new one
            if (currentDialogueBox != null)
            {
                currentDialogueBox.SetActive(false);
            }

            // Pick a random dialogue box
            int randomIndex = Random.Range(0, dialogueBoxes2.Count);
            currentDialogueBox = dialogueBoxes2[randomIndex];

            if (currentDialogueBox != null)
            {
                currentDialogueBox.SetActive(true);
            }
        }

        if (audioSource != null && correctAudio != null)
        {
            audioSource.clip = correctAudio;
            audioSource.Play();
        }

        // Hide the dialogue box after 2 seconds
        Invoke("HideCurrentDialogueBox2", 2f);
    }

    void HideCurrentDialogueBox2()
    {
        if (currentDialogueBox != null)
        {
            currentDialogueBox.SetActive(false);
        }
    }

    void ResetButtonColors()
    {
        sexualButton.image.color = Color.white; // Reset sexual button color
        asexualButton.image.color = Color.white; // Reset asexual button color

        // Reactivate both buttons for the next interaction
        sexualButton.interactable = true;
        asexualButton.interactable = true;
    }



    void ShowAnswerPanel()
    {
        answerPanel.SetActive(true);
        nextPanel.SetActive(false);

        // Set question and options
        questionText.text = selectedOrganism.question;

        // Shuffle options and update correctOptionIndex
        List<int> indices = new List<int>();
        for (int i = 0; i < selectedOrganism.options.Count; i++)
        {
            indices.Add(i);
        }
        indices = indices.OrderBy(x => Random.Range(0, indices.Count)).ToList();
        correctOptionIndex = indices.IndexOf(selectedOrganism.correctOption);

        for (int i = 0; i < optionTexts.Count; i++)
        {
            if (i < selectedOrganism.options.Count)
            {
                optionTexts[i].text = selectedOrganism.options[indices[i]];

                // Assign click listener to each option
                int optionIndex = i; // Local copy to use in the lambda
                optionTexts[i].GetComponentInParent<Button>().onClick.RemoveAllListeners();
                optionTexts[i].GetComponentInParent<Button>().onClick.AddListener(() =>
                {
                    OnOptionSelected(optionIndex);
                });
            }
        }
    }

    // Method for handling option selection
    void OnOptionSelected(int selectedOption)
    {
        // Disable all option buttons to prevent multiple selections
        foreach (var optionText in optionTexts)
        {
            optionText.GetComponentInParent<Button>().interactable = false;
        }

        // Reset the colors of all options first
        ResetOptionColors();

        if (selectedOption == correctOptionIndex)
        {
            ShowRandomDialogueBox2();
            HighlightOption(correctOptionIndex, Color.green);
            // Load and display the correct image

            Invoke("LoadCorrectPanelImage", 2f);
        }
        else
        {
            // Wrong answer selected
            // Start blinking effect for the wrong option
            StartCoroutine(BlinkWrongOption(selectedOption));

            // Highlight the correct option in green
            HighlightOption(correctOptionIndex, Color.green);

            Invoke("DelayWrongPanel", 2);
            PlayWrongAudio();
        }
    }


    IEnumerator BlinkWrongOption(int selectedOption)
    {
        int blinkCount = 5; // Number of blinks
        float blinkInterval = 0.3f; // Duration of each blink (seconds)

        for (int i = 0; i < blinkCount; i++)
        {
            // Toggle between red and default
            HighlightOption(selectedOption, (i % 2 == 0) ? Color.red : Color.white);
            yield return new WaitForSeconds(blinkInterval);
        }

        // Reset to default color after blinking
        HighlightOption(selectedOption, Color.white);
    }

    private void DelayWrongPanel()
    {
        // Display the wrong panel
        wrongPanel.SetActive(true);
    }

    // Helper method to reset the colors of all options
    void ResetOptionColors()
    {
        foreach (var optionText in optionTexts)
        {
            optionText.GetComponentInParent<Button>().image.color = Color.white; // Assuming white is the default color
        }
    }

    // Helper method to highlight a specific option
    void HighlightOption(int optionIndex, Color color)
    {
        optionTexts[optionIndex].GetComponentInParent<Button>().image.color = color;
    }


    public void LoadCorrectPanelImage()
    {
        // Correct answer selected
        answerPanel.SetActive(false);
        correctPanel.SetActive(true);

        if (selectedOrganism == null || string.IsNullOrEmpty(selectedOrganism.correctImage)) return;

        StartCoroutine(LoadCorrectImageCoroutine(selectedOrganism.correctImage));
    }
    private IEnumerator LoadCorrectImageCoroutine(string imageName)
    {
        string imagePath = Path.Combine(Application.streamingAssetsPath, imageName);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error loading correct image: {imagePath}, Error: {request.error}");
        }
        else
        {
            // Load the texture and update the sprite
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            correctPanelImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            correctPanelImage.gameObject.SetActive(true); // Ensure the image is visible
        }
    }
    public void FlipAllCardsMultipleTimes()
    {
        float flipDuration = 1f; // Duration for card flips
        foreach (GameObject card in instantiatedCards)
        {
            Transform childImage = card.transform.GetChild(0);

            // Flip card 3 times (you can adjust the number of flips)
            card.transform.DORotate(new Vector3(0, 180, 0), flipDuration)
                .SetLoops(4, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    // After flipping, reveal the child image
                    childImage.gameObject.SetActive(true);
                    canClickCards = true; // Enable card clicks after flipping completes
                    Debug.Log("Cards can now be clicked.");
                });
        }
    }

    public AudioSource wrongAudioSource; // Separate AudioSource for wrong answers



    private void PlayWrongAudio()
    {
        if (wrongAudioSource != null)
        {
            if (!wrongAudioSource.enabled)
            {
                wrongAudioSource.enabled = true; // Ensure the wrongAudioSource is enabled
            }

            if (wrongAudio != null)
            {
                wrongAudioSource.clip = wrongAudio;
                wrongAudioSource.Play();
            }
            else
            {
                Debug.LogError("WrongAudio clip is not assigned in the Inspector.");
            }
        }
        else
        {
            Debug.LogError("WrongAudioSource is not assigned to the OrganismManager script.");
        }
    }



}