public class LifeUpCard : Card
{
    public int hpGainAmount;

    public override void PlayCard()
    {
        GameController.Instance.maxHp += hpGainAmount;
        GameController.Instance.currentHp += hpGainAmount;
        base.PlayCard();
    }
}
