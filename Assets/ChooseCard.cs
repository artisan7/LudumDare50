using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCard : MonoBehaviour
{
    const float CARD_WIDTH = 1.5f;
    const float SPACING = 1f;
    const float START_X = -5f;
    const float Y_POS = 0f;

    public Text textGiven;
    public AudioClip sfx;

    private List<GameObject> cardObjects;
    private int counter = 0;

    public void Setup()
    {
        counter = 5;
        Time.timeScale = 0;

        gameObject.SetActive(true);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        // choose random cards
        cardObjects = new List<GameObject>();
        List<int> cardIndices = new List<int>(); // used to avoid repeating cards
        List<Card> cards = GameController.Instance.cards;

        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(3, cards.Count); // exclude first 3 cards

            while (cardIndices.Contains(r)) // make sure the card is not chosen yet
                r = Random.Range(3, cards.Count); // exclude first 3 cards

            cardIndices.Add(r);
            GameObject cardObject = Instantiate(cards[r].gameObject);
            cardObjects.Add(cardObject);

            float xPos = START_X + ((CARD_WIDTH + SPACING) * i);
            Vector3 newPos = new Vector3(xPos, Y_POS, -7f);
            cardObject.transform.position = newPos;
            cardObject.SetActive(true);
            cardObject.GetComponent<Card>().UseForMenu();
        }

        textGiven.text = $"Choose {counter} times to proceed to the next enemy";
    }

    public void Increment()
    {
        GameController.Instance.SfxPlayer.PlayOneShot(sfx, 0.25f);
        counter--;

        foreach (GameObject o in cardObjects)
            Destroy(o);

        if (counter > 0)
        {
            UpdateDisplay();
        }
        else
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
            GameController.Instance.ShuffleDrawPile();
            GameController.Instance.FillHand();
        }
    }
}
