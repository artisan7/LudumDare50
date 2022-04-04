using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Turn this into a Singleton
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    // constants here
    const float HAND_Y = -4.7f;
    const float CARD_WIDTH = 1.5f;
    const float HAND_SPACING = 0.25f;
    
    // variables here
    public new Camera camera;
    public Collider2D playingField;
    public GameObject cardHighlight;
    public Image cardPreview;
    public AudioSource SfxPlayer;
    public AudioClip hitSfx;
    public AudioClip reviveSfx;
    public AudioClip endureSfx;
    public AudioClip deathSfx;
    public EffectControl effectControl;
    public Sprite hitImg;
    public Sprite endureImg;
    public Sprite reviveImg;
    public Image dualCastIndicator;
    public Image endureIndicator;
    public Image reviveIndicator;
    public GameObject GameOverScreen;
    public GameObject MainMenuScreen;
    public Enemy enemy;
    // card management variables
    public List<Card> cards; // list of all card types
    private List<Card> drawPile;
    private List<Card> hand;
    private List<Card> discardPile;
    // ui variables
    public Text textDrawCount;
    public Text textDiscardCount;
    public Slider hpBar;
    public Image defenseIndicatorIcon;
    public Text textDefenseIndicator;
    public Text textBlockIndicator;
    public ChooseCard chooseCardMenu;
    // player variables
    public int currentHp;
    public int maxHp;
    public int defense;
    public int block;
    public int reviveCount;
    public bool hasEndure;
    public bool hasDualCast;

    bool onMenu = true;


    // Start is called before the first frame update
    void Start()
    {
        MainMenuScreen.SetActive(true);
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (onMenu && Input.GetKeyDown(KeyCode.Space))
            StartGame();
    }

    private void StartGame()
    {
        Time.timeScale = 1;

        // player setup
        currentHp = maxHp;

        drawPile = new List<Card>();

        // add 10 heal, 10 block, and 10 fortify cards to deck
        for (int i = 0; i < 10; i++)
            AddToDeck(cards[0], true);
        for (int i = 0; i < 10; i++)
            AddToDeck(cards[1], true);
        for (int i = 0; i < 10; i++)
            AddToDeck(cards[2], true);

        Shuffle(drawPile);

        // starting hand (5 cards)
        hand = new List<Card>();
        FillHand();

        // Initialize Discard pile
        discardPile = new List<Card>();

        // update UI displays
        UpdateCardPreview(null);
        UpdateStatsDisplay();

        enemy.StartGame();

        MainMenuScreen.SetActive(false);
        GameOverScreen.SetActive(false);
        onMenu = false;
    }

    public void AddToDeck(Card card, bool newInstance = false)
    {
        Card cardInstance = card;
        if (newInstance)
            cardInstance = Instantiate(card);
        cardInstance.gameObject.SetActive(false);

        drawPile.Add(cardInstance);
        UpdateDrawPileText();
    }

    // call this everytime hand gets updated
    private void UpdateHand()
    {
        float startX = ((hand.Count - 1) * CARD_WIDTH) + ((hand.Count - 1) * HAND_SPACING);
        startX = -(startX - startX / 2);

        for (int i = 0; i < hand.Count; i++)
        {
            float xPos = startX + ((CARD_WIDTH + HAND_SPACING) * i);
            Vector2 newPos = new Vector2(xPos, HAND_Y);
            hand[i].gameObject.transform.position = newPos;
            hand[i].PositionOnHand = newPos;
        }
    }

    void UpdateDrawPileText()
    {
        textDrawCount.text = drawPile.Count.ToString();
    }

    void UpdateDiscardPileText()
    {
        textDiscardCount.text = discardPile.Count.ToString();
    }

    public void DiscardCard(Card card)
    {
        hand.Remove(card);
        discardPile.Add(card);
        UpdateDiscardPileText();
    }
    
    // doUpdateHand indicates if hand is rerendered after draw. Useful when drawing a lot of cards at once
    public bool DrawCard(bool doUpdateHand = true)
    {
        Card c = Pop(drawPile);

        if (c != null)
        {
            c.MoveToHand();
            Push(hand, c.GetComponent<Card>());
            UpdateDrawPileText();
        }

        if (doUpdateHand)
            UpdateHand();

        return c != null;
    }

    public void UpdateCardPreview(Sprite sprite)
    {
        if (sprite == null)
        {
            cardPreview.gameObject.SetActive(false); 
        }
        else
        {
            cardPreview.gameObject.SetActive(true);
            cardPreview.sprite = sprite;
        }
    }
    
    // update UI for hp, defense and block stats
    public void UpdateStatsDisplay()
    {
        hpBar.maxValue = maxHp;
        hpBar.value = currentHp;

        if (defense > 0) // Defense Text
            textDefenseIndicator.text = defense.ToString();
        else
            textDefenseIndicator.text = "";

        if (block > 0) // Block Text
            textBlockIndicator.text = $"+{block}";
        else
            textBlockIndicator.text = "";

        if (defense == 0 && block == 0) // Shield Icon
            defenseIndicatorIcon.gameObject.SetActive(false);
        else
            defenseIndicatorIcon.gameObject.SetActive(true);

        // indicators at bottom of hp bar
        dualCastIndicator.gameObject.SetActive(hasDualCast);
        endureIndicator.gameObject.SetActive(hasEndure);
        reviveIndicator.gameObject.SetActive(reviveCount > 0);
    }

    public void TakeDamage(int dmg)
    {
        SfxPlayer.PlayOneShot(hitSfx, 0.25f);
        effectControl.ActivateEffect(hitImg);

        // block
        if (block < dmg) // if block cannot cover all damage
        {
            dmg -= block;
            block = 0;
        }
        else
        {
            block = 0;
            dmg = 0;
        }
        // defense
        if (defense < dmg) // if defense cannot cover all damage
        {
            dmg -= defense;
            defense = 0;
        }
        else
        {
            defense -= dmg;
            dmg = 0;
        }
        // hp
        currentHp = Mathf.Max(0, currentHp - dmg);

        if (currentHp <= 0)
        {
            if (hasEndure) // do endure
            {
                SfxPlayer.PlayOneShot(endureSfx, 0.25f);
                effectControl.ActivateEffect(endureImg);
                currentHp = 1;
            }
            else if (reviveCount > 0) // do revive
            {
                SfxPlayer.PlayOneShot(reviveSfx, 0.25f);
                effectControl.ActivateEffect(reviveImg);
                reviveCount--;
                currentHp = maxHp;
            }
            else
            {
                GameOver();
            }
        }

        hasEndure = false;
        hasDualCast = false;

        UpdateStatsDisplay();
    }

    private void GameOver()
    {
        GameOverScreen.SetActive(true);
        SfxPlayer.PlayOneShot(deathSfx, 0.25f);
        GameOverScreen.SetActive(true);
        Time.timeScale = 0;
        onMenu = true;
    }

    public void DoRetry()
    {
        GameOverScreen.SetActive(false);
        Time.timeScale = 1;
    }

    // Method called for Recycle Card effect
    public void DoRecycleCard()
    {
        Shuffle(discardPile);
        int noOfCardsToMove = Mathf.Min(discardPile.Count, 5);
        for (int i = 0; i < noOfCardsToMove; i++)
        {
            Card card = Pop(discardPile);
            Push(drawPile, card);
        }
        Shuffle(drawPile);

        FillHand();
        UpdateDrawPileText();
        UpdateDiscardPileText();
    }

    // Method called for Graveyard Shift Card effect
    public void DoGraveyardShiftCard()
    {
        List<Card> temp = drawPile;
        drawPile = discardPile;
        discardPile = temp;

        FillHand();
        UpdateDrawPileText();
        UpdateDiscardPileText();
    }

    // Fills hand to total 5 cards
    public void FillHand()
    {
        int noOfCardsToDraw = 5 - hand.Count;
        for (int i = 0; i < noOfCardsToDraw; i++)
            DrawCard(false);

        UpdateHand();
    }

    // custom push and pop cos i wanna use list loool
    private void Push(List<Card> cards, Card c)
    {
        cards.Add(c);
    }
    private Card Pop(List<Card> cards)
    {
        Card c = null;

        if (cards.Count > 0) {
            c = cards.Last();
            cards.RemoveAt(cards.Count - 1);
        }

        return c;
    }

    // fisher-yates shuffle copied from https://stackoverflow.com/questions/273313/randomize-a-listt
    public static void Shuffle(List<Card> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            Card value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void ShuffleDrawPile()
    {
        Shuffle(drawPile);
    }
}
