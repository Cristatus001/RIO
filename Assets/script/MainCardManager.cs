/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCardManager : MonoBehaviour
{
    public List<CardController> cards = new List<CardController>();  // List to hold all card controllers
    public GameObject newPanel;  // Reference to the new panel
    private bool panelDelayActive = false;

    public CountdownTimer timer;

    private void Start()
    {
        ShuffleCards(); // Shuffle cards at the start
        StartCoroutine(StartCardSequences());
    }

    private void ShuffleCards()
    {
        // Separate cards into right and wrong lists
        List<CardController> rightCards = new List<CardController>();
        List<CardController> wrongCards = new List<CardController>();

        foreach (CardController card in cards)
        {
            if (card.CompareTag("Right"))
            {
                rightCards.Add(card);
            }
            else
            {
                wrongCards.Add(card);
            }
        }

        // Shuffle the right and wrong cards independently
        ShuffleList(rightCards);
        ShuffleList(wrongCards);

        // Rebuild the original cards list
        cards.Clear();
        cards.AddRange(rightCards);
        cards.AddRange(wrongCards);
    }

    private void ShuffleList(List<CardController> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // Get a random index
            // Swap
            CardController temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private IEnumerator StartCardSequences()
    {
        yield return new WaitForSeconds(10f);

        // Start the rotation sequence for each card
        foreach (CardController card in cards)
        {
            if (card != null)  // Check if the card is not null before accessing
            {
                card.StartRotationSequence();
            }
        }
    }

    public void CheckAllRightCardsFlipped()
    {
        // Check if all right cards have been flipped
        bool allRightFlipped = true;

        foreach (CardController card in cards)
        {
            if (card != null && card.CompareTag("Right") && !card.isFlipped)  // Check for null and if the card is not flipped
            {
                allRightFlipped = false;
                break;
            }
        }

        if (allRightFlipped)
        {
            StartCoroutine(ShowNewPanelWithDelay());
        }
    }

    private IEnumerator ShowNewPanelWithDelay()
    {
        panelDelayActive = true;  // Activate delay to prevent further clicks on wrong cards
        yield return new WaitForSeconds(5f);  // Delay before showing the new panel
        newPanel.SetActive(true);  // Enable the new panel
        panelDelayActive = false;  // Disable the delay after showing the panel
        ResetCards();  // Changed from resertCards to ResetCards for proper naming
    }

    public bool IsPanelDelayActive()
    {
        return panelDelayActive;  // Check if panel delay is active
    }

    public void ResetCards()  // Changed from resertCards to ResetCards for proper naming
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)  // Check if the card is not null
            {
                cards[i].gameObject.SetActive(true);
                cards[i].currentRotationAngle = 0f;
                cards[i].gameObject.transform.rotation = Quaternion.identity;

                if (cards[i].transform.childCount > 0)
                {
                    cards[i].transform.GetChild(0).gameObject.SetActive(true);
                }

                cards[i].isRotating = true;
            }
            else
            {
                Debug.LogWarning("Card at index " + i + " is null.");
            }
        }

        if (timer != null)  // Check if the timer object is not null
        {
            timer.currentTime = 0;
            StartCoroutine(timer.StartCountdown());
        }
        else
        {
            Debug.LogWarning("Timer object is null.");
        }
    }
}
*/