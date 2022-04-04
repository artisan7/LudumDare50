public class RecycleCard : Card
{
    public override void PlayCard()
    {
        GameController.Instance.DoRecycleCard();
        base.PlayCard();
    }
}
