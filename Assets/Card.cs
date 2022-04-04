using UnityEngine;

public class Card : MonoBehaviour
{
    public AudioClip sfx;
    public Sprite effectImage;

    enum State
    {
        None, // default
        OnHand, // on player's hand
        OnHold, // when dragged by mouse
        OnPlay, // applying effects
        OnMenu, // used on menu
    }
    State state;
    bool isOnField;
    bool canDraw;
    GameObject cardHighlight = null;

    public Vector2 PositionOnHand { set; get; }

    // Start is called before the first frame update
    void Start()
    {
        isOnField = false;
        canDraw = true;

        // Load highlight border
        cardHighlight = Instantiate(GameController.Instance.cardHighlight, transform);
        cardHighlight.SetActive(false);
    }

    public void MoveToHand()
    {
        state = State.OnHand;
        gameObject.SetActive(true);
    }

    // called in choose card menu
    public void UseForMenu()
    {
        state = State.OnMenu;
    }

    public virtual void PlayCard()
    {
        cardHighlight.SetActive(false);

        GameController.Instance.DiscardCard(this);
        gameObject.SetActive(false);

        if (canDraw)
            GameController.Instance.DrawCard();

        GameController.Instance.UpdateStatsDisplay();

        GameController.Instance.SfxPlayer.PlayOneShot(sfx, 0.25f);
        GameController.Instance.effectControl.ActivateEffect(effectImage);

        state = State.None;
        canDraw = false;
    }

    private void OnMouseEnter()
    {
        Debug.Log(state.ToString());
        cardHighlight.SetActive(true);
        if (state == State.OnHand || state == State.OnMenu)
        {
            if (state == State.OnHand)
                transform.position = PositionOnHand + new Vector2(0, 0.5f);

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            GameController.Instance.UpdateCardPreview(sprite);
        }
    }

    private void OnMouseExit()
    {
        cardHighlight.SetActive(false);
        if (state == State.OnHand)
        {
            transform.position = PositionOnHand;
            GameController.Instance.UpdateCardPreview(null);
        }
        else if (state == State.OnMenu)
        {
            GameController.Instance.UpdateCardPreview(null);
        }
    }

    private void OnMouseDown()
    {
        if (state == State.OnMenu)
        {
            cardHighlight.SetActive(false);
            GameController.Instance.AddToDeck(this, true);
            GameController.Instance.chooseCardMenu.Increment();
        }
    }

    private void OnMouseDrag()
    {
        if (state == State.OnHand || state == State.OnHold)
        {
            Vector3 mousePosition = GameController.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, -0.5f);
            GameController.Instance.UpdateCardPreview(null);
            state = State.OnHold;

            if (isOnField)
                GetComponent<SpriteRenderer>().color = Color.green;
            else
                GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnMouseUp()
    {
        if (state == State.OnHold)
        {
            if (isOnField)
            {
                state = State.OnPlay;

                if (GameController.Instance.hasDualCast) // plays twice if player has dualcast
                    PlayCard();

                PlayCard();
            }
            else
            {
                state = State.OnHand;
                transform.position = PositionOnHand;
            }

            GetComponent<SpriteRenderer>().color = Color.white;
            canDraw = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == GameController.Instance.playingField)
            isOnField = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == GameController.Instance.playingField)
            isOnField = false;
    }
}
