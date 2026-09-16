using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomizeButtonPos : MonoBehaviour
{
    public RectTransform[] buttons; // Assign your button RectTransforms here
    public VerticalLayoutGroup layoutGroup; // Reference to the VerticalLayoutGroup component

    void Start()
    {
        if (buttons.Length == 0)
        {
            Debug.LogWarning("No buttons assigned for randomization.");
            return;
        }

        RandomizePositions();
    }

    public void RandomizePositions()
    {
        // Disable the layout group temporarily to rearrange the buttons
        layoutGroup.enabled = false;

        // Shuffle the buttons array
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform temp = buttons[i];
            int randomIndex = Random.Range(i, buttons.Length);
            buttons[i] = buttons[randomIndex];
            buttons[randomIndex] = temp;
        }

        // Rearrange the buttons in the hierarchy to match the shuffled order
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetSiblingIndex(i);
        }

        // Re-enable the layout group to update the layout
        layoutGroup.enabled = true;
    }
}
