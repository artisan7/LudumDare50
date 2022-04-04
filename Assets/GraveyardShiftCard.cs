public class GraveyardShiftCard : Card
{
    public override void PlayCard()
    {
        GameController.Instance.DoGraveyardShiftCard();
        base.PlayCard();
    }
}
