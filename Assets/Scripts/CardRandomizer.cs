using System.Collections.Generic;
using UnityEngine;

public class CardRandomizer : MonoBehaviour
{
    // List of all card objects in the scene
    public List<GameObject> cards;

    // List of predefined positions for the cards
    public List<Transform> cardPositions;

    private void Start()
    {
        RandomizeCardPositions();
    }

    private void RandomizeCardPositions()
    {
        // Create a temporary list of positions to keep track of available positions
        List<Transform> availablePositions = new List<Transform>(cardPositions);

        // Loop through each card and assign a random position
        foreach (GameObject card in cards)
        {
            // Get a random index from the available positions
            int randomIndex = Random.Range(0, availablePositions.Count);

            // Assign the card to the random position
            card.transform.position = availablePositions[randomIndex].position;

            // Remove the used position from the available list
            availablePositions.RemoveAt(randomIndex);
        }
    }
}
