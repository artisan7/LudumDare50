public class ReviveCard : Card
{
    public override void PlayCard()
    {
        GameController.Instance.reviveIndicator.gameObject.SetActive(true);
        GameController.Instance.reviveCount++;
        base.PlayCard();
    }
}
