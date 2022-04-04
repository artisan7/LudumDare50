public class FortifyCard : Card
{
    public int defenseGain;

    public override void PlayCard()
    {
        GameController.Instance.defense += defenseGain;
        base.PlayCard();
    }
}
