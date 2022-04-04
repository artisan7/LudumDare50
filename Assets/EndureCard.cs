public class EndureCard : Card
{
    public override void PlayCard()
    {
        GameController.Instance.endureIndicator.gameObject.SetActive(true);
        GameController.Instance.hasEndure = true;
        base.PlayCard();
    }
}
